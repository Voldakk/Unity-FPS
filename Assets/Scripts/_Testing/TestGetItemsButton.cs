using UnityEngine;

public class TestGetItemsButton : MonoBehaviour
{
    public int numItems = 1;

    UiWeaponPartList uiWeaponPartList;

    private void Start()
    {
        uiWeaponPartList = FindObjectOfType<UiWeaponPartList>();
    }

    public void GetItems()
    {
        for (int i = 0; i < numItems; i++)
        {
            PlayerData.instance.AddPart(WeaponParts.GetRandomPart(PlayerData.instance.level));
        }

        uiWeaponPartList.UpdatePanels();
    }
}
