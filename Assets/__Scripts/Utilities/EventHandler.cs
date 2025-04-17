using System;

namespace ChosTIS.Utility
{
    public static class EventHandler
    {
        public static event Action<int> StartNewGameEvent;
        public static void CallStartNewGameEvent(int index)
        {
            StartNewGameEvent?.Invoke(index);
        }

        public static event Action<int> ContinueGameEvent;
        public static void CallContinueGameEvent(int index)
        {
            ContinueGameEvent?.Invoke(index);
        }

        public static event Action BegineGameEvent;
        public static void CallBegineGameEvent()
        {
            BegineGameEvent?.Invoke();
        }

        public static event Action<int> StartGameEvent;
        public static void CallStartGameEvent(int index)
        {
            StartGameEvent?.Invoke(index);
        }

        public static event Action<int> SaveGameEvent;
        public static void CallSaveGameEvent(int index)
        {
            SaveGameEvent?.Invoke(index);
        }

        public static event Action<int> DeleteDataEvent;
        public static void CallDeleteDataEvent(int index)
        {
            DeleteDataEvent?.Invoke(index);
        }

        public static event Action DeleteObjectEvent;
        public static void CallDeleteObjectEvent()
        {
            DeleteObjectEvent?.Invoke();
        }

        public static event Action InstantiateInventoryItemUI;
        public static void CallInstantiateInventoryItemUI()
        {
            InstantiateInventoryItemUI?.Invoke();
        }

    }
}