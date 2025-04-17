using System.Collections.Generic;
using UnityEngine;

namespace ChosTIS
{
    public class GridPanelComponent : MonoBehaviour, IItemComponent, IGridPanel
    {
        public Transform GridPanel { get; set; }
        public List<TetrisItemGrid> TetrisItemGrids { get; set; } = new();
        private TetrisItem _owner;

        public void OnAttach(TetrisItem item) => _owner = item;
        public void OnDetach() => Destroy(GridPanel?.gameObject);

        public void Initialize()
        {
            if (_owner.ItemDetails.gridUIPrefab == null) return;

            GridPanel = Instantiate(_owner.ItemDetails.gridUIPrefab);
            GridPanel.SetParent(_owner.transform, false);
            GridPanel.GetComponent<RectTransform>().localPosition = new Vector3(60, 0, 0);
            GridPanel.gameObject.SetActive(false);

            TetrisItemGrids.AddRange(GridPanel.GetComponentsInChildren<TetrisItemGrid>(true));
            foreach (var subGrid in TetrisItemGrids)
                subGrid.RelatedTetrisItem = _owner;
        }

        public void SetGridPanelParent(Transform parent)
        {
            if (GridPanel == null) return;
            GridPanel.SetParent(parent, false);
            GridPanel.GetComponent<RectTransform>().localPosition = new Vector3(60, 0, 0);
            GridPanel.gameObject.SetActive(parent != _owner.transform);
        }

    }
}