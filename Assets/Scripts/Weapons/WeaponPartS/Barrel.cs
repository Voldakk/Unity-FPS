using UnityEngine;
using System.Linq;

public class Barrel : WeaponPart
{
    public float accuracy;
    public float 
        recoilModifier, 
        damageModifier;

    Muzzle muzzle;
    [HideInInspector]
    public Transform barrelEnd;

    public override void OnStart(ModularWeapon weapon)
    {
        base.OnStart(weapon);

        var slots = gameObject.GetComponentsInChildren<WeaponPartSlot>().Where(s => s.transform.parent = gameObject.transform);

        if (slots == null)
            return;

        var slot = slots.SingleOrDefault(s => s.slotType == WeaponPartType.Muzzle);

        if(slot != null && slot.part != null)
            muzzle = slot.part as Muzzle;

        if (muzzle != null)
        {
            muzzle.OnStart(weapon);
            barrelEnd = muzzle.gameObject.transform.Find("BarrelEnd");
        }
        else
        {
            barrelEnd = gameObject.transform.Find("BarrelEnd");
        }
    }

    public virtual void Fire(Ammo ammo, bool doDamage)
    {
        ammo.Fire(weapon, this, doDamage);
    }
}