using System.Collections.Generic;
using UnityEngine;

public class SoldierManager : Singleton_Mono_Method<SoldierManager>
{
    List<Soldier> soldiers;
    [SerializeField] int maxSoldiers = 10;
    [SerializeField] float minSpawnInterval = 5f;
    float timeSinceLastSpawn = 0f;
    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (soldiers.Count < maxSoldiers && timeSinceLastSpawn >= minSpawnInterval)
        {
            Soldier newSoldier = new Soldier();
            SpawnWoundedSoldier(RandomSpawnSoldier(newSoldier), newSoldier);            
            timeSinceLastSpawn = 0f;
        }
    }
    public void SpawnWoundedSoldier(Vector2 position, Soldier newSoldier = null )
    {
        Instantiate(newSoldier?.soldierPrefab ?? null, position, Quaternion.identity);
        Debug.Log("Wounded soldier spawned at: " + position);
    }
    private Vector2 RandomSpawnSoldier(Soldier newSoldier)
    {
        newSoldier.soldierPrefab.transform.position = new Vector2(Random.Range(1f, 15f), Random.Range(50f, 100f));
        soldiers.Add(newSoldier);
        return newSoldier.soldierPrefab.transform.position;
    }
}
