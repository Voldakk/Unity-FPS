using UnityEngine;
using System.Linq;

public class ModularWeapon : MonoBehaviour
{
    public WeaponPart body, barrel, stock, grip, mag;

    public int numSlots = 5;

    public WeaponPart GetPart(int i)
    {
        switch (i)
        {
            case 0:
                return body;
            case 1:
                return barrel;
            case 2:
                return stock;
            case 3:
                return grip;
            case 4:
                return mag;
        }

        return null;
    }

    public bool SetPart(int i, WeaponPart part)
    {
        switch (i)
        {
            case 0:
                return SetPart(ref body, part, transform);

            case 1:
                return SetPart(ref barrel, part, body);

            case 2:
                return SetPart(ref stock, part, body);

            case 3:
                return SetPart(ref grip, part, body);

            case 4:
                return SetPart(ref mag, part, body);
        }

        return false;
    }

    bool SetPart(ref WeaponPart oldPart, WeaponPart newPart, WeaponPart parent)
    {
        if (parent == null)
            return false;

        if (newPart == null)
        {
            return SetPart(ref oldPart, newPart, (Transform)null);
        }
        else
        {
            WeaponPartSlot slot = parent.slots.Single(s => s.slotType == newPart.partType);
            return SetPart(ref oldPart, newPart, slot.transform);
        }
    }

    bool SetPart(ref WeaponPart oldPart, WeaponPart newPart, Transform parent)
    {
        if (oldPart != null)
            oldPart.Unload();

        if (newPart != null)
            newPart.Load(parent);

        oldPart = newPart;

        return true;
    }
}
