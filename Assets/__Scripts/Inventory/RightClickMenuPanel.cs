using UnityEngine;
using UnityEngine.UI;

namespace ChosTIS
{
    public class RightClickMenuPanel : MonoBehaviour
    {
        public TetrisItem _currentItem;
        public RectTransform menuRect;
        [SerializeField] private ItemInformationPanel ItemInformationPanel;
        [SerializeField] private Button CheckBtn, SplitBtn, UseBtn;
        [SerializeField] private float distanceThreshold = 150f;


        private void Awake()
        {
            CheckBtn.onClick.AddListener(OnCheckBtnClick);
            SplitBtn.onClick.AddListener(OnSplitBtnClick);
            UseBtn.onClick.AddListener(OnUseBtnClick);
        }

        private void Update()
        {
            if (Vector2.Distance(Input.mousePosition, transform.position) > distanceThreshold)
            {
                Show(false);
            }
        }

        private void OnCheckBtnClick()
        {
            Show(false);
            ItemInformationPanel.Show(true);
            ItemDetails itemDetails = _currentItem.ItemDetails;
            ItemInformationPanel.SetItemImage(newSprite: itemDetails?.itemUI ?? ItemInformationPanel.itemDefaultSprite);
            ItemInformationPanel.ItemName.text = itemDetails.itemName;
            ItemInformationPanel.ItemDescription.text = itemDetails.itemDescription;
        }

        private void OnSplitBtnClick()
        {
            if (_currentItem == null) return;
            _currentItem.TryGetItemComponent<StackableComponent>(out var stackableComponent);
            bool canSplit = stackableComponent.TrySplitStack(stackableComponent.CurrentStack / 2);
            if (canSplit)
            {
                _currentItem.SetItemData(_currentItem.GetInstanceID());
            }
            Show(false);
        }

        private void OnUseBtnClick()
        {
            Show(false);
            Debug.Log("UseBtn Clicked");
            // Implement your use logic here
        }

        public void Show(bool isClick)
        {
            gameObject.SetActive(isClick);
        }

        public void SetBtnActive()
        {
            if (_currentItem == null) return;
            _currentItem.TryGetItemComponent<StackableComponent>(out var stackableComponent);
            if (stackableComponent != null)
            {
                CheckBtn.gameObject.SetActive(true);
                SplitBtn.gameObject.SetActive(true);
                UseBtn.gameObject.SetActive(true);
            }
            else
            {
                CheckBtn.gameObject.SetActive(true);
                SplitBtn.gameObject.SetActive(false);
                UseBtn.gameObject.SetActive(true);
            }
        }

    }
}