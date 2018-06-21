using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

	void Start ()
    {
        currentHealth = maxHealth;
    }

    public void Damage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0.0f)
        {
            currentHealth = 0.0f;
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
