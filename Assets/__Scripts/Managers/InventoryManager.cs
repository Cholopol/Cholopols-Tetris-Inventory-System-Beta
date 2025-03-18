using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChosTIS
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [SerializeField] private bool onDebugMode = false;
        [Header("当前背包")]
        public TetrisItemGrid selectedTetrisItemGrid;
        [Header("物品数据")]
        public ItemDataList_SO itemDataList_SO;
        [Header("Tetris点集数据")]
        public TetrisItemPointSet_SO tetrisItemPointSet_SO;
        [Header("组件配置")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private InventoryHighlight inventoryHighlight;
        public TetrisItemGhost tetrisItemGhost;
        public TetrisItem selectedTetrisItem;
        public Vector2Int tileGridOriginPosition;

        private TetrisItem overlapItem;
        private Vector2Int oldPosition;
        private int selectedItemIndex;

        private void Update()
        {
            //[Debug] Dynamically add items randomly
            if (Input.GetKeyDown(KeyCode.Q))
            {
                onDebugMode = true;
                if (selectedItemIndex >= itemDataList_SO.itemDetailsList.Count)
                {
                    selectedItemIndex = 0;
                }
                if (selectedTetrisItem != null)
                {
                    Destroy(selectedTetrisItem.gameObject);
                    selectedTetrisItem = null;
                }
                CreateRandomItem(selectedItemIndex);
                ++selectedItemIndex;
            }

            //Rotating item
            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateItemGhost();
            }
            //Pick up item UI location
            if (selectedTetrisItem && onDebugMode) selectedTetrisItem.transform.position = Input.mousePosition;
            if (selectedTetrisItemGrid == null)
            {
                inventoryHighlight.Show(false);
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                //Gets the grid coordinates of the current mouse position in the grid and prints it to the console
                Vector2Int tileGridOriginPosition = GetTileGridOriginPosition();
                if (selectedTetrisItem == null && !selectedTetrisItemGrid.HasItem(tileGridOriginPosition.x, tileGridOriginPosition.y)) return;
                if (selectedTetrisItem != null) PlaceItem(tileGridOriginPosition);
                selectedItemIndex = 0;

            }

            HandleHighlight();
        }

        /// <summary>
        /// Gets Tetris's point coordinates
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public List<Vector2Int> GetTetrisPieceShapePos(TetrisPieceShape shape)
        {
            return tetrisItemPointSet_SO.TetrisPieceShapeList[(int)shape].points;
        }

        private void RotateItemGhost()
        {
            if (tetrisItemGhost.ItemDetails == null) return;
            tetrisItemGhost.Rotate();
        }

        /// <summary>
        /// Gets the origin grid coordinates of TetrisItemGhost, and returns the mouse location grid coordinates if the item is not picked up
        /// </summary>
        /// <returns></returns>
        public Vector2Int GetGhostTileGridOriginPosition()
        {
            if (selectedTetrisItemGrid == null) return new Vector2Int();
            Vector2 origin = Input.mousePosition;
            if (tetrisItemGhost.ItemDetails != null)
            {
                //计算物品原点位置
                origin.x -= (tetrisItemGhost.WIDTH - 1) * TetrisItemGrid.tileSizeWidth / 2;
                origin.y += (tetrisItemGhost.HEIGHT - 1) * TetrisItemGrid.tileSizeHeight / 2;
            }
            Vector2Int tileGridPosition = selectedTetrisItemGrid.GetTileGridPosition(origin);
            //Debug.Log(tileGridPosition);
            return tileGridPosition;
        }

        /// <summary>
        /// [Debug]Gets the origin grid coordinates of the TetrisItem and returns the mouse location grid coordinates if the item is not picked up
        /// </summary>
        /// <returns></returns>
        private Vector2Int GetTileGridOriginPosition()
        {
            Vector2 origin = Input.mousePosition;
            if (selectedTetrisItem != null)
            {
                //计算物品原点位置
                origin.x -= (selectedTetrisItem.WIDTH - 1) * TetrisItemGrid.tileSizeWidth / 2;
                origin.y += (selectedTetrisItem.HEIGHT - 1) * TetrisItemGrid.tileSizeHeight / 2;
            }
            Vector2Int tileGridPosition = selectedTetrisItemGrid.GetTileGridPosition(origin);
            return tileGridPosition;
        }

        public void HandleHighlight()
        {
            Vector2Int positionOnGrid = GetGhostTileGridOriginPosition();
            if (oldPosition == positionOnGrid) return;
            oldPosition = positionOnGrid;
            if (tetrisItemGhost.OnDragging)
            {
                inventoryHighlight.Show(true);
                inventoryHighlight.UpdateShapeHighlight(tetrisItemGhost, positionOnGrid, selectedTetrisItemGrid);
                inventoryHighlight.SetParent(selectedTetrisItemGrid);
                inventoryHighlight.SetPosition(selectedTetrisItemGrid,
                tetrisItemGhost.selectedTetrisItem,
                positionOnGrid.x, positionOnGrid.y);
            }
            else
            {
                inventoryHighlight.Show(false);
            }
        }

        /// <summary>
        /// Place Tetris Item
        /// </summary>
        /// <param name="tileGridOriginPosition"></param>
        public void PlaceGhostItem(Vector2Int tileGridOriginPosition, TetrisItemGrid targetGrid)
        {
            if (targetGrid == null) return;
            bool isDone = targetGrid.PlaceTetrisItem(tetrisItemGhost.selectedTetrisItem, tileGridOriginPosition.x, tileGridOriginPosition.y, ref overlapItem);
            //if (isDone)
            //{
            //    if (overlapItem != null)
            //    {

            //    }
            //}
            //else
            //{

            //}
        }

        //[Debug Mode]Moving items
        public void PlaceItem(Vector2Int tileGridOriginPosition)
        {
            if (selectedTetrisItemGrid == null) return;
            bool isDone = selectedTetrisItemGrid.PlaceTetrisItem(selectedTetrisItem, tileGridOriginPosition.x, tileGridOriginPosition.y, ref overlapItem);

            if (isDone)
            {
                selectedTetrisItem.GetComponent<Image>().raycastTarget = true;
                selectedTetrisItem = null;
                if (overlapItem != null)
                {
                    selectedTetrisItem = overlapItem;
                    overlapItem = null;
                    selectedTetrisItem.transform.SetAsLastSibling();
                }
            }
            onDebugMode = false;
        }

        //Add random items
        private void CreateRandomItem(int selectedItemIndex)
        {
            if (selectedTetrisItem) return;

            selectedTetrisItem = Instantiate(itemDataList_SO.GetItemDetailsByIndex(selectedItemIndex).uiPrefab).GetComponent<TetrisItem>();
            selectedTetrisItem.transform.SetParent(canvas.transform, false);
            selectedTetrisItem.transform.SetAsLastSibling();
            selectedTetrisItem.Set(itemDataList_SO.itemDetailsList[selectedItemIndex]);
        }

        public int GetTetrisShapeIndex(TetrisPieceShape shape)
        {
            return shape switch
            {
                TetrisPieceShape.Frame => 0,
                TetrisPieceShape.Domino => 1,
                TetrisPieceShape.Tromino_I => 2,
                TetrisPieceShape.Tromino_L => 3,
                TetrisPieceShape.Tromino_J => 4,
                TetrisPieceShape.Tetromino_I => 5,
                TetrisPieceShape.Tetromino_O => 6,
                TetrisPieceShape.Tetromino_T => 7,
                TetrisPieceShape.Tetromino_J => 8,
                TetrisPieceShape.Tetromino_L => 9,
                TetrisPieceShape.Tetromino_S => 10,
                TetrisPieceShape.Tetromino_Z => 11,
                TetrisPieceShape.Pentomino_I => 12,
                TetrisPieceShape.Pentomino_L => 13,
                TetrisPieceShape.Pentomino_J => 14,
                TetrisPieceShape.Pentomino_U => 15,
                TetrisPieceShape.Pentomino_T => 16,
                TetrisPieceShape.Pentomino_P => 17,
                _ => throw new System.ArgumentOutOfRangeException(nameof(shape), shape, null),
            };
        }

    }
}