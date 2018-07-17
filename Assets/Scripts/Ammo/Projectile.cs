using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Ammo
{
    public float damage;
    public float lineTime = 0.01f;
    public float lineLength = 1000.0f;

    public float verticalRecoil;
    public float horizontalRecoil;

    public GameObject lineRendererPrefab;
    protected LineRendererController lineRendererController;

    public string bulletMarkPoolName;
    protected Pool bulletMarkPool;

    protected Transform barrelEnd;

    public override void Load(ModularWeapon weapon, Transform barrelEnd)
    {
        base.Load(weapon, barrelEnd);

        this.barrelEnd = barrelEnd;

        lineRendererController = Instantiate(lineRendererPrefab, barrelEnd).GetComponent<LineRendererController>();

        bulletMarkPool = GameObject.Find(bulletMarkPoolName).GetComponent<Pool>();
    }

    public override void Unload()
    {
        if (lineRendererController != null)
            Destroy(lineRendererController.gameObject);

        base.Unload();
    }

    public override void Fire(ModularWeapon weapon, bool doDamage)
    {
        base.Fire(weapon, doDamage);
    }

    protected void ApplyRecoil(ModularWeapon weapon)
    {
        weapon.weaponBehaviour.transform.localRotation *= Quaternion.Euler(0f, Random.Range(-horizontalRecoil, horizontalRecoil), 0f);
        weapon.camera.transform.localRotation *= Quaternion.Euler(-verticalRecoil, 0f, 0f);
    }
}