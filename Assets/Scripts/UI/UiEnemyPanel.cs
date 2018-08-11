using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class UiEnemyPanel : MonoBehaviour
{
    static UiEnemyPanel instance;

    public TextMeshProUGUI nameText, levelText;
    public Slider healthSlider;

    public float hideTime;
    private float hideTimer;

    Enemy currentEnemy;

    UnityAction updateCall;

    void Awake()
    {
        instance = this;

        updateCall = new UnityAction(delegate () {
            ShowPanel(currentEnemy);
        });

        HidePanel();

    }

    private void Update()
    {
        hideTimer -= Time.deltaTime;
        if (hideTimer <= 0.0f)
            HidePanel();
    }

    public static void Show(Enemy enemy)
    {
        if (instance != null)
            instance.ShowPanel(enemy);
    }

    public void ShowPanel(Enemy enemy)
    {
        HidePanel();
        currentEnemy = enemy;

        if (currentEnemy == null)
            return;

        gameObject.SetActive(true);
        hideTimer = hideTime;

        nameText.text = currentEnemy.gameObject.name;
        levelText.text = ((int)1).ToString("00");
        healthSlider.value = currentEnemy.health.CurrentHealth() / currentEnemy.health.maxHealth;

        currentEnemy.health.onDamage.AddListener(updateCall);
    }

    public static void Hide()
    {
        if (instance != null)
            instance.HidePanel();
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);

        if(currentEnemy != null)
        {
            if (currentEnemy.health != null)
            {
                currentEnemy.health.onDamage.RemoveListener(updateCall);
            }
        }

        currentEnemy = null;
    }
}
