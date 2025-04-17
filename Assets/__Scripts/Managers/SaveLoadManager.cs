using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ChosTIS.Utility;

namespace ChosTIS.SaveLoadSystem
{
    public class SaveLoadManager : Singleton<SaveLoadManager>
    {
        private List<ISaveable> saveableList = new List<ISaveable>();
        public List<DataSlot> dataSlots = new List<DataSlot>(new DataSlot[3]);
        private string jsonFolder;

        protected override void Awake()
        {
            base.Awake();
            jsonFolder = Application.persistentDataPath + "/ChosTIS SAVE DATA/";
            ReadSaveData();
        }

        private void OnEnable()
        {
            EventHandler.StartGameEvent += OnStartGameEvent;
            EventHandler.SaveGameEvent += OnSaveGameEvent;
            EventHandler.DeleteDataEvent += OnDeleteGameEvent;
        }

        private void OnDisable()
        {
            EventHandler.StartGameEvent -= OnStartGameEvent;
            EventHandler.SaveGameEvent -= OnSaveGameEvent;
            EventHandler.DeleteDataEvent -= OnDeleteGameEvent;
        }

        private void OnStartGameEvent(int index)
        {
            Load(index);
        }

        public void OnSaveGameEvent(int index)
        {
            Save(index);
            //System.GC.Collect();
        }

        public void OnDeleteGameEvent(int index)
        {
            Delete(index);
        }

        public void RegisterSaveable(ISaveable saveable)
        {
            if (!saveableList.Contains(saveable))
            {
                saveableList.Add(saveable);
            }
        }

        private void ReadSaveData()
        {
            if (Directory.Exists(jsonFolder))
            {
                for (int i = 0; i < dataSlots.Count; i++)
                {
                    var resultPath = jsonFolder + "RecordFile" + i + ".json";
                    if (File.Exists(resultPath))
                    {
                        var stringData = File.ReadAllText(resultPath);
                        var jsonData = JsonConvert.DeserializeObject<DataSlot>(stringData);
                        dataSlots[i] = jsonData;
                    }
                    else
                    {
                        dataSlots[i] = null;
                    }
                }
            }
        }

        private void Save(int index)
        {
            DataSlot data = new DataSlot();

            foreach (var saveable in saveableList)
            {
                data.dataDict.Add(saveable.GUID, saveable.GenerateSaveData());
            }
            dataSlots[index] = data;

            var resultPath = jsonFolder + "RecordFile" + index + ".json";
            var jsonData = JsonConvert.SerializeObject(dataSlots[index], Formatting.Indented);

            if (!File.Exists(resultPath))
            {
                Directory.CreateDirectory(jsonFolder);
            }
            Debug.Log("DATA" + index + "SAVED!");
            File.WriteAllText(resultPath, jsonData);
        }

        public void Load(int index)
        {
            var resultPath = jsonFolder + "RecordFile" + index + ".json";
            var stringData = File.ReadAllText(resultPath);
            var jsonData = JsonConvert.DeserializeObject<DataSlot>(stringData);
            foreach (var saveable in saveableList)
            {
                saveable.RestoreData(jsonData.dataDict[saveable.GUID]);
            }
        }

        public void Delete(int index)
        {
            var resultPath = jsonFolder + "RecordFile" + index + ".json";
            if (File.Exists(resultPath))
            {
                File.Delete(resultPath);
                ReadSaveData();
            }
            else
            {
                Debug.Log("The specified file was not found");
            }
        }
    }
}