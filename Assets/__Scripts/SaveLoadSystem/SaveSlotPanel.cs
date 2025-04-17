using System;
using System.Collections.Generic;

namespace ChosTIS.SaveLoadSystem
{
    public class SaveSlotPanel : Singleton<SaveSlotPanel>, ISaveable
    {
        public string GUID => GetComponent<DataGUID>().guid;

        private void Start()
        {
            ISaveable saveable = this;
            saveable?.RegisterSaveable();
        }

        private string SaveTime
        {
            get
            {
                int hour = DateTime.Now.Hour;
                int minute = DateTime.Now.Minute;
                int second = DateTime.Now.Second;
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month;
                int day = DateTime.Now.Day;
                return string.Format("{0:D4}/{1:D2}/{2:D2}" + " " + "{3:D2}:{4:D2}:{5:D2} ", year, month, day, hour, minute, second);
            }
        }

        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new()
            {
                gameSaveTimeDict = new Dictionary<string, string>
                {
                    { "SaveTime", SaveTime }
                }
            };

            return saveData;
        }

        public void RestoreData(GameSaveData saveData)
        {

        }
    }
}