using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player HP")]
    [SerializeField, Min(1)] int maxHP = 10;
    [SerializeField] TMP_Text hpText;

    int currentHP;
    int redZoneContactCount;
    float redZoneDamageTimer;
    bool isDead;

    public int CurrentHP => currentHP;

    void Start()
    {
        currentHP = maxHP;
        UpdateUI();
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

        if (redZoneContactCount <= 0)
        {
            redZoneDamageTimer = 0f;
            return;
        }

        redZoneDamageTimer += Time.deltaTime;
        if (redZoneDamageTimer >= 1f)
        {
            redZoneDamageTimer = 0f;
            TakeDamage(1);
        }
    }

    public void EnterRedZone()
    {
        redZoneContactCount++;
    }

    public void ExitRedZone()
    {
        redZoneContactCount = Mathf.Max(0, redZoneContactCount - 1);
    }

    public void TakeDamage(int amount)
    {
        if (isDead || amount <= 0)
        {
            return;
        }

        currentHP = Mathf.Max(0, currentHP - amount);
        UpdateUI();

        if (currentHP == 0)
        {
            isDead = true;
            GameManager.d_Instance?.ShowPlayerDeadUI();
        }
    }

    void UpdateUI()
    {
        if (hpText != null)
        {
            hpText.text = currentHP + " / " + maxHP;
        }
    }
}
