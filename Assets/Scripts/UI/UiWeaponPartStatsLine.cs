using UnityEngine;
using UnityEngine.UI;

public class UiWeaponPartStatsLine : MonoBehaviour
{
    public Text statsName, statsValue;

    public void Set(string name, string value)
    {
        statsName.text = name;
        statsValue.text = value;
    }
}