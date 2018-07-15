using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class UiSaveLoadWeapon : MonoBehaviour
{

    public InputField weaponNameInput;

    PlayerData playerData;

    public GameObject weaponPanelPrefab;

    public ModularWeapon weapon;

    List<WeaponData> weapons;
    int selectedWeapon = -1;

    List<GameObject> weaponPanels;

	void Start ()
    {
        playerData = PlayerData.instance;

        weaponPanels = new List<GameObject>();

        WeaponData[] wd = playerData.LoadWeaponsFromFile();
        if (wd == null)
            weapons = new List<WeaponData>();
        else 
            weapons = wd.ToList();

        if(weapons.Count == 0)
        {
            New();
        }
        else
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                int index = i;
                GameObject go = Instantiate(weaponPanelPrefab, transform);
                go.GetComponent<Button>().onClick.AddListener(() => { OnWeaponPanelClick(go); });
                go.GetComponentInChildren<Text>().text = weapons[i].weaponName;

                weaponPanels.Add(go);
            }

            SetSelected(0);
        }
    }

    public void New()
    {
        weapon.bodySlot.SetPart(null);
        
        weapon.weaponName = "New Weapon";
        weaponNameInput.text = weapon.weaponName;

        AddNew(playerData.GetWeaponData(weapon));

        UiWeaponPartSlotContainer uiSlot = weapon.bodySlot.GetComponentInChildren<UiWeaponPartSlotContainer>();
        if (uiSlot != null)
        {
            uiSlot.RemoveObject(new int[] { 0 });
        }
    }

    public void Save()
    {
        if (!string.IsNullOrEmpty(weaponNameInput.text))
        {
            weapon.weaponName = weaponNameInput.text;
            weapons[selectedWeapon] = playerData.GetWeaponData(weapon);

            playerData.SaveWeaponsToFile(weapons.ToArray());

            weaponPanels[selectedWeapon].GetComponentInChildren<Text>().text = weapons[selectedWeapon].weaponName;
        }
    }

    public void Copy()
    {
        WeaponData copy = playerData.GetWeaponData(weapon);
        copy.weaponName += " Copy";
        AddNew(copy);

        Save();
    }

    public void Load()
    {
        playerData.LoadWeaponFromData(weapon, weapons[selectedWeapon]);
        weaponNameInput.text = weapon.weaponName;

        UiWeaponPartSlotPanel uiSlot = weapon.bodySlot.GetComponentInChildren<UiWeaponPartSlotPanel>();
        if (uiSlot != null)
        {
            uiSlot.SetObject(weapon.bodySlot.part);
        }
    }

    public void SetSelected(int index)
    {
        if (selectedWeapon >= 0 && selectedWeapon < weaponPanels.Count)
            weaponPanels[selectedWeapon].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.4f);

        selectedWeapon = index;
        Load();

        weaponPanels[selectedWeapon].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
    }

    public void AddNew(WeaponData weaponData)
    {
        weapons.Add(weaponData);
        int index = weapons.Count - 1;

        GameObject go = Instantiate(weaponPanelPrefab, transform);
        go.GetComponent<Button>().onClick.AddListener(() => { OnWeaponPanelClick(go);  });
        go.GetComponentInChildren<Text>().text = weaponData.weaponName;

        weaponPanels.Add(go);

        SetSelected(index);
    }

    public void OnWeaponPanelClick(GameObject sender)
    {
        SetSelected(weaponPanels.IndexOf(sender));
    }

    public void Delete()
    {
        if (weapons.Count <= 1)
            return;

        weapons.RemoveAt(selectedWeapon);
        Destroy(weaponPanels[selectedWeapon]);
        weaponPanels.RemoveAt(selectedWeapon);

        playerData.SaveWeaponsToFile(weapons.ToArray());

        if (selectedWeapon == weapons.Count)
            SetSelected(selectedWeapon - 1);
    }
}