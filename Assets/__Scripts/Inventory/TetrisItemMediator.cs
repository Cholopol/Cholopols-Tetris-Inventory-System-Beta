using System.Collections.Generic;
using UnityEngine;

namespace ChosTIS
{
    public class TetrisItemMediator : Singleton<TetrisItemMediator>
    {
        [SerializeField] TetrisItemGhost tetrisItemGhost;

        private Dir _cachedDir;
        private bool _cachedRotated;
        private Vector2Int _cachedRotationOffset;
        private List<Vector2Int> _cachedShapePos;

        private TetrisItem _cachedOrginItem;
        private TetrisItemGrid _cachedOrginGrid;
        private Dir _cachedItemDir;
        private bool _cachedItemRotated;
        private Vector2Int _cachedItemRotationOffset;
        private List<Vector2Int> _cachedItemShapePos;

        // Cache the rotation state of ghost
        public void CacheGhostState(TetrisItemGhost ghost)
        {
            _cachedDir = ghost.Dir;
            _cachedRotated = ghost.Rotated;
            _cachedRotationOffset = ghost.RotationOffset;
            _cachedShapePos = ghost.TetrisPieceShapePos;
        }

        // Cache the rotation state of the item
        public void CacheItemState(TetrisItem item)
        {
            _cachedOrginItem = item;
            _cachedOrginGrid = item.CurrentInventoryContainer as TetrisItemGrid;
            _cachedItemDir = item.Dir;
            _cachedItemRotated = item.Rotated;
            _cachedItemRotationOffset = item.RotationOffset;
            _cachedItemShapePos = item.TetrisPieceShapePos;
        }

        // Synchronize cache status to TetrisItemGhost
        public void ApplyStateToGhost(TetrisItemGhost ghost)
        {
            ghost.Dir = _cachedItemDir;
            ghost.Rotated = _cachedItemRotated;
            ghost.RotationOffset = _cachedItemRotationOffset;
            ghost.TetrisPieceShapePos = _cachedItemShapePos;
            ghost.selectedTetrisItem = _cachedOrginItem;
            ghost.selectedTetrisItemOrginGrid = _cachedOrginGrid;

        }

        // Synchronize cache status to TetrisItem
        public void ApplyStateToItem(TetrisItem item)
        {
            item.Dir = _cachedDir;
            item.Rotated = _cachedRotated;
            item.RotationOffset = _cachedRotationOffset;
            item.TetrisPieceShapePos = _cachedShapePos;

        }

        public void ApplyStateToItem(TetrisItem item, Dir dir, bool rotated, Vector2Int rotationOffset, List<Vector2Int> tetrisPieceShapePos)
        {
            item.Dir = dir;
            item.Rotated = rotated;
            item.RotationOffset = rotationOffset;
            item.TetrisPieceShapePos = tetrisPieceShapePos;
        }

        public void SyncGhostFromItem(TetrisItem item)
        {
            tetrisItemGhost.InitializeFromItem(item);
        }

        public TetrisItemGhost GetTetrisItemGhost()
        {
            return tetrisItemGhost;
        }

    }
}