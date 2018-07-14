using UnityEngine;
using UnityEngine.UI;
using Voldakk.DragAndDrop;

public class UiWeaponPartSlotPanel : DragAndDropPanel
{
    public Text slotName, partName;
    public Image partIcon;

    public override void SetObject(object o)
    {
        if (o == null || !(o is WeaponPart))
        {
            partName.text = "Empty";
            partIcon.sprite = null;
        }
        else
        {
            WeaponPart part = o as WeaponPart;

            partName.text = part.partName;
            partIcon.sprite = part.icon;
            
        }
    }
}