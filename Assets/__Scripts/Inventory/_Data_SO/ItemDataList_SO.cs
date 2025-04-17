using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataList_SO", menuName = "Inventory/ItemDataList")]
public class ItemDataList_SO : ScriptableObject
{
    public List<ItemDetails> itemDetailsList;

    public ItemDetails GetItemDetailsByID(int ID)
    {
        return itemDetailsList.Find(i => i.itemID == ID);
    }

    public ItemDetails GetItemDetailsByIndex(int index)
    {
        return itemDetailsList[index];
    }

}
