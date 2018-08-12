using UnityEngine;

using System.Collections.Generic;

public class WeaponParts : MonoBehaviour
{
    static WeaponParts instance;

    WeaponPart[] parts;
    Dictionary<string, WeaponPart> shortCodes;

    public AnimationCurve relativeLootLevel = new AnimationCurve(new Keyframe[] { new Keyframe(0, -2f), new Keyframe(1, 2f) });

    public AnimationCurve recoilCurve;

    void Awake ()
    {
        instance = this;
        parts = Resources.LoadAll<WeaponPart>("WeaponParts/");

        shortCodes = new Dictionary<string, WeaponPart>();
        foreach (var p in parts)
        {
            shortCodes.Add(p.stortCode, p);
        }
    }

    public static WeaponPart[] GetAllParts()
    {
        return instance.parts;
    }

    public static WeaponPart GetPart(string shortCode)
    {
        return Instantiate(instance.shortCodes[shortCode]);
    }

    public static WeaponPart GetRandomPart(int level)
    {
        var part = Instantiate(instance.parts[Random.Range(0, instance.parts.Length)]);
        part.level = Mathf.Max (1, level + Mathf.RoundToInt(instance.relativeLootLevel.Evaluate(Random.value)));
        part.quality = Qualities.GetRandomQuality();

        return part;
    }

    public static float GetRecoil(float recoil)
    {
        return instance.recoilCurve.Evaluate(recoil);
    }
}