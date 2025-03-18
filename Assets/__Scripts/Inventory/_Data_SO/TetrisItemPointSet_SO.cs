using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TetrisItemPointSet_SO", menuName = "Inventory/ItemPointSet")]
public class TetrisItemPointSet_SO : ScriptableObject
{
    public List<PointSet> TetrisPieceShapeList;

    //public List<Vector2Int> GetTetrisPointSetByID(int index)
    //{
    //    return TetrisPieceShapeList[index].points;
    //}

}
