using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiWeaponPartHoverPanel : MonoBehaviour
{
    static UiWeaponPartHoverPanel instance;

    public TextMeshProUGUI partLevel;
    public TextMeshProUGUI partName;
    public Image partIcon;

    public Transform statsPanel;
    public GameObject statsLinePrefab;

    public TextMeshProUGUI partCost;

    void Awake()
    {
        instance = this;
    }

    public static void Show(WeaponPart part)
    {
        if (instance != null)
            instance.ShowPanel(part);
    }

    public void ShowPanel(WeaponPart part)
    {
        if(part == null)
        {
            Hide();
            return;
        }


        RectTransform rt = GetComponent<RectTransform>();
        rt.pivot = new Vector2(Input.mousePosition.x <= Screen.width/2f ? 0 : 1, Input.mousePosition.y <= Screen.height/2f ? 0 : 1);

        transform.position = Input.mousePosition;

        for (int i = 0; i < statsPanel.childCount; i++)
        {
            Destroy(statsPanel.GetChild(i).gameObject);
        }

        partName.text = part.name;
        partName.color = part.quality.color;

        partLevel.text = "LEVEL REQUIREMENT: " + part.level;

        partCost.text = "$" + part.price;

        partIcon.sprite = part.icon;

        foreach (var stat in part.stats)
        {
            switch (stat.statsType)
            {
                case StatsType.Base:
                    AddStat(stat, part.GetStats(stat.stats));
                    break;
                case StatsType.Additive:
                    AddStat(stat, part.GetStats(stat.stats));
                    break;
                case StatsType.Modifier:
                    AddModifier(stat, part.GetStats(stat.stats));
                    break;
                default:
                    break;
            }
        }
    }

    public static void Hide()
    {
        if(instance != null)
            instance.HidePanel();
    }

    public void HidePanel()
    {
        transform.position = Vector3.one * 1000000f;
    }

    void AddStat(WeaponPartStat stat, float value)
    {
        AddStat(stat.stats.ToString(), stat.isInt ? ((int)value).ToString() : value.ToString("0.0"));
    }

    void AddStat(string name, string value)
    {
        var statsLine = Instantiate(statsLinePrefab, statsPanel).GetComponent<UiWeaponPartStatsLine>();
        statsLine.Set(name, value);
    }

    void AddModifier(WeaponPartStat stat, float value)
    {
        if (value == 1f)
            return;

        var statsLine = Instantiate(statsLinePrefab, statsPanel).GetComponent<UiWeaponPartStatsLine>();
        float p = (value - 1.0f) * 100f;
        statsLine.Set(stat.stats.ToString(), (p > 0f ? "+" : "") + p.ToString("0.0") + "%");
    }
}