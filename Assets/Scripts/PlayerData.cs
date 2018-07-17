using UnityEngine;

using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class WeaponPartData
{
    public string partShortCode;

    public WeaponPartData[] dataParts;
}

[System.Serializable]
public class WeaponData
{
    public string weaponName;
    public WeaponPartData rootPart;
}

public class PlayerData : MonoBehaviour
{
    public static PlayerData instance;

    public List<WeaponPart> defaultWeaponParts;
    private List<WeaponPart> weaponParts;

    public List<WeaponData> defaultWeapons;
    private List<WeaponData> weapons;

    public int currentWeapon;

    void Awake ()
    {
        instance = this;

        var fileParts = LoadPartsFromFile();

        if (fileParts == null)
            weaponParts = defaultWeaponParts;
        else
            weaponParts = fileParts.ToList();


        var fileWeapons = LoadWeaponsFromFile();

        if (fileWeapons == null)
            weapons = defaultWeapons;
        else
            weapons = fileWeapons.ToList();
    }

    public void Save()
    {
        SavePartsToFile();
        SaveWeaponsToFile();
    }

    public void SaveToFile(object o, string path)
    {
        var bf = new BinaryFormatter();
        var file = File.Create(Application.persistentDataPath + "/" + path);

        bf.Serialize(file, o);
        file.Close();
    }

    public T LoadFromFile<T>(string path) where T : class
    {
        if (File.Exists(Application.persistentDataPath + "/" + path))
        {
            var bf = new BinaryFormatter();
            var file = File.Open(Application.persistentDataPath + "/" + path, FileMode.Open);

            T o = (T)bf.Deserialize(file);
            file.Close();

            return o;
        }

        return null;
    }

    void SavePartsToFile()
    {
        var shortCodes = weaponParts.Select(wp => wp.stortCode).ToArray();

        SaveToFile(shortCodes, "parts");
    }

    public WeaponPart[] LoadPartsFromFile()
    {
        var shortCodes = LoadFromFile<string[]>("parts");

        if (shortCodes == null)
            return null;

        var parts = new WeaponPart[shortCodes.Length];

        for (int i = 0; i < shortCodes.Length; i++)
        {
            parts[i] = Resources.Load<WeaponPart>("WeaponParts/" + shortCodes[i]);
        }

        return parts;
    }

    public WeaponPart GetPart(int index)
    {
        if (index >= 0 && index < weaponParts.Count)
            return weaponParts[index];

        return null;
    }

    public void AddPart(WeaponPart weaponPart)
    {
        if (weaponPart != null)
        {
            weaponParts.Add(weaponPart);

            weaponParts = weaponParts.OrderBy(p => p.stortCode).ToList();
        }
    }

    public void RemovePart(int index)
    {
        if (index >= 0 && index < weaponParts.Count)
            weaponParts.RemoveAt(index);
    }

    public WeaponPart RemovePart(WeaponPart weaponPart)
    {
        var part = weaponParts.Single(p => p.stortCode == weaponPart.stortCode);

        if (part != null)
            weaponParts.Remove(part);

        return part;
    }

    public List<WeaponPart> GetParts()
    {
        return weaponParts;
    }

    public List<WeaponData> GetWeapons()
    {
        return weapons;
    }

    public void SaveWeapon(int index, ModularWeapon weapon)
    {
        weapons[index] = GetWeaponData(weapon);
    }

    void SaveWeaponsToFile()
    {
        SaveToFile(weapons.ToArray(), "weapons");
    }

    public void LoadWeapon(int index, ModularWeapon weapon)
    {
        LoadWeaponFromData(weapon, weapons[index]);
    }

    public void AddWeapon(WeaponData weaponData)
    {
        weapons.Add(weaponData);
    }

    public void RemoveWeapon(int index)
    {
        weapons.RemoveAt(index);
    }

    public int WeaponCount()
    {
        return weapons.Count;
    }

    public WeaponData[] LoadWeaponsFromFile()
    {
        return LoadFromFile<WeaponData[]>("weapons");
    }

    public string WeaponToJson(ModularWeapon weapon)
    {
        if (weapon == null || weapon.bodySlot.part == null)
            return "{}";

        return JsonUtility.ToJson(GetWeaponData(weapon));
    }

    public WeaponData GetWeaponData(ModularWeapon weapon)
    {
        var weaponData = new WeaponData
        {
            weaponName = weapon.weaponName
        };

        var wpd = new WeaponPartData();
        weaponData.rootPart = wpd;

        SavePart(ref wpd, weapon.bodySlot.part);

        return weaponData;
    }

    void SavePart(ref WeaponPartData wpd, WeaponPart part)
    {
        if (part == null)
        {
            wpd.partShortCode = "";
            return;
        }

        wpd.partShortCode = part.stortCode;
        wpd.dataParts = new WeaponPartData[part.slots.Length];

        for (int i = 0; i < part.slots.Length; i++)
        {
            wpd.dataParts[i] = new WeaponPartData();
            SavePart(ref wpd.dataParts[i], part.slots[i].part);
        }
    }

    public void JsonToWeapon(ModularWeapon weapon, string json)
    {
        var weaponData = JsonUtility.FromJson<WeaponData>(json);

        LoadWeaponFromData(weapon, weaponData);
    }

    void LoadPart(ref WeaponPartSlot slot, WeaponPartData wpd)
    {
        if (wpd.partShortCode == "")
            return;

        var loadedPart = Instantiate(Resources.Load<WeaponPart>("WeaponParts/" + wpd.partShortCode));
        slot.SetPart(loadedPart);

        for (int i = 0; i < wpd.dataParts.Length; i++)
        {
            var childSlot = loadedPart.slots[i];
            var childData = wpd.dataParts[i];
            LoadPart(ref childSlot, childData);
        }
    }

    public void LoadWeaponFromData(ModularWeapon weapon, WeaponData weaponData)
    {
        weapon.weaponName = weaponData.weaponName;

        LoadPart(ref weapon.bodySlot, weaponData.rootPart);
    }

    public void ClearSlot(WeaponPartSlot slot, bool root = true)
    {
        if (slot == null || slot.part == null)
            return;

        foreach (WeaponPartSlot s in slot.part.slots)
        {
            ClearSlot(s, false);
        }

        if (!root)
            AddPart(slot.part);

        slot.SetPart(null);
    }
}