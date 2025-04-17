using System.Collections.Generic;
using UnityEngine;

namespace ChosTIS
{
    public interface IInventoryContainer
    {
        public TetrisItem RelatedTetrisItem { get; set; }

        public bool TryPlaceTetrisItem(TetrisItem tetrisItem, int posX, int posY)
        {
            return false;
        }

        public void PlaceTetrisItem(TetrisItem tetrisItem, int posX, int posY)
        {

        }

        public void RemoveTetrisItem(TetrisItem toReturn, int x, int y, Vector2Int oldRotationOffset, List<Vector2Int> tetrisPieceShapePositions)
        {

        }

    }
}

