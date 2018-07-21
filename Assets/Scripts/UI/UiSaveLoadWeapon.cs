using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

public class UiSaveLoadWeapon : MonoBehaviour
{
    public InputField weaponNameInput;

    PlayerData playerData;

    public GameObject weaponPanelPrefab;

    public ModularWeapon weapon;

    int selectedWeapon = -1;

    List<GameObject> weaponPanels;

    public UiWeaponPartList uiWeaponPartList;

	void Start ()
    {
        playerData = PlayerData.instance;

        weaponPanels = new List<GameObject>();


        var weapons = playerData.GetWeapons();

        if(weapons.Count == 0)
        {
            New();
        }
        else
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                var go = Instantiate(weaponPanelPrefab, transform);
                go.GetComponent<Button>().onClick.AddListener(() => { OnWeaponPanelClick(go); });
                go.GetComponentInChildren<Text>().text = weapons[i].weaponName;

                weaponPanels.Add(go);
            }

            SetSelected(0);
        }
    }

    public void New()
    {
        Save();

        var wd = playerData.GetWeaponData(weapon);
        wd.rootPart.partShortCode = "";
        wd.weaponName = "New weapon";
        AddNew(wd);

        Save();
    }

    public void Delete()
    {
        if (playerData.WeaponCount() <= 1)
            return;

        playerData.ClearSlot(weapon.bodySlot, false);

        playerData.RemoveWeapon(selectedWeapon);
        Destroy(weaponPanels[selectedWeapon]);
        weaponPanels.RemoveAt(selectedWeapon);

        if (selectedWeapon >= playerData.WeaponCount())
            SetSelected(playerData.WeaponCount() - 1, false);
        else
            SetSelected(selectedWeapon, false);

        uiWeaponPartList.UpdatePanels();
    }

    public void Copy()
    {
        /*Save();

        WeaponData copy = playerData.GetWeaponData(weapon);
        copy.weaponName += " Copy";
        AddNew(copy);

        Save();

        playerData.SaveWeaponsToFile();*/
    }

    public void Save()
    {
        if (!string.IsNullOrEmpty(weaponNameInput.text))
        {
            weapon.weaponName = weaponNameInput.text;

            playerData.SaveWeapon(selectedWeapon, weapon);

            weaponPanels[selectedWeapon].GetComponentInChildren<Text>().text = weapon.weaponName;
        }
    }

    void Load()
    {
        weapon.bodySlot.SetPart(null);

        playerData.LoadWeapon(selectedWeapon, weapon);
        weaponNameInput.text = weapon.weaponName;

        UiWeaponPartSlotPanel uiSlot = weapon.bodySlot.GetComponentInChildren<UiWeaponPartSlotPanel>();
        if (uiSlot != null)
        {
            uiSlot.SetObject(weapon.bodySlot.part);
        }
    }

    void SetSelected(int index, bool save = true)
    {
        if (selectedWeapon >= 0 && selectedWeapon < weaponPanels.Count)
            weaponPanels[selectedWeapon].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.4f);

        if (save)
            Save();

        selectedWeapon = index;

        Load();

        weaponPanels[selectedWeapon].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
    }

    void AddNew(WeaponData weaponData)
    {
        playerData.AddWeapon(weaponData);
        int index = playerData.WeaponCount() - 1;

        GameObject go = Instantiate(weaponPanelPrefab, transform);
        go.GetComponent<Button>().onClick.AddListener(() => { OnWeaponPanelClick(go);  });
        go.GetComponentInChildren<Text>().text = weaponData.weaponName;

        weaponPanels.Add(go);

        SetSelected(index);
    }

    void OnWeaponPanelClick(GameObject sender)
    {
        SetSelected(weaponPanels.IndexOf(sender));
    }

    public void SaveToFile()
    {
        PlayerData.instance.Save();
    }
}