using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static ChosTIS.Utilities;

namespace ChosTIS
{
    public class InventorySlot : MonoBehaviour, IInventoryFunctionalContainer, IPointerEnterHandler, IPointerExitHandler
    {
        private TetrisItemGhost tetrisItemGhost;
        public TetrisItem RelatedTetrisItem { get; set; }
        public Transform GridPanelParent { get; set; }
        [Header("Configuration")]
        [SerializeField] private InventorySlotType inventorySlotType;
        [SerializeField] private Image activeUIImage;

        private void Start()
        {
            InstanceIDManager.Register(this);

            tetrisItemGhost = TetrisItemMediator.Instance.GetTetrisItemGhost();
            GridPanelParent = transform.parent.Find("GridPanel");
        }

        private void OnDestroy()
        {
            InstanceIDManager.Unregister(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tetrisItemGhost.CurrentSlot = this;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tetrisItemGhost.CurrentSlot = null;
        }

        public bool TryPlaceTetrisItem(TetrisItem tetrisItem)
        {
            if (tetrisItem.InventorySlotType == inventorySlotType)
            {
                PlaceTetrisItem(tetrisItem);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void PlaceTetrisItem(TetrisItem tetrisItem)
        {
            RelatedTetrisItem = tetrisItem;
            tetrisItem.transform.SetParent(transform, false);
            transform.GetChild(0).GetComponent<Image>().enabled = false;
            tetrisItem.transform.SetPositionAndRotation(
                transform.GetChild(0).position,
                    Quaternion.Euler(0, 0, 0));
            tetrisItem.GetComponent<RectTransform>().sizeDelta = transform.GetComponent<RectTransform>().sizeDelta;
            tetrisItem.TryGetItemComponent<GridPanelComponent>(out var gridPanelComponent);
            gridPanelComponent.SetGridPanelParent(GridPanelParent);
        }

        public void RemoveTetrisItem()
        {
            RelatedTetrisItem.TryGetItemComponent<GridPanelComponent>(out var gridPanelComponent);
            gridPanelComponent.SetGridPanelParent(RelatedTetrisItem.transform);
            RelatedTetrisItem = null;
            transform.GetChild(0).GetComponent<Image>().enabled = true;
        }

        public bool HasItem()
        {
            if (RelatedTetrisItem == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public RectTransform GetSlotRectTransform()
        {
            return GetComponent<RectTransform>();
        }

        public InventorySlotType GetInventorySlotType()
        {
            return inventorySlotType;
        }
    }
}