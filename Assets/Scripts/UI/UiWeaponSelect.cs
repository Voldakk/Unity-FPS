using UnityEngine;
using UnityEngine.UI;

public class UiWeaponSelect : MonoBehaviour
{
    public GameObject weaponPanelPrefab;
    public Transform weaponList;
	void Start ()
    {
        var weapons = PlayerData.instance.GetWeapons();
        Debug.Log("Count: " + weapons.Count);
        for (int i = 0; i < weapons.Count; i++)
        {
            GameObject go = Instantiate(weaponPanelPrefab, weaponList);
            Text weaponNameText = go.transform.Find("WeaponName").GetComponent<Text>();
            weaponNameText.text = weapons[i].weaponName;

            int index = i;
            go.GetComponent<Button>().onClick.AddListener(delegate{ OnButtonPressed(index); });
        }
	}

    public void OnButtonPressed(int index)
    {
        Debug.Log("Clicked " + index);

        PlayerData.instance.currentWeapon = index;
    }
}
