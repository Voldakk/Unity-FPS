using UnityEngine;
using System.Collections;

public class MagBehaviour : WeaponPartBehaviour
{
    Mag data;
    bool isReloading;

    AudioSource audioSource;

    public Ammo Ammo { get; private set; }

    int magSize;
    public int CurrentAmmo { get; private set; }

    float reloadTime;

    public override void SetPart(WeaponPart part)
    {
        base.SetPart(part);
        data = part as Mag;

        magSize = data.GetStatsI(WeaponPartStats.MagSize, 1);
        reloadTime = data.GetStats(WeaponPartStats.ReloadTime, 1f);
    }

    public void OnStart()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = data.reloadSound;
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

        audioSource.Play();
    }   
    public void EndReload()
    {
        CurrentAmmo = magSize;
        isReloading = false;
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime);
        EndReload();
    }
}
