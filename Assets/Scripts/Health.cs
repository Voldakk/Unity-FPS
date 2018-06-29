using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    private bool dead;

    private Text hudPlayerHealth;

    void Awake ()
    {
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
        if (dead)
            return;

        currentHealth -= amount;
        if (currentHealth <= 0.0f)
        {
            currentHealth = 0.0f;
            Die();
        }

        UpdateHud();
    }

    void Die()
    {
        dead = true;
        SendMessage("OnDeath");
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
}