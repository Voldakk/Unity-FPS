using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;
    private float currentHealth;

    private bool dead;

	void Awake ()
    {
        Reset();
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
    }
}
