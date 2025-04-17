using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDetails
{
    public int itemID;
    public string itemName;
    public string itemDescription;

    public GameObject itemEntity;

    public TetrisPieceShape tetrisPieceShape;
    public Sprite itemIcon;
    public Sprite itemUI;

    public Transform uiPrefab;
    public Transform gridUIPrefab;
    public InventorySlotType inventorySlotType;
    public int itemDamage;
    public int maxStack;
    public float reloadTime;

    public int yHeight;
    public int xWidth;
    public float weight;
    public Dir dir;

    public int itemPrice;
    [Range(0f, 1f)]
    public float sellPercentage;
    
}

[System.Serializable]
public class PointSet
{
    public int itemShapeID;
    public TetrisPieceShape tetrisPieceShape;
    public List<Vector2Int> points;
}

[System.Serializable]
public class GameSaveData
{
    public Dictionary<string, List<GridItem>> inventoryDict;
    public Dictionary<string, string> gameSaveTimeDict;
}

[System.Serializable]
public class GridItem
{
    public int itemID;
    public int itemIndex;
    public int parentItemIndex;
    public int gridPIndex;
    public int persistentGridTypeIndex;
    public bool isParent;
    public bool isOnSlot;
    public Vector2Int orginPosition;
    public Dir direction;
    public int stack;
}