using UnityEngine;
using UnityEngine.UI;

using Voldakk.DragAndDrop;

public class UiWeaponPartPanel : DragAndDropPanel
{
    public Text partName;
    public Image partIcon;

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
            WeaponPart part = o as WeaponPart;

            if (partName != null)
                partName.text = part.partName;

            if (partIcon != null)
                partIcon.sprite = part.icon;
        }
    }
}