using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : Singleton_Mono_Method<GameManager>
{
    [Header("Rescue")]
    [SerializeField] float rescueTickInterval = 0.2f;

    [Header("Red Zone")]
    [SerializeField] RedZone redZonePrefab;
    [SerializeField] int maxRedZones = 3;
    [SerializeField] float redZoneLifetime = 3f;
    [SerializeField] float minRedZoneSpawnInterval = 1.2f;
    [SerializeField] float maxRedZoneSpawnInterval = 2.8f;
    [SerializeField] Vector2 redZoneSpawnMinBounds = new Vector2(-8f, -4f);
    [SerializeField] Vector2 redZoneSpawnMaxBounds = new Vector2(8f, 4f);
    [SerializeField] PlayerHealth playerHealth;

    [Header("UI")]
    [SerializeField] GameObject loseUI;

    float nextRescueTime;
    float redZoneSpawnTimer;
    float nextRedZoneSpawnTime;
    readonly List<RedZone> activeRedZones = new List<RedZone>();

    void Start()
    {
        ScheduleNextRedZoneSpawn();
        if (loseUI != null)
        {
            loseUI.SetActive(false);
        }
    }

    void Update()
    {
        HandleRedZoneSpawning();
    }

    void OnValidate()
    {
        maxRedZones = Mathf.Max(1, maxRedZones);
        redZoneLifetime = Mathf.Max(0.1f, redZoneLifetime);
        minRedZoneSpawnInterval = Mathf.Max(0.1f, minRedZoneSpawnInterval);
        maxRedZoneSpawnInterval = Mathf.Max(minRedZoneSpawnInterval, maxRedZoneSpawnInterval);
    }

    public void TryRescue(WoundedSolider woundedSolider)
    {
        if (woundedSolider == null)
        {
            Debug.LogWarning("[GameManager] TryRescue failed: woundedSolider is null.");
            return;
        }

        if (Time.time < nextRescueTime)
        {
            Debug.Log("[GameManager] TryRescue throttled by rescueTickInterval.");
            return;
        }

        nextRescueTime = Time.time + rescueTickInterval;
        Debug.Log("[GameManager] TryRescue accepted. Calling Heal().");
        woundedSolider.Heal();
    }

    public void UpdateRescueUI(WoundedSolider woundedSolider)
    {
        if (woundedSolider == null)
        {
            return;
        }
    }

    public void NotifyRedZoneRemoved(RedZone redZone)
    {
        activeRedZones.Remove(redZone);
    }

    public void ShowPlayerDeadUI()
    {
        if (loseUI != null)
        {
            loseUI.SetActive(true);
        }
    }

    void HandleRedZoneSpawning()
    {
        if (redZonePrefab == null || playerHealth == null)
        {
            return;
        }

        activeRedZones.RemoveAll(zone => zone == null);
        redZoneSpawnTimer += Time.deltaTime;

        if (activeRedZones.Count >= maxRedZones || redZoneSpawnTimer < nextRedZoneSpawnTime)
        {
            return;
        }

        SpawnRedZone();
        redZoneSpawnTimer = 0f;
        ScheduleNextRedZoneSpawn();
    }

    void SpawnRedZone()
    {
        Vector2 spawnPosition = RandomRedZonePosition();
        RedZone newZone = Instantiate(redZonePrefab, spawnPosition, Quaternion.identity);
        newZone.Initialize(playerHealth, redZoneLifetime);

        CircleCollider2D playerCollider = playerHealth.GetComponent<CircleCollider2D>();
        if (playerCollider != null)
        {
            float playerRadius = playerCollider.radius * Mathf.Max(playerHealth.transform.lossyScale.x, playerHealth.transform.lossyScale.y);
            float sideLength = playerRadius * 2f;
            newZone.SetSize(sideLength);
        }

        activeRedZones.Add(newZone);
    }

    Vector2 RandomRedZonePosition()
    {
        float minX = Mathf.Min(redZoneSpawnMinBounds.x, redZoneSpawnMaxBounds.x);
        float maxX = Mathf.Max(redZoneSpawnMinBounds.x, redZoneSpawnMaxBounds.x);
        float minY = Mathf.Min(redZoneSpawnMinBounds.y, redZoneSpawnMaxBounds.y);
        float maxY = Mathf.Max(redZoneSpawnMinBounds.y, redZoneSpawnMaxBounds.y);

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        return new Vector2(randomX, randomY);
    }

    void ScheduleNextRedZoneSpawn()
    {
        nextRedZoneSpawnTime = Random.Range(minRedZoneSpawnInterval, maxRedZoneSpawnInterval);
    }
}
