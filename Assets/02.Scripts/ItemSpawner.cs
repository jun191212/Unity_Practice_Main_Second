using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject itemPrefab;
    public int itemCount = 10;
    public bool useRandomSpawn = true;

    [Header("Spawn Area")]
    public Vector3 spawnAreaCenter = Vector3.zero;
    public Vector3 spawnAreaSize = new Vector3(20f, 0f, 20f);
    public float spawnHeight = 1f;

    [Header("Spawn Points (if not using random)")]
    public Transform[] spawnPoints;

    [Header("Respawn Settings")]
    public bool enableRespawn = false;
    public float respawnDelay = 5f;

    [Header("Pool Settings")]
    public int poolSize = 20;

    private Queue<GameObject> itemPool = new Queue<GameObject>();
    private List<GameObject> activeItems = new List<GameObject>();

    void Start()
    {
        InitializePool();
        SpawnInitialItems();
    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(itemPrefab);
            obj.SetActive(false);
            itemPool.Enqueue(obj);
        }
    }

    void SpawnInitialItems()
    {
        if (GameManager.Instance != null)
        {
            itemCount = GameManager.Instance.GetTotalItems();
        }

        for (int i = 0; i < itemCount; i++)
        {
            SpawnItem();
        }
    }

    void SpawnItem()
    {
        Vector3 spawnPosition;

        if (useRandomSpawn)
        {
            spawnPosition = GetRandomSpawnPosition();
        }
        else
        {
            spawnPosition = GetSpawnPointPosition();
        }

        GameObject item = GetItemFromPool();
        if (item != null)
        {
            CollectibleItem collectible = item.GetComponent<CollectibleItem>();
            if (collectible != null)
            {
                collectible.ResetItem(spawnPosition);
            }
            else
            {
                item.transform.position = spawnPosition;
                item.SetActive(true);
            }

            activeItems.Add(item);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2);
        float z = Random.Range(spawnAreaCenter.z - spawnAreaSize.z / 2, spawnAreaCenter.z + spawnAreaSize.z / 2);
        
        return new Vector3(x, spawnHeight, z);
    }

    Vector3 GetSpawnPointPosition()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            return spawnPoints[randomIndex].position;
        }

        return GetRandomSpawnPosition();
    }

    GameObject GetItemFromPool()
    {
        if (itemPool.Count > 0)
        {
            GameObject item = itemPool.Dequeue();
            item.SetActive(true);
            return item;
        }

        GameObject newItem = Instantiate(itemPrefab);
        return newItem;
    }

    public void ReturnItemToPool(GameObject item)
    {
        activeItems.Remove(item);
        item.SetActive(false);
        itemPool.Enqueue(item);

        if (enableRespawn && !IsGameComplete())
        {
            Invoke("SpawnItem", respawnDelay);
        }
    }

    bool IsGameComplete()
    {
        if (GameManager.Instance != null)
        {
            return GameManager.Instance.GetItemsCollected() >= GameManager.Instance.GetTotalItems();
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);

        if (spawnPoints != null)
        {
            Gizmos.color = Color.red;
            foreach (Transform point in spawnPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawWireSphere(point.position, 0.5f);
                }
            }
        }
    }
}
