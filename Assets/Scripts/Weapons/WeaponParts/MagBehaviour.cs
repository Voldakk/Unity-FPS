using UnityEngine;
using System.Collections;

public class MagBehaviour : WeaponPartBehaviour
{
    Mag data;
    bool isReloading;

    public Ammo Ammo { get; private set; }

    public int CurrentAmmo { get; private set; }

    public override void SetPart(WeaponPart part)
    {
        base.SetPart(part);
        data = part as Mag;
    }

    public void OnStart()
    {
        EndReload();
    }

    public void SetupAmmo(Transform barrelEnd)
    {
        Ammo = Instantiate(data.ammo);
        Ammo.Load(weapon, barrelEnd);
    }

    public void Fire()
    {
        CurrentAmmo--;
    }

    public void StartReload()
    {
        if (isReloading)
            return;

        CurrentAmmo = 0;
        isReloading = true;
        StartCoroutine(Reload());
    }   
    public void EndReload()
    {
        CurrentAmmo = data.magSize;
        isReloading = false;
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(data.reloadTime);
        EndReload();
    }
}
