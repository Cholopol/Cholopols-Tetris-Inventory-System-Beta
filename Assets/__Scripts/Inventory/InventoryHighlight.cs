using ChosTIS.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChosTIS
{
    public class InventoryHighlight : MonoBehaviour
    {
        [SerializeField] RectTransform highlighter;
        [SerializeField] private GameObject highlightTilePrefab;
        private List<GameObject> activeTiles = new();

        public void UpdateShapeHighlight(TetrisItemGhost ghost, Vector2Int originPos, TetrisItemGrid selectedTetrisItemGrid)
        {
            ClearTiles();

            foreach (var point in ghost.TetrisPieceShapePos)
            {
                Vector2Int actualPos = originPos + point + ghost.RotationOffset;
                if (!selectedTetrisItemGrid.BoundryCheck(actualPos.x, actualPos.y, 1, 1))
                    continue;

                Vector2 tilePos = selectedTetrisItemGrid.CalculateTilePosition(ghost, point.x, point.y);
                GameObject tile = PoolManager.Instance.GetObject(highlightTilePrefab);
                SetColor(tile, selectedTetrisItemGrid, actualPos);
                tile.transform.SetParent(highlighter);
                tile.transform.localScale = Vector3.one;
                tile.transform.localPosition = tilePos;
                activeTiles.Add(tile);
            }
        }

        private void ClearTiles()
        {
            foreach (var tile in activeTiles)
            {
                PoolManager.Instance.PushObject(tile);
            }
            activeTiles.Clear();
        }

        public void SetSize(TetrisItem targetItem)
        {
            Vector2 size = new Vector2();
            size.x = targetItem.WIDTH * TetrisItemGrid.tileSizeWidth;
            size.y = targetItem.HEIGHT * TetrisItemGrid.tileSizeHeight;
            highlighter.sizeDelta = size;
        }

        public void Show(bool b)
        {
            highlighter.gameObject.SetActive(b);
        }

        public void SetParent(TetrisItemGrid targetGrid)
        {
            highlighter.SetParent(targetGrid.GetComponent<RectTransform>());
            highlighter.transform.SetAsFirstSibling();
        }

        public void SetPosition(TetrisItemGrid targetGrid, TetrisItem targetItem, int posX, int posY)
        {
            Vector2 pos = targetGrid.CalculateHighlighterPosition(posX, posY);
            highlighter.localPosition = pos;
        }

        private void SetColor(GameObject tile, TetrisItemGrid targetGrid, Vector2Int tileOnGridPos)
        {
            TetrisItem tetrisItem = targetGrid.GetTetrisItem(tileOnGridPos.x, tileOnGridPos.y);
            if (targetGrid.HasItem(tileOnGridPos.x, tileOnGridPos.y))
            {
                if (tetrisItem.ItemDetails.maxStack > 0
                    && TetrisItemMediator.Instance.GetTetrisItemGhost().selectedTetrisItem.ItemDetails.itemID == tetrisItem.ItemDetails.itemID)
                {
                    tile.GetComponent<Image>().color = new Color(1f, 1f, 0f, 100f / 255f);
                }
                else
                {
                    tile.GetComponent<Image>().color = new Color(1f, 0f, 0f, 100f / 255f);
                }
            }
            else
            {
                tile.GetComponent<Image>().color = new Color(0f, 1f, 0f, 100f / 255f);
            }
        }

    }
}