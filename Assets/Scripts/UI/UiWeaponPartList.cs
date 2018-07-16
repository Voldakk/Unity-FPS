using UnityEngine;
using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;

using Voldakk.DragAndDrop;

public class UiWeaponPartList : DragAndDropContainer
{
    public GameObject itemPanelPrefab;
    List<UiWeaponPartPanel> itemPanels;

    PlayerData playerData;

    public Transform panelParent;

    protected override void Start()
    {
        base.Start();

        playerData = PlayerData.instance;

        itemPanels = new List<UiWeaponPartPanel>();

        UpdatePanels();

        GetComponent<UiWeaponPartPanel>().SetIndeces(containerIndex, new int[] { -1 });
    }

    public override object GetObject(int[] indices, bool isFromContainer)
    {
        if (indices.Length != 1)
            return null;

        return playerData.GetPart(indices[0]);
    }

    public override bool RecieveObject(object o, int[] indices)
    {
        if (o == null || !(o is WeaponPart))
            return false;

        playerData.AddPart(o as WeaponPart);
        UpdatePanels();

        return true;
    }

    public override void RemoveObject(int[] indices)
    {
        if (indices.Length != 1)
            return;

        playerData.RemovePart(indices[0]);
        UpdatePanels();
    }

    public override void OnObjectMouseDown(PointerEventData.InputButton button, int[] indices)
    {
        
    }

    public void UpdatePanels()
    {
        List<WeaponPart> parts = playerData.GetParts();

        for (int i = 0; i < parts.Count; i++)
        {
            if (i >= itemPanels.Count)
            {
                itemPanels.Add(Instantiate(itemPanelPrefab, panelParent).GetComponent<UiWeaponPartPanel>());
                itemPanels[i].SetIndeces(containerIndex, new int[] { i });
            }

            itemPanels[i].gameObject.SetActive(true);
            itemPanels[i].SetObject(parts[i]);
        }

        for (int i = parts.Count; i < itemPanels.Count; i++)
        {
            itemPanels[i].gameObject.SetActive(false);
        }
    }
}