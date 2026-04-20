using UnityEngine;
using TMPro;

public class WoundedSolider : MonoBehaviour
{
    [System.Serializable]
    public class WoundedSoliderData
    {
        [Min(0.1f)] public float lifetime = 12f;
    }

    [Header("Data")]
    [SerializeField] WoundedSoliderData data = new WoundedSoliderData();

    [Header("HP")]
    [SerializeField, Min(1)] int fullHP = 10;
    [SerializeField, Min(1)] int healPerTick = 1;
    [SerializeField] TMP_Text hpText;

    float aliveTime;
    int currentHP;

    public int CurrentHP => currentHP;
    public int FullHP => fullHP;
    public float Lifetime => data.lifetime;

    void OnEnable()
    {
        ResetState();
    }

    void Update()
    {
        aliveTime += Time.deltaTime;
        if (aliveTime >= data.lifetime)
        {
            SoldierManager.d_Instance?.NotifySoldierRemoved(this);
            Destroy(gameObject);
        }
    }

    public void ResetState()
    {
        aliveTime = 0f;
        currentHP = 0;
        UpdateHPUI();
        GameManager.d_Instance?.UpdateRescueUI(this);
    }

    public void Heal()
    {
        if (currentHP >= fullHP)
        {
            Debug.Log("[WoundedSolider] Heal skipped: HP already full.");
            return;
        }

        currentHP = Mathf.Min(currentHP + healPerTick, fullHP);
        Debug.Log("[WoundedSolider] Healed. Current HP: " + currentHP + " / " + fullHP);
        UpdateHPUI();
        GameManager.d_Instance?.UpdateRescueUI(this);

        if (currentHP >= fullHP)
        {
            SoldierManager.d_Instance?.NotifySoldierRemoved(this);
            Destroy(gameObject);
        }
    }

    public void HandlePlayerTrigger(Collider2D other)
    {
        Debug.Log("[WoundedSolider] Trigger entered by: " + other.name + ", tag: " + other.tag);
        if (!other.CompareTag("Player"))
        {
            Debug.Log("[WoundedSolider] Trigger ignored: collider is not Player tag.");
            return;
        }

        Debug.Log("[WoundedSolider] Player detected. Requesting rescue from GameManager.");
        GameManager.d_Instance?.TryRescue(this);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandlePlayerTrigger(other);
    }

    void UpdateHPUI()
    {
        if (hpText == null)
        {
            Debug.LogWarning("[WoundedSolider] hpText is not assigned.");
            return;
        }

        hpText.text = currentHP + " / " + fullHP;
        Debug.Log("[WoundedSolider] HP text updated to: " + hpText.text);
    }
}
