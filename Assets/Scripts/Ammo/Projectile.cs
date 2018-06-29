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

    public override void Load(Gun gun)
    {
        base.Load(gun);

        lineRendererController = Instantiate(lineRendererPrefab, gun.weaponObject.transform.Find("BarrelEnd")).GetComponent<LineRendererController>();

        bulletMarkPool = GameObject.Find(bulletMarkPoolName).GetComponent<Pool>();
    }

    public override void Unload()
    {
        if (lineRendererController != null)
            Destroy(lineRendererController.gameObject);

        base.Unload();
    }

    public override void Fire(Gun gun, bool doDamage)
    {
        base.Fire(gun, doDamage);
    }

    protected void ApplyRecoil(Gun gun)
    {
        gun.transform.localRotation *= Quaternion.Euler(0f, Random.Range(-horizontalRecoil, horizontalRecoil), 0f);
        gun.camera.transform.localRotation *= Quaternion.Euler(-verticalRecoil, 0f, 0f);
    }
}