using System.Collections.Generic;
using UnityEngine;

public class ChunkPoolManager : MonoBehaviour
{
    public static ChunkPoolManager Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, List<GameObject>> activeChunks;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        activeChunks = new Dictionary<string, List<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            activeChunks[pool.tag] = new List<GameObject>();

            for (int i = 0; i < pool.size+1; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        GameObject obj;

        if (poolDictionary[tag].Count > 0)
        {
            obj = poolDictionary[tag].Dequeue();
        }
        else
        {
            // No available chunks — recycle the first active one
            if (activeChunks[tag].Count > 0)
            {
                obj = activeChunks[tag][0];
                activeChunks[tag].RemoveAt(0);
                Debug.LogWarning($"Pool exhausted. Recycling chunk: {obj.name}");
            }
            else
            {
                Debug.LogError($"No chunks available or recyclable for tag {tag}");
                return null;
            }
        }

        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        activeChunks[tag].Add(obj);

        return obj;
    }

    public void ReturnToPool(string tag, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return;
        }

        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
        activeChunks[tag].Remove(obj);
    }
}
