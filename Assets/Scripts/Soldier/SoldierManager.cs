using System.Collections.Generic;
using UnityEngine;

public class SoldierManager : Singleton_Mono_Method<SoldierManager>
{
    [Header("Spawn")]
    [SerializeField] WoundedSolider woundedSoliderPrefab;
    [SerializeField] int maxActiveSoldiers = 10;
    [SerializeField] float startSpawnInterval = 5f;
    [SerializeField] float minSpawnInterval = 1.5f;
    [SerializeField] float timeDecrementPerInterval = 0.2f;
    [SerializeField] Vector2 spawnMinBounds = new Vector2(1f, 50f);
    [SerializeField] Vector2 spawnMaxBounds = new Vector2(15f, 100f);

    readonly List<WoundedSolider> activeSoldiers = new List<WoundedSolider>();
    float currentSpawnInterval;
    float timeSinceLastSpawn;

    void OnValidate()
    {
        maxActiveSoldiers = Mathf.Max(1, maxActiveSoldiers);
        startSpawnInterval = Mathf.Max(0.1f, startSpawnInterval);
        minSpawnInterval = Mathf.Max(0.1f, minSpawnInterval);
        timeDecrementPerInterval = Mathf.Max(0f, timeDecrementPerInterval);
    }

    void Start()
    {
        currentSpawnInterval = Mathf.Max(minSpawnInterval, startSpawnInterval);
    }

    void Update()
    {
        activeSoldiers.RemoveAll(soldier => soldier == null);
        timeSinceLastSpawn += Time.deltaTime;

        if (activeSoldiers.Count < maxActiveSoldiers && timeSinceLastSpawn >= currentSpawnInterval)
        {
            SpawnWoundedSoldier(RandomSpawnPosition());
            currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval - timeDecrementPerInterval);
            timeSinceLastSpawn = 0f;
        }
    }

    public void SpawnWoundedSoldier(Vector2 position)
    {
        if (woundedSoliderPrefab == null)
        {
            return;
        }

        WoundedSolider soldier = Instantiate(woundedSoliderPrefab, position, Quaternion.identity);
        activeSoldiers.Add(soldier);
        Debug.Log("Wounded soldier spawned at: " + position);
    }

    public void NotifySoldierRemoved(WoundedSolider woundedSolider)
    {
        activeSoldiers.Remove(woundedSolider);
    }

    private Vector2 RandomSpawnPosition()
    {
        float minX = Mathf.Min(spawnMinBounds.x, spawnMaxBounds.x);
        float maxX = Mathf.Max(spawnMinBounds.x, spawnMaxBounds.x);
        float minY = Mathf.Min(spawnMinBounds.y, spawnMaxBounds.y);
        float maxY = Mathf.Max(spawnMinBounds.y, spawnMaxBounds.y);

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        return new Vector2(randomX, randomY);
    }
}
