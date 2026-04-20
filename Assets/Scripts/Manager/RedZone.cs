using UnityEngine;

public class RedZone : MonoBehaviour
{
    PlayerHealth targetPlayer;
    float lifetime;
    float elapsedTime;
    bool isPlayerInside;

    public void Initialize(PlayerHealth playerHealth, float zoneLifetime)
    {
        targetPlayer = playerHealth;
        lifetime = zoneLifetime;
        elapsedTime = 0f;
    }

    public void SetSize(float sideLength)
    {
        transform.localScale = new Vector3(sideLength, sideLength, 1f);
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= lifetime)
        {
            GameManager.d_Instance?.NotifyRedZoneRemoved(this);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (targetPlayer == null || !other.CompareTag("Player"))
        {
            return;
        }

        isPlayerInside = true;
        targetPlayer.EnterRedZone();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (targetPlayer == null || !other.CompareTag("Player"))
        {
            return;
        }

        isPlayerInside = false;
        targetPlayer.ExitRedZone();
    }

    void OnDisable()
    {
        if (targetPlayer == null || !isPlayerInside)
        {
            return;
        }

        targetPlayer.ExitRedZone();
        isPlayerInside = false;
    }
}
