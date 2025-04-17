using UnityEngine;
using UnityEngine.UI;

namespace ChosTIS
{
    public class StackableComponent : MonoBehaviour, IItemComponent, IStackable
    {
        public int MaxStack { get; set; }
        public int CurrentStack { get; set; }
        public Text stackNumText;
        private TetrisItem _owner;

        public void OnAttach(TetrisItem item) => _owner = item;
        public void OnDetach() => _owner = null;

        public void Initialize(GridItem itemData)
        {
            MaxStack = _owner.ItemDetails.maxStack;
            if (itemData != null)
            {
                CurrentStack = itemData.stack;
            }
            else
            {
                CurrentStack = MaxStack;
            }
            if (stackNumText == null)
            {
                stackNumText = transform.Find("StackNum").GetComponent<Text>();
            }
            if (stackNumText != null)
            {
                stackNumText.text = CurrentStack.ToString();
            }
        }

        public bool TryMergeStack(StackableComponent other)
        {
            if (CurrentStack >= MaxStack) return false;

            var transfer = Mathf.Min(other.CurrentStack, MaxStack - CurrentStack);
            CurrentStack += transfer;
            other.CurrentStack -= transfer;
            UpdateCurrentStackText(this);
            UpdateCurrentStackText(other);
            return true;
        }

        public bool TrySplitStack(int amount)
        {
            if (CurrentStack <= 1 || amount <= 0 || amount >= CurrentStack) return false;
            StackableComponent newStack = InventoryManager.Instance.CreateNewStackableItem(_owner.ItemDetails.itemID);
            newStack.CurrentStack = amount;
            CurrentStack -= amount;
            UpdateCurrentStackText(this);
            UpdateCurrentStackText(newStack);
            return true;
        }

        private void UpdateCurrentStackText(StackableComponent stackableComponent)
        {
            if (stackableComponent.stackNumText != null)
            {
                stackableComponent.stackNumText.text = stackableComponent.CurrentStack.ToString();
            }
        }
    }
}