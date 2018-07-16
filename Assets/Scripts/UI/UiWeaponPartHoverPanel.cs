using UnityEngine;
using UnityEngine.UI;

public class UiWeaponPartHoverPanel : MonoBehaviour
{
    public Text partName;
    public Image partIcon;

    public Transform statsPanel;
    public GameObject statsLinePrefab;

    public void Show(WeaponPart part)
    {
        if(part == null)
        {
            Hide();
            return;
        }

        transform.position = Input.mousePosition;

        for (int i = 0; i < statsPanel.childCount; i++)
        {
            Destroy(statsPanel.GetChild(i).gameObject);
        }

        partName.text = part.partName;
        partIcon.sprite = part.icon;


        switch (part.partType)
        {
            case WeaponPartType.Barrel:
                var b = part as Barrel;
                AddStat("Accuracy", b.accuracy);
                AddModifier("Damage", b.damageModifier);
                AddModifier("Recoil", b.recoilModifier);
                break;

            case WeaponPartType.Body:
                var bd = part as Body;
                AddStat("Firerate", bd.fireRate);
                break;

            case WeaponPartType.Grip:
                var g = part as Grip;
                AddModifier("Recoil", g.recoilModifier);
                break;

            case WeaponPartType.Mag:
                var m = part as Mag;
                AddStat("Size", m.magSize);
                AddModifier("Recoil", m.recoilModifier);
                break;

            case WeaponPartType.Sight:
                var s = part as Sight;
                AddStat("Fov", s.fov);
                AddModifier("Accuracy", s.accuracyModifier);
                break;

            case WeaponPartType.Stock:
                var st = part as Stock;
                AddModifier("Recoil", st.recoilModifier);
                break;

            case WeaponPartType.Muzzle:
                var mu = part as Muzzle;
                AddModifier("Damage", mu.damageModifier);
                AddModifier("Accuracy", mu.accuracyModifier);
                AddModifier("Recoil", mu.recoilModifier);
                break;

            default:
                break;
        }

    }

    public void Hide()
    {
        transform.position = Vector3.one * 1000000f;
    }

    void AddStat(string name, float value)
    {
        AddStat(name, value.ToString());
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