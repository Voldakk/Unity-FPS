using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WeaponPartData
{
    [SerializeField]
    public string partShortCode;

    [SerializeField]
    public List<WeaponPartData> dataParts;
}

public class PlayerData : MonoBehaviour
{
    public PlayerData instance;

    string weaponSave;

	void Awake ()
    {
        instance = this;
	}

    public void LoadFromFile()
    {

    }

    public void SaveToFile()
    {
        
    }

    public void SaveWeapon(ModularWeapon weapon)
    {
        weaponSave = WeaponToJson(weapon);
    }

    public WeaponPartData saveWpd;

    public string WeaponToJson(ModularWeapon weapon)
    {
        if (weapon == null || weapon.bodySlot.part == null)
            return "{}";

        saveWpd = new WeaponPartData();

        SavePart(ref saveWpd, weapon.bodySlot.part);

        return JsonUtility.ToJson(saveWpd);
    }

    void SavePart(ref WeaponPartData wpd, WeaponPart part)
    {
        wpd.dataParts = new List<WeaponPartData>();

        if (part == null)
        {
            wpd.partShortCode = "";
            return;
        }

        wpd.partShortCode = part.stortCode;

        foreach (WeaponPartSlot slot in part.slots)
        {
            WeaponPartData child = new WeaponPartData();
            wpd.dataParts.Add(child);
            SavePart(ref child, slot.part);
        }
    }

    public WeaponPartData loadWpd;

    public void LoadWeapon(ModularWeapon weapon)
    {
        Debug.Log(weaponSave);
        loadWpd = JsonUtility.FromJson<WeaponPartData>(weaponSave);

        LoadPart(ref weapon.bodySlot, loadWpd);
    }

    //public WeaponPart loadedPart;

    void LoadPart(ref WeaponPartSlot slot, WeaponPartData wpd)
    {
        if (wpd.partShortCode == "")
            return;

        Debug.Log("Loading " + wpd.partShortCode);

        WeaponPart loadedPart = Resources.Load<WeaponPart>("WeaponParts/" + wpd.partShortCode);
        slot.SetPart(loadedPart);

        for (int i = 0; i < wpd.dataParts.Count; i++)
        {
            Debug.Log(wpd.partShortCode + " child " + i);
            WeaponPartSlot childSlot = loadedPart.slots[i];
            WeaponPartData childData = wpd.dataParts[i];
            LoadPart(ref childSlot, childData);
        }

        Debug.Log("Loaded " + wpd.partShortCode);
    }
}