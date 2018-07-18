using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BarrelBehaviour : WeaponPartBehaviour
{
    Barrel data;

    public Transform barrelEnd;

    [HideInNormalInspector]
    public MuzzleBehaviour muzzle;

    public override void SetPart(WeaponPart part)
    {
        base.SetPart(part);

        data = part as Barrel;
    }

    public void OnStart()
    {
        muzzle = GetComponentInChildren<MuzzleBehaviour>();
        if (muzzle != null)
        {
            barrelEnd = muzzle.gameObject.transform.Find("BarrelEnd");
        }
    }

    public void Fire(Ammo ammo, bool doDamage)
    {
        ammo.Fire(weapon, doDamage);
    }

    public Transform GetBarrelEnd()
    {
        return muzzle == null ? barrelEnd : muzzle.GetBarrelEnd();
    }
}
