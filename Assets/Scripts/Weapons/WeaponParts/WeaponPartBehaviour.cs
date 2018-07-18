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
        SetPart(transform.parent.GetComponent<WeaponPartSlot>().part);
    }

    public virtual void SetPart(WeaponPart part)
    {

    }
}