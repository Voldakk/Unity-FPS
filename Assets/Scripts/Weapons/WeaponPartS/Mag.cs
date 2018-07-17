using System.Collections;
using UnityEngine;

public class Mag : WeaponPart
{
    public int magSize;
    public float reloadTime;
    public float recoilModifier;

    public Ammo ammo;

    public int CurrentAmmo { get; private set; }

    public void Fire()
    {
        CurrentAmmo--;
    }

    public override void OnStart(ModularWeapon weapon)
    {
        base.OnStart(weapon);

        EndReload();
    }

    public void StartReload()
    {
        CurrentAmmo = 0;
        weapon.StartCoroutine(Reload());
    }
    public void EndReload()
    {
        CurrentAmmo = magSize;
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime);
        EndReload();
    }
}