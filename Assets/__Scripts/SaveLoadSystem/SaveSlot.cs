using UnityEngine;
using UnityEngine.UI;

namespace ChosTIS.SaveLoadSystem
{
    public class SaveSlot : MonoBehaviour
    {
        public Text saveTime;
        [SerializeField] private Button delletBtn, saveBtn, startBtn;
        private DataSlot currentData;
        private int Index => transform.GetSiblingIndex();

        private void Awake()
        {
            startBtn.onClick.AddListener(LoadGameData);
            saveBtn.onClick.AddListener(SaveGameData);
            delletBtn.onClick.AddListener(DeleteGameData);
        }

        private void Start()
        {
            SetupSlotUI();

        }

        private void SetupSlotUI()
        {
            currentData = SaveLoadManager.Instance.dataSlots[Index];

            if (currentData != null)
            {
                saveTime.text = "Save Time£º" + currentData.SaveTime;
                delletBtn.gameObject.SetActive(true);
            }
            else
            {
                saveTime.text = "NUll";
                delletBtn.gameObject.SetActive(false);
                saveBtn.gameObject.SetActive(true);
            }
        }

        private void LoadGameData()
        {
            if (currentData != null)
            {
                Debug.Log("[LoadGame]" + Index);
                ChosTIS.Utility.EventHandler.CallStartGameEvent(Index);
                ChosTIS.Utility.EventHandler.CallInstantiateInventoryItemUI();
            }
        }

        private void SaveGameData()
        {
            Debug.Log("[SaveGame]" + Index);
            ChosTIS.Utility.EventHandler.CallSaveGameEvent(Index);
            SetupSlotUI();
        }

        private void DeleteGameData()
        {
            if (currentData != null)
            {
                Debug.Log("[DeleteGame]" + Index);
                ChosTIS.Utility.EventHandler.CallDeleteDataEvent(Index);
                ChosTIS.Utility.EventHandler.CallDeleteObjectEvent();
                SetupSlotUI();
            }

            Utilities.TetrisItemUtilities.TriggerPointerEnter(InventoryManager.Instance.depositoryGrid.gameObject);
        }

    }
}