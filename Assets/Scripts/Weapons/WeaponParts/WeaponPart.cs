using UnityEngine;

using Voldakk.DragAndDrop;

public enum WeaponPartStats { Damage, Accuracy, Recoil, FireRate, MagSize, ReloadTime, Zoom, AdsTime, Pitch, Volume};
public enum StatsType { Base, Additive, Modifier };

[System.Serializable]
public class WeaponPartStat
{
    public WeaponPartStats stats;
    public StatsType statsType;
    public AnimationCurve valueCurve = new AnimationCurve(new Keyframe[] { new Keyframe(1, 0), new Keyframe(50, 1) });
    public bool isInt;
    public bool lowIsPositive;

    public bool capMinValue;
    public float minValue;

    public bool capMaxValue;
    public float maxValue;

    public float GetValue(WeaponPart part)
    {
        return GetValue(part.level, part.quality.modifier);
    }

    public float GetValue(int level, float modifier)
    {
        float value = 0f;

        if (lowIsPositive)
            value = ApplyLowIsPosModifier(valueCurve.Evaluate(level), modifier);
        else
            value = ApplyModifier(valueCurve.Evaluate(level), modifier);

        if (capMinValue)
            value = Mathf.Max(minValue, value);
        if (capMaxValue)
            value = Mathf.Min(maxValue, value);

        return isInt ? Mathf.RoundToInt(value) : value;
    }

    float ApplyModifier(float value, float modifier)
    {
        if (value < 0)
            return value * (1f - (modifier - 1f));
        else
            return value * modifier;
    }

    float ApplyLowIsPosModifier(float value, float modifier)
    {
        if (value < 0)
            return value * modifier;
        else
            return value * (1f - (modifier - 1f));
    }
}

public enum WeaponPartType { Barrel, Body, Grip, Mag, Sight, Stock, Muzzle }

[System.Serializable]
public class WeaponPart : Item
{
    public string stortCode;
    public GameObject prefab;
    public WeaponPartType partType;

    [HideInInspector]
    public WeaponPartSlot[] slots;

    [HideInInspector]
    public GameObject gameObject;

    [HideInInspector]
    public ModularWeapon weapon;

    // Stats
    public int level = 1;
    public Quality quality;
    public float weight = 0;
    public float price = 0;

    public WeaponPartStat[] stats;


    public int GetStatsI(WeaponPartStats statType, int defaultValue = 0)
    {
        return Mathf.RoundToInt(GetStats(statType, defaultValue));
    }

    public float GetStats(WeaponPartStats statType, float defaultValue = 0f)
    {
        foreach (var stat in stats)
        {
            if (stat.stats == statType)
                return stat.GetValue(level, quality.modifier);
        }

        Debug.LogWarningFormat("WeaponPart::GetStats - Stats of type {0} not found", statType.ToString());
        return defaultValue;
    }

    public void Load(Transform parent)
    {
        if (gameObject != null)
            Destroy(gameObject);

        gameObject = Instantiate(prefab, parent);
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;

        slots = gameObject.GetComponentsInChildren<WeaponPartSlot>();
    }

    public void Unload()
    {
        if (gameObject != null)
            Destroy(gameObject);

        slots = null;
    }

    public float ApplyModifier(float value)
    {
        if (value < 0)
            return value * (1f - (quality.modifier - 1f));
        else
            return value * quality.modifier;
    }

    public float ApplyLowIsPosModifier(float value)
    {
        if (value < 0)
            return value * quality.modifier;
        else
            return value * (1f - (quality.modifier - 1f));
    }
}