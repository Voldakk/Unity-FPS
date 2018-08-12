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

        switch (part.partType)
        {
            case WeaponPartType.Barrel:
                var b = part as Barrel;
                AddStat("Damage", b.Damage);
                AddStat("Recoil", b.Recoil);
                AddStat("Accuracy", b.Accuracy);
                break;

            case WeaponPartType.Body:
                var bd = part as Body;
                AddStat("Firerate", bd.FireRate);
                break;

            case WeaponPartType.Grip:
                var g = part as Grip;
                //AddModifier("Recoil", g.RecoilModifier);
                break;

            case WeaponPartType.Mag:
                var m = part as Mag;
                AddStat("Size", m.MagSize);
                AddStat("Reload time", m.ReloadTime);
                //AddModifier("Recoil", m.recoilModifier);
                break;

            case WeaponPartType.Sight:
                var s = part as Sight;
                AddStat("Zoom", s.Zoom);
                AddStat("ADS time", s.AdsTime);
                AddModifier("Accuracy", s.AccuracyModifier);
                break;

            case WeaponPartType.Stock:
                var st = part as Stock;
                //AddModifier("Recoil", st.RecoilModifier);
                break;

            case WeaponPartType.Muzzle:
                var mu = part as Muzzle;
                AddStat("Damage", mu.Damage);
                AddStat("Recoil", mu.Recoil);
                AddModifier("Accuracy", mu.AccuracyModifier);
                break;

            default:
                break;
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

    void AddStat(string name, float value)
    {
        AddStat(name, value.ToString("0.0"));
    }
    void AddStat(string name, int value)
    {
        AddStat(name, value.ToString());
    }
    void AddStat(string name, string value)
    {
        var statsLine = Instantiate(statsLinePrefab, statsPanel).GetComponent<UiWeaponPartStatsLine>();
        statsLine.Set(name, value);
    }

    void AddModifier(string name, float value)
    {
        if (value == 1f)
            return;

        var statsLine = Instantiate(statsLinePrefab, statsPanel).GetComponent<UiWeaponPartStatsLine>();
        float p = (value - 1.0f) * 100f;
        statsLine.Set(name, (p > 0f ? "+" : "") + p.ToString("0.0") + "%");
    }
}