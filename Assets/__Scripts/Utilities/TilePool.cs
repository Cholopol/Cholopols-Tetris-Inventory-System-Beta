using UnityEngine;
using System.Collections.Generic;

namespace ChosTIS
{
    public class TilePool : MonoBehaviour
    {
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private int initialPoolSize = 10;

        private Queue<GameObject> pool = new Queue<GameObject>();

        private void Awake()
        {
            InitializePool();
        }

        // Initialize object pool
        private void InitializePool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                CreateNewTile();
            }
        }

        // Create a new Tile and add it to the pool
        private GameObject CreateNewTile()
        {
            GameObject tile = Instantiate(tilePrefab, transform);
            tile.SetActive(false);
            pool.Enqueue(tile);
            return tile;
        }

        // Retrieve available tiles from the pool
        public GameObject GetTile()
        {
            if (pool.Count == 0)
            {
                // Dynamic expansion when the pool is empty
                CreateNewTile();
            }

            GameObject tile = pool.Dequeue();
            tile.SetActive(true);
            return tile;
        }

        // Return Tile to the Pool
        public void ReturnTile(GameObject tile)
        {
            tile.SetActive(false);
            pool.Enqueue(tile);
        }

        // Empty pool (optional)
        public void ClearPool()
        {
            foreach (var tile in pool)
            {
                Destroy(tile);
            }
            pool.Clear();
        }
    }
}