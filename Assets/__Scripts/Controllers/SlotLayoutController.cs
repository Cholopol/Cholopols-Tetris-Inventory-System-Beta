using UnityEngine;

namespace ChosTIS
{
    public class SlotLayoutController : MonoBehaviour
    {
        public RectTransform slotRect;
        public RectTransform gridPanelRect;
        private int childCount = 0;
        private bool isRemoved = false;

        private void Update()
        {
            if (gridPanelRect.childCount < childCount) isRemoved = true;

            if (HasGrid(gridPanelRect) && !isRemoved)
            {
                SetUp();
            }

            if (isRemoved)
            {
                gridPanelRect.sizeDelta = new Vector2(
                    gridPanelRect.rect.width,
                    50f);
                slotRect.sizeDelta = new Vector2(slotRect.rect.width, gridPanelRect.rect.height + 10f);
                isRemoved = false;
            }
        }

        public void SetUp()
        {
            gridPanelRect.sizeDelta = new Vector2(
                gridPanelRect.rect.width,
                gridPanelRect.GetChild(0).GetComponent<RectTransform>().rect.height);
            slotRect.sizeDelta = new Vector2(slotRect.rect.width, gridPanelRect.rect.height + 10f);
        }

        private bool HasGrid(RectTransform gridPanel)
        {
            //if (gridPanel.childCount == 0) return false;
            if (gridPanel.childCount != childCount)
            {
                childCount = gridPanel.childCount;
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}