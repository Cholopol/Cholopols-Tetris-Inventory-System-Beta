using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChosTIS
{
    public class ItemInformationPanel : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        public Image itemImage;
        public Sprite itemDefaultSprite;
        public Text ItemName;
        public Text ItemDescription;
        [SerializeField] private Button CloseBtn;
        [SerializeField] private RectTransform panelRect;
        [SerializeField] private RectTransform itemImageParentRect;
        [SerializeField] private RectTransform itemImageRect;
        [SerializeField] private Canvas canvas;
        private Vector2 offset;

        private void Awake()
        {
            CloseBtn.onClick.AddListener(OnCloseBtnClick);
        }

        private void OnCloseBtnClick()
        {
            Show(false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out Vector2 localPoint);
            offset = panelRect.anchoredPosition - localPoint;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out Vector2 localPoint))
            {
                panelRect.anchoredPosition = localPoint + offset;
            }
            ClampToCanvas();
        }


        public void Show(bool isShow)
        {
            gameObject.SetActive(isShow);
        }

        public void SetItemImage(Sprite newSprite)
        {
            itemImage.sprite = newSprite;
            itemImage.SetNativeSize();

            Vector2 maxSize = new Vector2(
                itemImageParentRect.rect.width,
                itemImageParentRect.rect.height
            );

            if(itemImageRect.sizeDelta.x > maxSize.x || itemImageRect.sizeDelta.y > maxSize.y)
            {
                float widthRatio = maxSize.x / itemImageRect.sizeDelta.x;
                float heightRatio = maxSize.y / itemImageRect.sizeDelta.y;
                float scaleFactor = Mathf.Min(widthRatio, heightRatio);

                itemImageRect.sizeDelta *= scaleFactor;
            }

        }
        private void ClampToCanvas()
        {
            Vector3 pos = panelRect.localPosition;
            Vector2 canvasSize = canvas.GetComponent<RectTransform>().rect.size;
            Vector2 panelSize = panelRect.rect.size;

            pos.x = Mathf.Clamp(pos.x,
                -canvasSize.x / 2 + panelSize.x / 2,
                canvasSize.x / 2 - panelSize.x / 2);
            pos.y = Mathf.Clamp(pos.y,
                -canvasSize.y / 2 + panelSize.y / 2,
                canvasSize.y / 2 - panelSize.y / 2);

            panelRect.localPosition = pos;
        }

    }
}