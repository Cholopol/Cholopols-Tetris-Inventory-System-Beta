using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDetails
{
    public int itemID;
    public string itemName;
    //public ItemType itemType;
    public string itemDescription;

    public GameObject itemEntity;

    public TetrisPieceShape tetrisPieceShape;
    public Sprite itemIcon;
    public Sprite itemUI;

    public Transform uiPrefab;
    public Transform gridUIPrefab;
    public InventorySlotType inventorySlotType;
    public int itemDamage;
    public int load;    //容量
    public float reloadTime;

    public int yHeight;
    public int xWidth;
    public float weight;
    public Dir dir;

    public int itemPrice;
    //售出时的折扣百分比
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