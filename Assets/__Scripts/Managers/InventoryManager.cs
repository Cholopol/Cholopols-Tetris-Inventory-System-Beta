using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChosTIS
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("Current Tetris Item Grid")]
        public TetrisItemGrid selectedTetrisItemGrid;
        [Header("Tetris Item Details Data")]
        public ItemDataList_SO itemDataList_SO;
        [Header("Tetris Item Points Set Data")]
        public TetrisItemPointSet_SO tetrisItemPointSet_SO;
        [Header("Depository")]
        public TetrisItemGrid depositoryGrid;
        [Header("Components")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private InventoryHighlight inventoryHighlight;
        [SerializeField] private TetrisItemGhost tetrisItemGhost;
        [SerializeField] private RightClickMenuPanel rightClickMenuPanel;
        public TetrisItem selectedTetrisItem;
        public Vector2Int tileGridOriginPosition;

        private TetrisItem overlapItem;
        private Vector2Int oldPosition;
        private int selectedItemIndex;

        private void Update()
        {
            //[Debug] Dynamically add items randomly
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (selectedItemIndex >= itemDataList_SO.itemDetailsList.Count)
                {
                    selectedItemIndex = 0;
                }
                if (selectedTetrisItem != null)
                {
                    Destroy(selectedTetrisItem.gameObject);
                    selectedTetrisItem = null;
                }
                CreateItemInOrder(selectedItemIndex);
                ++selectedItemIndex;
            }

            //Rotating item
            if (Input.GetKeyDown(KeyCode.R))
            {
                oldPosition = new();
                RotateItemGhost();
            }

            //Pick up item UI location
            if (selectedTetrisItem) selectedTetrisItem.transform.position = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                TetrisItemGrid tetrisItemGrid = GetGridUnderMouse();
                if (tetrisItemGrid != null)
                {
                    //Gets the grid coordinates of the current mouse position in the grid and prints it to the console
                    Vector2Int tileGridOriginPosition = GetTileGridOriginPosition(tetrisItemGrid);
                    if (selectedTetrisItem == null)
                    {
                        return;
                    }
                    else
                    {
                        PlaceItem(tileGridOriginPosition, tetrisItemGrid);
                        selectedItemIndex = 0;
                    }

                }

            }

            HandleHighlight(selectedTetrisItemGrid != null);
        }

        /// <summary>
        /// Gets Tetris's point coordinates
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public List<Vector2Int> GetTetrisPieceShapePos(TetrisPieceShape shape)
        {
            return tetrisItemPointSet_SO.TetrisPieceShapeList[(int)shape].points;
        }

        private void RotateItemGhost()
        {
            if (tetrisItemGhost.ItemDetails == null) return;
            tetrisItemGhost.Rotate();
        }

        /// <summary>
        /// Gets the origin grid coordinates of TetrisItemGhost, and returns the mouse location grid coordinates if the item is not picked up
        /// </summary>
        /// <returns></returns>
        public Vector2Int GetGhostTileGridOriginPosition()
        {
            if (selectedTetrisItemGrid == null) return new Vector2Int();
            Vector2 origin = Input.mousePosition;
            Vector2Int tileGridPosition = selectedTetrisItemGrid.GetTileGridPosition(origin);
            if (tetrisItemGhost.ItemDetails != null)
            {
                int offsetX = Mathf.FloorToInt((tetrisItemGhost.WIDTH - 1) / 2);
                int offsetY = Mathf.FloorToInt((tetrisItemGhost.HEIGHT - 1) / 2);
                tileGridPosition.x -= offsetX;
                tileGridPosition.y -= offsetY;
            }
            return tileGridPosition;
        }

        /// <summary>
        /// Gets the origin grid coordinates of the TetrisItem and returns the mouse location grid coordinates if the item is not picked up
        /// </summary>
        /// <returns></returns>
        private Vector2Int GetTileGridOriginPosition(TetrisItemGrid tetrisItemGrid)
        {
            Vector2 origin = Input.mousePosition;
            if (selectedTetrisItem != null)
            {
                origin.x -= (selectedTetrisItem.WIDTH - 1) * TetrisItemGrid.tileSizeWidth / 2;
                origin.y += (selectedTetrisItem.HEIGHT - 1) * TetrisItemGrid.tileSizeHeight / 2;
            }
            Vector2Int tileGridPosition = tetrisItemGrid.GetTileGridPosition(origin);
            return tileGridPosition;
        }

        private void HandleHighlight(bool isShow)
        {
            Vector2Int positionOnGrid = GetGhostTileGridOriginPosition();
            if (oldPosition == positionOnGrid) return;
            oldPosition = positionOnGrid;
            if (tetrisItemGhost.OnDragging && isShow)
            {
                inventoryHighlight.Show(true);
                inventoryHighlight.UpdateShapeHighlight(tetrisItemGhost, positionOnGrid, selectedTetrisItemGrid);
                inventoryHighlight.SetParent(selectedTetrisItemGrid);
                inventoryHighlight.SetPosition(selectedTetrisItemGrid,
                tetrisItemGhost.selectedTetrisItem,
                positionOnGrid.x, positionOnGrid.y);
            }
            else
            {
                inventoryHighlight.Show(false);
            }
        }

        /// <summary>
        /// Drag Drop to Place Tetris Item
        /// </summary>
        /// <param name="tileGridOriginPosition"></param>
        public void PlaceGhostItem(Vector2Int tileGridOriginPosition, TetrisItem tetrisItem, TetrisItemGrid targetGrid, InventorySlot fromSlot)
        {
            if (targetGrid == null) return;
            bool isDone = targetGrid.TryPlaceTetrisItem(tetrisItem, tileGridOriginPosition.x, tileGridOriginPosition.y);
            StartCoroutine(PlaceChildItem(tetrisItem, targetGrid, fromSlot));

        }

        /// <summary>
        /// Programmatic place item
        /// </summary>
        /// <param name="tileGridOriginPosition"></param>
        private void PlaceItem(Vector2Int tileGridOriginPosition, IInventoryContainer container)
        {
            if (container == null) return;
            TetrisItemGrid tetrisItemGrid = container as TetrisItemGrid;
            selectedTetrisItem.CurrentInventoryContainer = tetrisItemGrid;
            bool isDone = tetrisItemGrid.TryPlaceTetrisItem(
                ref selectedTetrisItem,
                tileGridOriginPosition.x, 
                tileGridOriginPosition.y, 
                ref overlapItem);
            overlapItem = null;
        }

        /// <summary>
        /// Place sub items in the activity backpack, nested items are not allowed in inventory
        /// </summary>
        /// <param name="parentItem"></param>
        /// <param name="targetGrid"></param>
        /// <returns></returns>
        private IEnumerator PlaceChildItem(TetrisItem parentItem, TetrisItemGrid targetGrid, InventorySlot fromSlot)
        {
            if (!parentItem.TryGetItemComponent<GridPanelComponent>(out GridPanelComponent gridPanel)) yield break;
            yield return null;
            if (gridPanel.TetrisItemGrids.Count > 0)
            {
                foreach (TetrisItemGrid fromGrid in gridPanel.TetrisItemGrids)
                {
                    if (fromGrid.OwnerItemDic.Count > 0)
                    {
                        bool isDone = false;
                        Dictionary<int, TetrisItem> _ownerItemDic = new(fromGrid.OwnerItemDic);
                        foreach (TetrisItem item in _ownerItemDic.Values)
                        {
                            int _itemPosX = item.onGridPositionX;
                            int _itemPosY = item.onGridPositionY;
                            Vector2Int _rotationOffset = item.RotationOffset;
                            List<Vector2Int> _tetrisPieceShapePos = item.TetrisPieceShapePos;
                            for (int row = 0; row < targetGrid.gridSizeHeight; row++)
                            {
                                for (int column = 0; column < targetGrid.gridSizeWidth; column++)
                                {
                                    isDone = targetGrid.TryPlaceTetrisItem(item, column, row);
                                    if (isDone)
                                    {
                                        fromGrid.RemoveTetrisItem(item, _itemPosX, _itemPosY, _rotationOffset, _tetrisPieceShapePos);
                                        item.CurrentInventoryContainer = targetGrid;
                                        item.SetItemData(item.GetInstanceID());
                                        break;
                                    }
                                }
                                if (isDone)
                                {
                                    break;
                                }
                            }
                            if (!isDone)
                            {
                                Debug.Log("Nesting items is not allowed in inventory");
                                targetGrid.RemoveTetrisItem(parentItem, parentItem.onGridPositionX, parentItem.onGridPositionY, parentItem.RotationOffset, parentItem.TetrisPieceShapePos);
                                ResetParent(parentItem, fromSlot);
                                break;
                            }
                            yield return null;
                        }
                        if (!isDone) break;
                    }
                    yield return null;
                }
            }
        }

        private void ResetParent(TetrisItem parentItem, InventorySlot fromSlot)
        {
            if (parentItem.CurrentInventoryContainer as InventorySlot == null)
            {
                fromSlot.PlaceTetrisItem(parentItem);
                parentItem.CurrentInventoryContainer = fromSlot;
                parentItem.SetItemData(parentItem.GetInstanceID());
            }
        }

        public StackableComponent CreateNewStackableItem(int itemID)
        {
            if (selectedTetrisItem) return null;
            selectedTetrisItem = Instantiate(itemDataList_SO.GetItemDetailsByID(itemID).uiPrefab).GetComponent<TetrisItem>();
            selectedTetrisItem.transform.SetParent(canvas.transform, false);
            selectedTetrisItem.transform.SetAsLastSibling();
            selectedTetrisItem.GetComponent<Image>().raycastTarget = false;
            TetrisItemGrid tetrisItemGrid = rightClickMenuPanel._currentItem.GetComponentInParent<TetrisItemGrid>();
            selectedTetrisItem.Initialize(itemDataList_SO.GetItemDetailsByID(itemID), null, tetrisItemGrid);
            selectedTetrisItem.TryGetItemComponent<StackableComponent>(out var stackableComponent);
            return stackableComponent;
        }

        private void CreateItemInOrder(int selectedItemIndex)
        {
            if (selectedTetrisItem) return;

            selectedTetrisItem = Instantiate(itemDataList_SO.GetItemDetailsByIndex(selectedItemIndex).uiPrefab).GetComponent<TetrisItem>();
            selectedTetrisItem.transform.SetParent(canvas.transform, false);
            selectedTetrisItem.transform.SetAsLastSibling();
            selectedTetrisItem.GetComponent<Image>().raycastTarget = false;
            selectedTetrisItem.Initialize(itemDataList_SO.itemDetailsList[selectedItemIndex], null, selectedTetrisItemGrid);
        }

        private TetrisItemGrid GetGridUnderMouse()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.CompareTag("TetrisGrid"))
                {
                    return result.gameObject.GetComponent<TetrisItemGrid>();
                }
            }

            return null;
        }

    }
}