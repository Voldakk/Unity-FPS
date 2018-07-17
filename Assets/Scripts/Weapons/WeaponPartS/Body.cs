using UnityEngine;
using System.Linq;
using System.Collections;

public class Body : WeaponPart
{
    public float fireRate;

    Barrel barrel;
    Mag mag;

    bool ready = true;

    public override void OnStart(ModularWeapon weapon)
    {
        base.OnStart(weapon);

        var slots = gameObject.GetComponentsInChildren<WeaponPartSlot>().Where(s => s.transform.parent = gameObject.transform);

        if (slots == null)
            return;

        barrel = slots.SingleOrDefault(s => s.slotType == WeaponPartType.Barrel).part as Barrel;
        if (barrel != null)
            barrel.OnStart(weapon);

        mag = slots.SingleOrDefault(s => s.slotType == WeaponPartType.Mag).part as Mag;
        if (mag != null)
            mag.OnStart(weapon);

        if(mag != null && barrel != null)
        {
            mag.ammo = Instantiate(mag.ammo);
            mag.ammo.Load(weapon, barrel);
        }
    }

    public virtual void Fire(bool doDamage)
    {
        if (barrel == null || mag == null)
            return;

        if(mag.CurrentAmmo > 0 && ready)
        {
            mag.Fire();
            barrel.Fire(mag.ammo, doDamage);

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
        yield return new WaitForSeconds(60.0f / fireRate);
        ready = true;
    }
}