using UnityEngine;

public class UiWeaponStats : MonoBehaviour
{
    public static UiWeaponStats instance;

    public ModularWeapon weapon;
    public GameObject statsLinePrefab;

    private void Awake()
    {
        instance = this;
    }

    public static void UpdateStats()
    {
        instance.UpdateStatsInstance();
    }

    void UpdateStatsInstance()
    {
        weapon.UpdateStats();

        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        foreach (var key in weapon.stats.Keys)
        {
            AddStat(key.ToString(), weapon.stats[key].ToString("0.0"));
        }

        foreach (var key in weapon.modifiers.Keys)
        {
            AddModifier(key.ToString(), weapon.modifiers[key]);
        }
    }

    void AddStat(string name, string value)
    {
        var statsLine = Instantiate(statsLinePrefab, transform).GetComponent<UiWeaponPartStatsLine>();
        statsLine.Set(name, value);
    }

    void AddModifier(string name, float value)
    {
        if (value == 1f)
            return;

        var statsLine = Instantiate(statsLinePrefab, transform).GetComponent<UiWeaponPartStatsLine>();
        float p = (value - 1.0f) * 100f;
        statsLine.Set(name, (p > 0f ? "+" : "") + p.ToString("0.0") + "%");
    }
}
