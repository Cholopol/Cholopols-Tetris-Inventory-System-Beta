using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static ChosTIS.Utilities;
using ChosTIS.SaveLoadSystem;

namespace ChosTIS
{
    public class TetrisItem : MonoBehaviour, IPointerEnterHandler, ITetrisRotatable
    {
        //Private fields
        private List<IItemComponent> _components = new();
        private Image image;

        //Public fields
        public ItemDetails ItemDetails { get; set; }
        public IInventoryContainer CurrentInventoryContainer { get; set; }
        public InventorySlotType InventorySlotType { get; set; } = InventorySlotType.Pocket;
        public int onGridPositionX;
        public int onGridPositionY;

        //Rotate fields
        public bool Rotated { get; set; } = false;
        public Dir Dir { get; set; } = Dir.Down;
        public Vector2Int RotationOffset { get; set; }
        public List<Vector2Int> TetrisPieceShapePos { get; set; }

        public int WIDTH => Rotated ? ItemDetails.yHeight : ItemDetails.xWidth;
        public int HEIGHT => Rotated ? ItemDetails.xWidth : ItemDetails.yHeight;

        private void Awake()
        {
            InstanceIDManager.Register(this);
            image = GetComponent<Image>();
        }

        private void OnDestroy()
        {
            InstanceIDManager.Unregister(this);
            foreach (var component in _components)
                component.OnDetach();
        }

        public void Initialize(ItemDetails itemDetails, GridItem itemData, TetrisItemGrid grid)
        {
            image.alphaHitTestMinimumThreshold = 0.1f;
            image.raycastPadding = new Vector4(-1, -1, -1, -1);
            ItemDetails = itemDetails;
            InventorySlotType = itemDetails.inventorySlotType;
            CurrentInventoryContainer = grid;
            TetrisPieceShapePos = Utilities.RotationHelper.RotatePoints(InventoryManager.Instance.GetTetrisPieceShapePos(itemDetails.tetrisPieceShape), Dir);
            RotationOffset = RotationHelper.GetRotationOffset(Dir, WIDTH, HEIGHT);
            //Modify item size
            Vector2 size = new Vector2();
            size.x = itemDetails.xWidth * TetrisItemGrid.tileSizeWidth;
            size.y = itemDetails.yHeight * TetrisItemGrid.tileSizeHeight;
            GetComponent<RectTransform>().sizeDelta = size;
            //Initialize item components
            AddItemComponent<InformationlayoutComponent>().Initialize();
            if (itemDetails.maxStack > 0) AddItemComponent<StackableComponent>().Initialize(itemData);
            if (itemDetails.gridUIPrefab != null) AddItemComponent<GridPanelComponent>().Initialize();

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //Update only when not selected
            TetrisItemMediator.Instance.SyncGhostFromItem(this);
        }

        public T AddItemComponent<T>() where T : Component, IItemComponent
        {
            var component = gameObject.AddComponent<T>();
            component.OnAttach(this);
            _components.Add(component);
            return component;
        }

        public void RemoveItemComponent<T>() where T : IItemComponent
        {
            var component = GetComponent<T>();
            if (component != null)
            {
                component.OnDetach();
                _components.Remove(component);
                Destroy(component as Component);
            }
        }

        public bool TryGetItemComponent<T>(out T result) where T : IItemComponent
        {
            result = GetComponent<T>();
            return result != null;
        }

        public bool IsItemLocationOnGrid()
        {
            if (CurrentInventoryContainer is TetrisItemGrid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieve GridItem from the inventoryData_SO using the instance ID of TetrisItem and modify its data
        /// </summary>
        /// <param name="itemIndex"></param>
        public void SetItemData(int itemIndex)
        {
            GridItem gridItem = InventorySaveLoadService.Instance.inventoryData_SO.GetGridItem(itemIndex);
            if (IsItemLocationOnGrid())
            {
                if (gridItem != null)
                {
                    SetGridItemData(gridItem);
                }
                else
                {
                    GridItem item = new GridItem();
                    SetGridItemData(item);
                    InventorySaveLoadService.Instance.inventoryData_SO.inventoryItemList.Add(item);
                }
            }
            else
            {
                SetGridItemData(gridItem);
            }
        }

        public void RemoveItemData(int itemIndex)
        {
            GridItem gridItem = InventorySaveLoadService.Instance.inventoryData_SO.GetGridItem(itemIndex);
            if (gridItem != null)
            {
                InventorySaveLoadService.Instance.inventoryData_SO.inventoryItemList.Remove(gridItem);
            }
        }

        /// <summary>
        /// Assign the relevant fields of TetrisItem to gridItem
        /// </summary>
        /// <param name="gridItem"></param>
        private void SetGridItemData(GridItem gridItem)
        {
            gridItem.itemID = ItemDetails.itemID;
            gridItem.itemIndex = GetInstanceID();
            gridItem.parentItemIndex = transform.parent?.GetComponent<TetrisItemGrid>()?.RelatedTetrisItem?.GetInstanceID() ?? 0;
            gridItem.gridPIndex = transform?.parent.GetSiblingIndex() ?? -1;
            gridItem.isParent = transform.GetComponentInChildren<Transform>(true).CompareTag("GridPanel");
            gridItem.isOnSlot = !IsItemLocationOnGrid();
            gridItem.persistentGridTypeIndex = transform.parent?.GetComponent<TetrisItemGrid>()?.GetPersistentGridIdentification()?.GetPersistentGridTypeIndex() ?? 0;
            gridItem.orginPosition = new Vector2Int(onGridPositionX, onGridPositionY);
            gridItem.direction = Dir;
            if (TryGetItemComponent<StackableComponent>(out var stackableComponent))
            {
                gridItem.stack = stackableComponent.CurrentStack;
            }
        }

    }
}
