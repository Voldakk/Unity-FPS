using UnityEngine;
using System.Collections;

public class BodyBehaviour : WeaponPartBehaviour
{
    Body data;

    bool ready = true;

    BarrelBehaviour barrel;
    MagBehaviour mag;

    public override void SetPart(WeaponPart part)
    {
        base.SetPart(part);

        data = part as Body;
    }

    public void OnStart()
    {
        barrel = GetComponentInChildren<BarrelBehaviour>();
        if (barrel != null)
            barrel.OnStart();

        mag = GetComponentInChildren<MagBehaviour>();
        if (mag != null)
            mag.OnStart();

        if (mag != null && barrel != null)
        {
            mag.SetupAmmo(barrel.GetBarrelEnd());
        }
    }

    public virtual void Fire(bool doDamage)
    {
        if (barrel == null || mag == null)
            return;

        if (mag.CurrentAmmo > 0 && ready)
        {
            mag.Fire();
            barrel.Fire(mag.Ammo, doDamage);

            weapon.StartCoroutine(Cycle());
        }
    }

    public void Reload()
    {
        if (mag != null)
            mag.StartReload();
    }

    IEnumerator Cycle()
    {
        ready = false;
        yield return new WaitForSeconds(60.0f / data.fireRate);
        ready = true;
    }
}