using System.Collections.Generic;
using UnityEngine;

namespace ChosTIS.Utility
{
    public class PoolManager : Singleton<PoolManager>
    {
        private Dictionary<string, Queue<GameObject>> objectPool = new();
        private GameObject pool;

        public GameObject GetObject(GameObject prefab)
        {
            //Declare the object to be retrieved
            GameObject _object;
            if (!objectPool.ContainsKey(prefab.name) || objectPool[prefab.name].Count == 0)
            {
                _object = GameObject.Instantiate(prefab);
                PushObject(_object);
                if (pool == null)
                {
                    pool = new GameObject("ObjectPool");
                    pool.transform.SetParent(this.transform);
                }
                GameObject childPool = GameObject.Find(prefab.name + "Pool");
                if (!childPool)
                {
                    childPool = new GameObject(prefab.name + "Pool");
                    childPool.transform.SetParent(pool.transform);
                }
                _object.transform.SetParent(childPool.transform);
            }

            _object = objectPool[prefab.name].Dequeue();
            _object.SetActive(true);
            return _object;
        }

        public void PushObject(GameObject prefab)
        {
            string _name = prefab.name.Replace("(Clone)", string.Empty);
            if (!objectPool.ContainsKey(_name))
            {
                objectPool.Add(_name, new Queue<GameObject>());
            }
            objectPool[_name].Enqueue(prefab);
            prefab.SetActive(false);
        }
    }
}