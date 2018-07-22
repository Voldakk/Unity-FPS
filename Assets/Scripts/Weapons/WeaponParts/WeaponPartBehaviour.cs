using UnityEngine;
using System.Linq;

public class WeaponPartBehaviour : MonoBehaviour
{
    [HideInInspector]
    public WeaponPartSlot[] slots;

    [HideInInspector]
    public ModularWeapon weapon;

	protected virtual void Awake ()
    {
        weapon = GetComponentInParent<ModularWeapon>();
        slots = gameObject.GetComponentsInChildren<WeaponPartSlot>().Where(s => s.transform.parent = transform).ToArray();

        if (transform.parent != null)
        {
            var slot = transform.parent.GetComponent<WeaponPartSlot>();
            if (slot != null)
                SetPart(slot.part);
        }
    }

    public virtual void SetPart(WeaponPart part)
    {

    }
}