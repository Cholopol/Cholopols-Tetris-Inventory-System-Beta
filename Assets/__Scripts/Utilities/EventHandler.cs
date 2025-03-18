using UnityEngine;
using System;

namespace ChosTIS.Utility
{
    public static class EventHandler
    {
        public static event Action<float> UpdateScrollHeight;
        public static void CallUpdateScrollHeight(float height)
        {
            UpdateScrollHeight?.Invoke(height);
        }
    }
}