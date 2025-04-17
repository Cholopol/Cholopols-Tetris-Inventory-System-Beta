using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChosTIS.SaveLoadSystem
{
    [CreateAssetMenu(fileName = "InventoryData_SO", menuName = "SaveLoad/InventoryDataList")]
    public class InventoryData_SO : ScriptableObject
    {
        public List<GridItem> inventoryItemList;

        public GridItem GetGridItem(int index)
        {
            return inventoryItemList.Find(e => e.itemIndex == index);
        }

        public bool HasGridItem(int index)
        {
            return inventoryItemList.Any(p => p.itemIndex == index);
        }

    }
}