using UnityEngine;
using TMPro;

public class UiWeaponPartStatsLine : MonoBehaviour
{
    public TextMeshProUGUI statsName, statsValue;

    public void Set(string name, string value)
    {
        statsName.text = name;
        statsValue.text = value;
    }
}