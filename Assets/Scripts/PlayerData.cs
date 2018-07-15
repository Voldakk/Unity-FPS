using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

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

	void Awake ()
    {
        instance = this;
	}

    public void SaveWeaponsToFile(WeaponData[] weapons)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/Weapons.dat");

        bf.Serialize(file, weapons);
        file.Close();
    }

    public WeaponData[] LoadWeaponsFromFile()
    {
        WeaponData[] weapons;

        if (File.Exists(Application.persistentDataPath + "/Weapons.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Weapons.dat", FileMode.Open);

            weapons = (WeaponData[])bf.Deserialize(file);
            file.Close();

            return weapons;
        }

        return null;
    }

    public string WeaponToJson(ModularWeapon weapon)
    {
        if (weapon == null || weapon.bodySlot.part == null)
            return "{}";

        return JsonUtility.ToJson(GetWeaponData(weapon));
    }

    public WeaponData GetWeaponData(ModularWeapon weapon)
    {
        WeaponData weaponData = new WeaponData
        {
            weaponName = weapon.weaponName
        };

        WeaponPartData wpd = new WeaponPartData();
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
        WeaponData weaponData = JsonUtility.FromJson<WeaponData>(json);

        LoadWeaponFromData(weapon, weaponData);
    }

    void LoadPart(ref WeaponPartSlot slot, WeaponPartData wpd)
    {
        if (wpd.partShortCode == "")
            return;

        WeaponPart loadedPart = Resources.Load<WeaponPart>("WeaponParts/" + wpd.partShortCode);
        slot.SetPart(loadedPart);

        for (int i = 0; i < wpd.dataParts.Length; i++)
        {
            WeaponPartSlot childSlot = loadedPart.slots[i];
            WeaponPartData childData = wpd.dataParts[i];
            LoadPart(ref childSlot, childData);
        }
    }

    public void LoadWeaponFromData(ModularWeapon weapon, WeaponData weaponData)
    {
        weapon.weaponName = weaponData.weaponName;

        LoadPart(ref weapon.bodySlot, weaponData.rootPart);
    }
}