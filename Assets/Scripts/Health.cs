using GameSparks.RT;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : NetworkObject
{
    public float maxHealth = 100f;
    private float currentHealth;

    private bool dead;

    private Text hudPlayerHealth;

    public UnityEvent onDamage, onDeath;

    protected override void Awake ()
    {
        base.Awake();

        Reset();
    }

    public void Initialize(bool isPlayer)
    {
        if (!isPlayer)
            return;

        hudPlayerHealth = GameObject.Find("HudPlayerHealth").GetComponent<Text>();
        UpdateHud();
    }

    public void Damage(float amount)
    {
        SendFloat(1, amount);
        ApplyDamage(amount);
    }

    private void ApplyDamage(float amount)
    {
        if (dead)
            return;

        currentHealth -= amount;
        if (currentHealth <= 0.0f)
        {
            currentHealth = 0.0f;
            OnDeath();
        }

        UpdateHud();
        onDamage.Invoke();
    }

    public void Die()
    {
        ApplyDamage(currentHealth);
    }

    void OnDeath()
    {
        dead = true;
        onDeath.Invoke();
    }

    void Reset()
    {
        currentHealth = maxHealth;
        dead = false;
        UpdateHud();
    }

    void UpdateHud()
    {
        if(hudPlayerHealth != null)
            hudPlayerHealth.text = Mathf.CeilToInt(currentHealth).ToString();
    }

    public override void OnPacket(RTPacket packet)
    {
        ApplyDamage(packet.Data.GetFloat(1).Value);
    }

    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}