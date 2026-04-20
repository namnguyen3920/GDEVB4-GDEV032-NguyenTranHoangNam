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
            return;
        }

        currentHP = Mathf.Min(currentHP + healPerTick, fullHP);
        UpdateHPUI();
        GameManager.d_Instance?.UpdateRescueUI(this);

        if (currentHP >= fullHP)
        {
            SoldierManager.d_Instance?.NotifySoldierRemoved(this);
            Destroy(gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        GameManager.d_Instance?.TryRescue(this);
    }

    void UpdateHPUI()
    {
        if (hpText == null)
        {
            return;
        }

        hpText.text = currentHP + " / " + fullHP;
    }
}
