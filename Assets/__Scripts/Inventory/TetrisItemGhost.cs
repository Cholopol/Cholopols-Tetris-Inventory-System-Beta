using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ChosTIS
{
    public class TetrisItemGhost : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, ITetrisRotatable
    {
        //Private fields
        public PlaceState PlaceState { get; set; } = PlaceState.InvalidPos;
        public bool OnDragging { get; set; } = false;
        public TetrisItemGrid selectedTetrisItemOrginGrid;
        public TetrisItem selectedTetrisItem;
        public RectTransform ghostRect;
        public Image ghostImage;
        private CanvasGroup canvasGroup;
        private Vector2Int oldPosition;

        //Public fields
        public ItemDetails ItemDetails { get; set; }
        public int onGridPositionX;
        public int onGridPositionY;

        //Rotate fields
        public bool Rotated { get; set; } = false;
        public Dir Dir { get; set; } = Dir.Down;
        public Vector2Int RotationOffset { get; set; }

        //public TetrisPieceShape tetrisPieceShape;
        public List<Vector2Int> TetrisPieceShapePos { get; set; }

        public int WIDTH => Rotated ? ItemDetails.yHeight : ItemDetails.xWidth;
        public int HEIGHT => Rotated ? ItemDetails.xWidth : ItemDetails.yHeight;

        public InventorySlot CurrentSlot { get; set; }

        private void Awake()
        {
            ghostRect = GetComponent<RectTransform>();
            ghostImage = GetComponent<Image>();
            canvasGroup = GetComponent<CanvasGroup>();
            ghostImage.alphaHitTestMinimumThreshold = 0.1f;
        }

        void Update()
        {
            if (InventoryManager.Instance.selectedTetrisItemGrid != null)
            {
                Vector2Int positionOnGrid = InventoryManager.Instance.GetGhostTileGridOriginPosition();
                if (oldPosition == positionOnGrid) return;
                oldPosition = positionOnGrid;

                onGridPositionX = positionOnGrid.x;
                onGridPositionY = positionOnGrid.y;
            }

        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = false;
            OnDragging = true;
            ghostImage.color = new Color(1, 1, 1, 0.8f);
            //Cache source status
            TetrisItemMediator.Instance.CacheItemState(selectedTetrisItem);
            TetrisItemMediator.Instance.CacheGhostState(this);
            OnBeginAction(eventData);

        }

        public void OnDrag(PointerEventData eventData)
        {
            ghostRect.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true;
            OnDragging = false;
            ghostImage.color = new Color(1, 1, 1, 0);
            OnEndAction(eventData);
            UpdatePlaceState();
        }

        public void Rotate()
        {
            Dir = Utilities.RotationHelper.GetNextDir(Dir);
            Rotated = !Rotated;
            RotationOffset = Utilities.RotationHelper.GetRotationOffset(Dir, WIDTH, HEIGHT);
            TetrisPieceShapePos = Utilities.RotationHelper.RotatePointsClockwise(TetrisPieceShapePos);
            TetrisItemMediator.Instance.CacheGhostState(this);
            transform.rotation = Quaternion.Euler(0, 0, -Utilities.RotationHelper.GetRotationAngle(Dir));
        }

        private void OnBeginAction(PointerEventData eventData)
        {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            foreach (var result in results)
            {
                GameObject target = result.gameObject;
                if (target.CompareTag("TetrisGrid"))
                {
                    target.GetComponent<TetrisItemGrid>().PickUpItem(
                    selectedTetrisItem.onGridPositionX,
                    selectedTetrisItem.onGridPositionY,
                    selectedTetrisItem.RotationOffset,
                    selectedTetrisItem.TetrisPieceShapePos);
                }
                else if (target.CompareTag("TetrisSlot"))
                {
                    target.GetComponent<InventorySlot>().PickUpItem();
                }
            }
        }

        private void OnEndAction(PointerEventData eventData)
        {
            // Radiographic inspection of all UI objects
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (var result in results)
            {
                GameObject target = result.gameObject;

                // Skip oneself
                if (target == gameObject) continue;

                // Determine the target type
                if (target.CompareTag("TetrisGrid"))
                {
                    if (!target.GetComponent<TetrisItemGrid>().BoundryCheck(
                           onGridPositionX, onGridPositionY,
                           WIDTH, HEIGHT))
                    {
                        PlaceState = PlaceState.InvalidPos;
                        return;
                    }

                    foreach (Vector2Int v2i in TetrisPieceShapePos)
                    {
                        PlaceState = target.GetComponent<TetrisItemGrid>().HasItem(
                            onGridPositionX + v2i.x + RotationOffset.x,
                            onGridPositionY + v2i.y + RotationOffset.y) ?
                        PlaceState.OnGridHasItem : PlaceState.OnGridNoItem;
                        //Debug.Log(PlaceState);
                        if (PlaceState == PlaceState.OnGridHasItem)
                        {
                            return;
                        }
                    }
                    return;
                }
                else if (target.CompareTag("TetrisSlot"))
                {

                    PlaceState = target.GetComponent<InventorySlot>().HasItem() ?
                        PlaceState.OnSlotHasItem : PlaceState.OnSlotNoItem;
                    return;
                }

                // No valid area detected
                PlaceState = PlaceState.InvalidPos;
            }
        }

        private void UpdatePlaceState()
        {
            switch (PlaceState)
            {
                case PlaceState.OnGridHasItem:
                    // return-to-home position
                    ResetState();
                    break;
                case PlaceState.OnSlotHasItem:
                    // return-to-home position
                    ResetState();
                    break;
                case PlaceState.OnGridNoItem:
                    PlaceOnGrid();
                    // Adsorb to the Grid position
                    break;
                case PlaceState.OnSlotNoItem:
                    PlaceOnSlot();
                    // Adsorb to the Slot position
                    break;
                case PlaceState.InvalidPos:
                    // return-to-home position
                    ResetState();
                    break;
            }
        }

        private void PlaceOnGrid()
        {
            selectedTetrisItem.CurrentPlacedGrid = InventoryManager.Instance.selectedTetrisItemGrid;
            selectedTetrisItem.CurrentPlacedSlot = null;
            TetrisItemMediator.Instance.ApplyStateToItem(selectedTetrisItem);
            InventoryManager.Instance.PlaceGhostItem(
                InventoryManager.Instance.GetGhostTileGridOriginPosition(),
                 selectedTetrisItem.CurrentPlacedGrid);
            transform.position = selectedTetrisItem.transform.position;
        }

        private void PlaceOnSlot()
        {
            TetrisItemMediator.Instance.ApplyStateToItem(selectedTetrisItem);
            bool canPlace = CurrentSlot.PlaceTetrisItem(selectedTetrisItem);
            if (canPlace)
            {
                selectedTetrisItem.CurrentPlacedGrid = null;
                selectedTetrisItem.CurrentPlacedSlot = CurrentSlot;
                transform.position = CurrentSlot.transform.position;
            }
            else
            {
                ResetState();
            }
        }

        public void ResetState()
        {
            TetrisItemMediator.Instance.ApplyStateToGhost(this);
            if (selectedTetrisItem.IsItemLocationOnGrid())
            {
                InventoryManager.Instance.PlaceGhostItem(new Vector2Int(
                    selectedTetrisItem.onGridPositionX,
                    selectedTetrisItem.onGridPositionY),
                    selectedTetrisItem.CurrentPlacedGrid);
            }
            else
            {
                selectedTetrisItem.CurrentPlacedSlot.PlaceTetrisItem(selectedTetrisItem);
            }
            transform.position = selectedTetrisItem.transform.position;

        }

        public void InitializeFromItem(TetrisItem item)
        {
            if (item == null) return;
            if (!OnDragging)
            {
                // Synchronize basic attributes
                transform.SetPositionAndRotation(
                    item.transform.position,
                    Quaternion.Euler(0, 0, -Utilities.RotationHelper.GetRotationAngle(item.Dir))
                );
                ghostImage.sprite = item.GetComponent<Image>().sprite;
                ghostRect.sizeDelta = item.GetComponent<RectTransform>().sizeDelta;

                // Synchronize business logic attributes
                selectedTetrisItemOrginGrid = item.CurrentPlacedGrid;
                selectedTetrisItem = item;
                ItemDetails = item.ItemDetails;
                Rotated = item.Rotated;
                Dir = item.Dir;
                RotationOffset = item.RotationOffset;
                onGridPositionX = item.onGridPositionX;
                onGridPositionY = item.onGridPositionY;
                TetrisPieceShapePos = item.TetrisPieceShapePos;
            }
        }

    }
}