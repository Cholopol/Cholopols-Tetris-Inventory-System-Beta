using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChosTIS.Utilities;

namespace ChosTIS.SaveLoadSystem
{
    public class InventorySaveLoadService : Singleton<InventorySaveLoadService>, ISaveable
    {
        [Header("ItemDetails Data")]
        public ItemDataList_SO itemDataList_SO;
        [Header("TetrisItem Database")]
        public InventoryData_SO inventoryData_SO;
        [Header("Slot Panels")]
        public InventorySlot[] inventorySlots;
        [Header("PersistentGrid")]
        public TetrisItemGrid[] persistentGridArray;
        private Dictionary<int, TetrisItem> slotItemDic = new();
        private Dictionary<int, TetrisItem> parentItemDic = new();
        private Dictionary<int, TetrisItem> normalItemDic = new();

        public string GUID => GetComponent<DataGUID>().guid;

        private void OnEnable()
        {
            ChosTIS.Utility.EventHandler.InstantiateInventoryItemUI += HandleInstantiateUI;
            ChosTIS.Utility.EventHandler.DeleteObjectEvent += HandleDeleteItem;
        }

        private void OnDisable()
        {
            ChosTIS.Utility.EventHandler.InstantiateInventoryItemUI -= HandleInstantiateUI;
            ChosTIS.Utility.EventHandler.DeleteObjectEvent -= HandleDeleteItem;
        }

        private void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();
        }

        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new()
            {
                inventoryDict = new Dictionary<string, List<GridItem>>
                {
                    { inventoryData_SO.name, inventoryData_SO.inventoryItemList }
                }
            };

            return saveData;
        }

        public void RestoreData(GameSaveData saveData)
        {
            inventoryData_SO.inventoryItemList = saveData.inventoryDict[inventoryData_SO.name];
        }

        public void HandleInstantiateUI()
        {
            StartCoroutine(InstantiateInventoryItemUICoroutine());
        }

        /// <summary>
        /// Asynchronous generation of item UI during archive loading
        /// </summary>
        /// <returns></returns>
        public IEnumerator InstantiateInventoryItemUICoroutine()
        {
            var operations = new Action[] {
                UpdateSlotItem,
                UpdateParentItem,
                SetTetrisItemGrid,
                UpdateItem,
                UpdateIndex
            };

            foreach (var op in operations)
            {
                op();
                yield return null;
            }
        }

        private void UpdateSlotItem()
        {
            foreach (GridItem item in inventoryData_SO.inventoryItemList)
            {
                if (item.isOnSlot)
                {
                    TetrisItem tetrisItem = Instantiate(itemDataList_SO.GetItemDetailsByID(item.itemID).uiPrefab).GetComponent<TetrisItem>();
                    slotItemDic.Add(item.itemIndex, tetrisItem);
                    tetrisItem.Rotated = RotationHelper.IsRotated(item.direction);
                    tetrisItem.Dir = item.direction;
                    tetrisItem.Initialize(itemDataList_SO.GetItemDetailsByID(item.itemID), null, null);
                    InventorySlot targetSlot = GetSlotByType(itemDataList_SO.GetItemDetailsByID(item.itemID).inventorySlotType);
                    targetSlot.PlaceTetrisItem(tetrisItem);
                    tetrisItem.CurrentInventoryContainer = targetSlot;
                }
            }
        }

        private void UpdateParentItem()
        {
            foreach (GridItem item in inventoryData_SO.inventoryItemList)
            {
                if (item.isParent)
                {
                    TetrisItem tetrisItem = CreateItemByData(item);
                    parentItemDic.Add(item.itemIndex, tetrisItem);
                    if (item.parentItemIndex != 0)
                    {
                        slotItemDic.TryGetValue(item.parentItemIndex, out TetrisItem parentItem);
                        parentItem.TryGetItemComponent<GridPanelComponent>(out GridPanelComponent gridPanel);
                        TetrisItemGrid grid = gridPanel.GridPanel.transform.GetChild(item.gridPIndex).GetComponent<TetrisItemGrid>();
                        tetrisItem.Initialize(itemDataList_SO.GetItemDetailsByID(item.itemID), null, grid);
                        grid.PlaceTetrisItem(tetrisItem, tetrisItem.onGridPositionX, tetrisItem.onGridPositionY);
                    }
                    else
                    {
                        TetrisItemGrid tetrisItemGrid = persistentGridArray[--item.persistentGridTypeIndex];
                        tetrisItem.Initialize(itemDataList_SO.GetItemDetailsByID(item.itemID), null, tetrisItemGrid);
                        tetrisItemGrid.PlaceTetrisItem(tetrisItem, tetrisItem.onGridPositionX, tetrisItem.onGridPositionY);
                    }
                }
            }
        }

        private void SetTetrisItemGrid()
        {
            foreach (GridItem item in inventoryData_SO.inventoryItemList)
            {
                if (item.isParent)
                {
                    parentItemDic.TryGetValue(item.itemIndex, out TetrisItem parentItem);
                    parentItem.TryGetItemComponent<GridPanelComponent>(out GridPanelComponent gridPanel);
                    gridPanel.GridPanel.gameObject.SetActive(true);
                }
            }
        }

        private void UpdateItem()
        {
            foreach (GridItem item in inventoryData_SO.inventoryItemList)
            {
                if (!item.isOnSlot && !item.isParent)
                {
                    TetrisItem tetrisItem = CreateItemByData(item);
                    normalItemDic.Add(item.itemIndex, tetrisItem);
                    TetrisItem parentItem;
                    if (slotItemDic.TryGetValue(item.parentItemIndex, out parentItem))
                    {
                        parentItem.TryGetItemComponent<GridPanelComponent>(out GridPanelComponent gridPanel);
                        TetrisItemGrid grid = gridPanel.GridPanel.transform.GetChild(item.gridPIndex).GetComponent<TetrisItemGrid>();
                        tetrisItem.Initialize(itemDataList_SO.GetItemDetailsByID(item.itemID), item, grid);
                        grid.PlaceTetrisItem(tetrisItem, tetrisItem.onGridPositionX, tetrisItem.onGridPositionY);
                    }
                    else if (parentItemDic.TryGetValue(item.parentItemIndex, out parentItem))
                    {
                        parentItem.TryGetItemComponent<GridPanelComponent>(out GridPanelComponent gridPanel);
                        TetrisItemGrid grid = gridPanel.GridPanel.transform.GetChild(item.gridPIndex).GetComponent<TetrisItemGrid>();
                        tetrisItem.Initialize(itemDataList_SO.GetItemDetailsByID(item.itemID), item, grid);
                        grid.PlaceTetrisItem(tetrisItem, tetrisItem.onGridPositionX, tetrisItem.onGridPositionY);
                        gridPanel.GridPanel.gameObject.SetActive(false);
                    }
                    else
                    {
                        TetrisItemGrid tetrisItemGrid = persistentGridArray[--item.persistentGridTypeIndex];
                        tetrisItem.Initialize(itemDataList_SO.GetItemDetailsByID(item.itemID), item, tetrisItemGrid);
                        tetrisItemGrid.PlaceTetrisItem(tetrisItem, tetrisItem.onGridPositionX, tetrisItem.onGridPositionY);
                    }
                }
                if (item.isParent)
                {
                    if (parentItemDic.TryGetValue(item.itemIndex, out TetrisItem parentItem))
                    {
                        parentItem.TryGetItemComponent<GridPanelComponent>(out GridPanelComponent gridPanel);
                        gridPanel.GridPanel.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void UpdateIndex()
        {
            foreach (GridItem item in inventoryData_SO.inventoryItemList)
            {
                if (slotItemDic.TryGetValue(item.itemIndex, out TetrisItem slotItem))
                {
                    slotItem.SetItemData(item.itemIndex);
                    //Debug.Log("A");
                }
                else if (parentItemDic.TryGetValue(item.itemIndex, out TetrisItem parentItem))
                {
                    parentItem.SetItemData(item.itemIndex);
                    //Debug.Log("B");
                }
                else if (normalItemDic.TryGetValue(item.itemIndex, out TetrisItem normalItem))
                {
                    normalItem.SetItemData(item.itemIndex);
                    //Debug.Log("C");
                }
            }
        }

        private InventorySlot GetSlotByType(InventorySlotType type)
        {
            foreach (InventorySlot slot in inventorySlots)
            {
                if (slot.GetComponent<InventorySlot>().GetInventorySlotType() == type)
                {
                    return slot;
                }
            }
            return null;
        }

        private void HandleDeleteItem()
        {
            foreach (GridItem item in inventoryData_SO.inventoryItemList)
            {
                GameObject itemObj = InstanceIDManager.GetObject<TetrisItem>(item.itemIndex).gameObject;
                if (itemObj != null)
                {
                    Destroy(itemObj);
                }
            }
            inventoryData_SO.inventoryItemList.Clear();
        }

        private TetrisItem CreateItemByData(GridItem gridItem)
        {
            ItemDetails details = itemDataList_SO.GetItemDetailsByID(gridItem.itemID);
            TetrisItem item = Instantiate(details.uiPrefab).GetComponent<TetrisItem>();
            item.Rotated = RotationHelper.IsRotated(gridItem.direction);
            item.Dir = gridItem.direction;
            item.onGridPositionX = gridItem.orginPosition.x;
            item.onGridPositionY = gridItem.orginPosition.y;
            return item;
        }

    }
}