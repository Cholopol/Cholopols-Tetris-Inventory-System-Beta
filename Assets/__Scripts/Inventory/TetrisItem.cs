using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ChosTIS
{
    public class TetrisItem : MonoBehaviour, IPointerEnterHandler, ITetrisRotatable
    {
        //Private fields
        private Image image;
        private Transform gridPanel;

        //Public fields
        public ItemDetails ItemDetails { get; set; }
        public InventorySlot CurrentPlacedSlot { get; set; }
        public TetrisItemGrid CurrentPlacedGrid { get; set; }
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
            image = GetComponent<Image>();
        }

        public void Set(ItemDetails itemData)
        {
            image.alphaHitTestMinimumThreshold = 0.1f;
            image.raycastPadding = new Vector4(-10, -10, -10, -10);
            ItemDetails = itemData;
            InventorySlotType = itemData.inventorySlotType;
            CurrentPlacedGrid = InventoryManager.Instance.selectedTetrisItemGrid;
            TetrisPieceShapePos = InventoryManager.Instance.GetTetrisPieceShapePos(itemData.tetrisPieceShape);
            RotationOffset = Utilities.RotationHelper.GetRotationOffset(Dir, WIDTH, HEIGHT);
            //Modify item size
            Vector2 size = new Vector2();
            size.x = itemData.xWidth * TetrisItemGrid.tileSizeWidth;
            size.y = itemData.yHeight * TetrisItemGrid.tileSizeHeight;
            GetComponent<RectTransform>().sizeDelta = size;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //Update only when not selected
            TetrisItemMediator.Instance.SyncGhostFromItem(this);

        }

        public bool IsItemLocationOnGrid()
        {
            if (CurrentPlacedGrid != null && CurrentPlacedSlot == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetSlotGridPanel(InventorySlot slot)
        {
            if (gridPanel != null)
            {
                gridPanel.SetParent(slot.gridPanelParent);
                gridPanel.gameObject.SetActive(true);
            }
            else
            {
                gridPanel = Instantiate(ItemDetails.gridUIPrefab);
                gridPanel.SetParent(slot.gridPanelParent, false);
                gridPanel.gameObject.SetActive(true);
                gridPanel.SetAsFirstSibling();
            }
        }

        public void RemoveSlotGridPanel()
        {
            gridPanel.SetParent(transform);
            gridPanel.gameObject.SetActive(false);
        }
    }
}
