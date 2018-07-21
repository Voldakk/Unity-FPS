using UnityEngine;

using System.Collections.Generic;

public class WeaponParts : MonoBehaviour
{
    static WeaponParts instance;

    WeaponPart[] parts;
    Dictionary<string, WeaponPart> shortCodes;

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
        part.level = level;
        part.quality = Qualities.GetRandomQuality();

        return part;
    }
}