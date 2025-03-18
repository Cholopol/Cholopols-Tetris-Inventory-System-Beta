using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ChosTIS
{
    public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private TetrisItemGhost tetrisItemGhost;
        public TetrisItem PlacedTetrisItem { get; set; }
        public Transform gridPanelParent;
        [Header("≈‰÷√œÓ")]
        [SerializeField] private InventorySlotType inventorySlotType;
        [SerializeField] private Image activeUIImage;

        private void Start()
        {
            tetrisItemGhost = InventoryManager.Instance.tetrisItemGhost;
            gridPanelParent = transform.parent.Find("GridPanel");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tetrisItemGhost.CurrentSlot = this;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tetrisItemGhost.CurrentSlot = null;
        }

        public bool PlaceTetrisItem(TetrisItem tetrisItem)
        {
            if (tetrisItem.InventorySlotType == inventorySlotType)
            {
                PlacedTetrisItem = tetrisItem;
                tetrisItem.transform.SetParent(transform, false);
                transform.GetChild(0).GetComponent<Image>().enabled = false;
                tetrisItem.transform.SetPositionAndRotation(
                    transform.GetChild(0).position,
                        Quaternion.Euler(0, 0, 0));
                tetrisItem.GetComponent<RectTransform>().sizeDelta = transform.GetComponent<RectTransform>().sizeDelta;
                tetrisItem.SetSlotGridPanel(this);

                return true;
            }
            else
            {
                return false;
            }
        }

        public void PickUpItem()
        {
            PlacedTetrisItem.RemoveSlotGridPanel();
            PlacedTetrisItem = null;
            transform.GetChild(0).GetComponent<Image>().enabled = true;
        }

        public bool HasItem()
        {
            if (PlacedTetrisItem == null)
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