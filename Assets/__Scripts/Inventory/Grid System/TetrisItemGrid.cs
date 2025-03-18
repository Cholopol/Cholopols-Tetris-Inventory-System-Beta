using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChosTIS
{
    public class TetrisItemGrid : MonoBehaviour
    {
        private TetrisItem[,] itemSlot;

        [Header("网格单元大小")]
        public const float tileSizeWidth = 20;
        public const float tileSizeHeight = 20;
        [Header("背包网格大小")]
        public int gridSizeWidth = 1;
        public int gridSizeHeight = 1;
        [Header("相关组件")]
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Canvas canvas;
        [SerializeField] private GridInteract gridInteract;

        // Calculate the position in the Grid panel
        private Vector2 positionOnTheGrid = new Vector2();
        // Calculate the coordinates in the Grid panel
        private Vector2Int tileGridPosition = new Vector2Int();

        private void Start()
        {
            itemSlot = new TetrisItem[gridSizeWidth, gridSizeHeight];

            rectTransform = GetComponent<RectTransform>();
            gridInteract = GetComponent<GridInteract>();
            canvas = FindObjectOfType<Canvas>();

            Init(gridSizeWidth, gridSizeHeight);


        }

        //Resize Rect
        public void Init(int width, int height)
        {
            Vector2 size = new Vector2(width * tileSizeWidth, height * tileSizeHeight);
            rectTransform.sizeDelta = size;
        }

        /// <summary>
        /// Add items at grid coordinates
        /// </summary>
        /// <param name="item"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="overlapItem"></param>
        /// <returns></returns>
        public bool PlaceTetrisItem(TetrisItem item, int posX, int posY, ref TetrisItem overlapItem)
        {
            //Determine if the item is out of bounds
            if (BoundryCheck(posX, posY, item.WIDTH, item.HEIGHT) == false) return false;
            //Check for overlapping items in the specified location and range. Multiple overlapping items have exited
            if (OverlapCheck(item, posX, posY, ref overlapItem) == false) return false;
            PlaceTetrisItem(item, posX, posY);

            return true;
        }

        //Add items according to grid coordinates
        public void PlaceTetrisItem(TetrisItem item, int posX, int posY)
        {
            item.transform.SetParent(transform, false);
            if (item.WIDTH == item.HEIGHT)
            {
                item.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    item.WIDTH * tileSizeWidth, item.HEIGHT * tileSizeHeight);
            }
            List<Vector2Int> tetrisPieceShapePos = item.TetrisPieceShapePos;

            item.transform.rotation = Quaternion.Euler(0, 0, -Utilities.RotationHelper.GetRotationAngle(item.Dir));

            //Occupy the corresponding size of the grid according to the size of the item
            foreach (Vector2Int v2i in tetrisPieceShapePos)
            {

                itemSlot[posX + v2i.x + item.RotationOffset.x, posY + v2i.y + item.RotationOffset.y] = item;
            }

            item.onGridPositionX = posX;
            item.onGridPositionY = posY;
            item.transform.localPosition = CalculatePositionOnGrid(item, posX, posY);
        }

        //Get items by grid coordinates
        public TetrisItem PickUpItem(int x, int y, Vector2Int oldRotationOffset, List<Vector2Int> tetrisItemGrids)
        {
            TetrisItem toReturn = itemSlot[x + tetrisItemGrids[0].x + oldRotationOffset.x,
                y + tetrisItemGrids[0].y + oldRotationOffset.y];
            if (toReturn == null) return null;
            CleanGridReference(x, y, oldRotationOffset, tetrisItemGrids);
            return toReturn;
        }

        //Unoccupy the corresponding size of the grid by item size
        void CleanGridReference(int startColumn, int startRow, Vector2Int oldRotationOffset, List<Vector2Int> tetrisItemGrids)
        {
            //Check starting coordinates and item dimensions for out-of-bounds
            if (startRow < 0 || startRow >= gridSizeHeight ||
                startColumn < 0 || startColumn >= gridSizeWidth)
            {
                Debug.LogError($"起始坐标 ({startRow}, {startColumn}) 无效");
                return;
            }

            foreach (Vector2Int v2i in tetrisItemGrids)
            {
                itemSlot[startColumn + v2i.x + oldRotationOffset.x, startRow + v2i.y + oldRotationOffset.y] = null;
            }
        }

        //Checks for overlapping items in the specified location and range and returns overlapItem and false if there are multiple overlapping items
        public bool OverlapCheck(TetrisItem item, int posX, int posY, ref TetrisItem overlapItem)
        {
            foreach (Vector2Int v2i in item.TetrisPieceShapePos)
            {
                //If there are items in the current location
                if (itemSlot[posX + v2i.x + item.RotationOffset.x, posY + v2i.y + item.RotationOffset.y] != null)
                {
                    //If the overlapItem has not yet been assigned a value (the first time an overlapping item is found)
                    if (overlapItem == null)
                    {
                        overlapItem = itemSlot[posX + v2i.x + item.RotationOffset.x, posY + v2i.y + item.RotationOffset.y];
                    }
                    else
                    {
                        //If you find multiple overlapping items in the range
                        if (overlapItem != itemSlot[posX + v2i.x + item.RotationOffset.x, posY + v2i.y + item.RotationOffset.y])
                        {
                            overlapItem = null;
                            return false;
                        }
                    }
                }

            }

            //Return true if all the locations being checked have the same overlapping items
            return true;
        }

        /// <summary>
        ///Calculate the coordinates of position in the current grid
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector2Int GetTileGridPosition(Vector2 position)
        {
            //Calculates the offset of the mouse position with respect to the RectTransform
            positionOnTheGrid.x = position.x - rectTransform.position.x;
            positionOnTheGrid.y = rectTransform.position.y - position.y;

            //Converts the offset to the grid position
            tileGridPosition.x = (int)(positionOnTheGrid.x / tileSizeWidth / canvas.scaleFactor);
            tileGridPosition.y = (int)(positionOnTheGrid.y / tileSizeHeight / canvas.scaleFactor);

            return tileGridPosition;
        }


        /// <summary>
        /// Determine if the item is out of bounds
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public bool BoundryCheck(int posX, int posY, int width, int height)
        {
            if (PositionCheck(posX, posY) == false) return false;//Top left corner
            posX += width - 1;
            posY += height - 1;
            if (PositionCheck(posX, posY) == false) return false;//Lower right corner
            return true;
        }

        //Determine whether the grid coordinates are out of line
        bool PositionCheck(int posX, int posY)
        {
            if (posX < 0 || posY < 0) return false;
            if (posX >= gridSizeWidth || posY >= gridSizeHeight) return false;
            return true;
        }

        public Vector2 CalculateHighlighterPosition(int gridX, int gridY)
        {
            return new Vector2(
                gridX * tileSizeWidth,
                -gridY * tileSizeHeight
            );
        }
        public Vector2 CalculateTilePosition(TetrisItemGhost ghost, int gridX, int gridY)
        {
            return new Vector2(
                gridX * tileSizeWidth + ghost.RotationOffset.x * tileSizeWidth,
                -gridY * tileSizeHeight - ghost.RotationOffset.y * tileSizeWidth
            );
        }

        /// <summary>
        /// The grid coordinates are calculated as the UI pivot position
        /// </summary>
        /// <param name="item"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <returns></returns>
        public Vector2 CalculatePositionOnGrid(TetrisItem item, int posX, int posY)
        {
            return new Vector2(
                posX * tileSizeWidth + tileSizeWidth * item.WIDTH / 2,
                -(posY * tileSizeHeight + tileSizeHeight * item.HEIGHT / 2)
            );
        }

        //Get items by grid coordinates
        public TetrisItem GetTetrisItem(int x, int y)
        {
            return itemSlot[x, y];
        }

        public bool HasItem(int x, int y)
        {
            if (itemSlot[x, y] == null) return false;
            return true;
        }
    }
}