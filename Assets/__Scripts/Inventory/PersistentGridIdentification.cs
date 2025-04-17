using UnityEngine;

namespace ChosTIS
{
    [RequireComponent(typeof(TetrisItemGrid))]
    public class PersistentGridIdentification : MonoBehaviour
    {
        [Header("Manual Configuration")]
        public TetrisItemGrid persistentGridObject;
        public PersistentGridType persistentGridType;

        public int GetPersistentGridTypeIndex()
        {
            return (int)persistentGridType;
        }
    }
}