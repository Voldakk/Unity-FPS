using UnityEngine;
using UnityEngine.UI;

public class UiWeaponSelect : MonoBehaviour
{
    public GameObject weaponPanelPrefab;
    public Transform weaponList;

    Weapon[] weapons;

	void Start ()
    {
        weapons = Resources.LoadAll<Weapon>("Weapons");
        Debug.Log("Count: " + weapons.Length);
        for (int i = 0; i < weapons.Length; i++)
        {
            GameObject go = Instantiate(weaponPanelPrefab, weaponList);
            Image image = go.transform.Find("Image").GetComponent<Image>();
            image.sprite = weapons[i].icon;
            image.type = Image.Type.Simple;
            image.preserveAspect = true;

            int index = i;
            go.GetComponent<Button>().onClick.AddListener(delegate{ OnButtonPressed(index); });
        }
	}

    public void OnButtonPressed(int index)
    {
        Debug.Log("Clicked " + index);

        PlayerSetting.Current.startingWeapon = index;
    }
}
