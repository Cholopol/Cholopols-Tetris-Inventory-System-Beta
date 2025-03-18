using System.Collections.Generic;
using UnityEngine;

public interface ITetrisRotatable
{
    bool Rotated { get; set; }
    Dir Dir { get; set; }
    Vector2Int RotationOffset { get; set; }
    ItemDetails ItemDetails { get; set; }
    List<Vector2Int> TetrisPieceShapePos { get; set; }

    int WIDTH { get; }
    int HEIGHT { get; }
}
