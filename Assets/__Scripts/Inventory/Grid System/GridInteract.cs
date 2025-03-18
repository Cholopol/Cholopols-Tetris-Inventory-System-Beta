using UnityEngine;
using UnityEngine.EventSystems;

namespace ChosTIS
{
    [RequireComponent(typeof(TetrisItemGrid))]
    public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private InventoryManager inventoryManager;
        [SerializeField] private TetrisItemGrid tetrisItemGrid;

        private void Awake()
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
            tetrisItemGrid = GetComponent<TetrisItemGrid>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            inventoryManager.selectedTetrisItemGrid = tetrisItemGrid;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryManager.selectedTetrisItemGrid = null;
        }
    }
}