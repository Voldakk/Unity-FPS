﻿using UnityEngine;

[CreateAssetMenu(fileName = "New ammo", menuName = "Ammo/SingleProjectile")]
public class SingleProjectile : Projectile
{
    public override void Load(ModularWeapon weapon, Transform barrelEnd)
    {
        base.Load(weapon, barrelEnd);

        lineRendererController.lineRenderer.useWorldSpace = true;
        lineRendererController.lineRenderer.SetPositions(new Vector3[] { Vector3.zero, Vector3.forward * lineLength });
    }

    public override void Fire(ModularWeapon weapon, bool doDamage)
    {
        base.Fire(weapon, doDamage);

        RaycastHit hit;
        if (Physics.Raycast(weapon.camera.transform.position, weapon.camera.transform.forward, out hit))
        {
            lineRendererController.lineRenderer.SetPosition(0, barrelEnd.position);
            lineRendererController.lineRenderer.SetPosition(1, hit.point);
            lineRendererController.Fire(lineTime);

            if (doDamage)
            {
                Health health;

                if(hit.rigidbody == null)
                    health = hit.transform.GetComponent<Health>();
                else
                    health = hit.rigidbody.GetComponent<Health>();

                if (health != null)
                    health.Damage(damage);
            }

            if (hit.rigidbody == null)
            {
                GameObject bulletMark = bulletMarkPool.Get();
                bulletMark.transform.parent = hit.transform;
                bulletMark.transform.position = hit.point + hit.normal * 0.001f;
                bulletMark.transform.rotation = Quaternion.LookRotation(-hit.normal, Vector3.up);
            }
        }
        else
        {
            lineRendererController.lineRenderer.SetPosition(0, barrelEnd.position);
            lineRendererController.lineRenderer.SetPosition(1, barrelEnd.position + weapon.camera.transform.forward * lineLength);
            lineRendererController.Fire(lineTime);
        }

        ApplyRecoil(weapon);
    }
}