using UnityEngine;
using UnityEngine.UI;

using Voldakk.DragAndDrop;

public class UiWeaponPartPanel : DragAndDropPanel
{
    public Text partName;
    public Image partIcon;

    WeaponPart part;

    public override void SetObject(object o)
    {
        if (o == null || !(o is WeaponPart))
        {
            if(partName != null)
                partName.text = "None";

            if (partIcon != null)
                partIcon.sprite = null;
        }
        else
        {
            part = o as WeaponPart;

            if (partName != null)
                partName.text = part.partName;

            if (partIcon != null)
                partIcon.sprite = part.icon;
        }
    }

    public void MouseEnter()
    {
        FindObjectOfType<UiWeaponPartHoverPanel>().Show(part);
    }

    public void MouseExit()
    {
        FindObjectOfType<UiWeaponPartHoverPanel>().Hide();
    }
}