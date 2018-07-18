using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BarrelBehaviour : WeaponPartBehaviour
{
    Barrel data;

    [HideInNormalInspector]
    public MuzzleBehaviour muzzle;
    Transform barrelEnd;

    public override void SetPart(WeaponPart part)
    {
        base.SetPart(part);

        data = part as Barrel;
    }

    protected override void Awake()
    {
        base.Awake();

        barrelEnd = transform.Find("BarrelEnd");
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
