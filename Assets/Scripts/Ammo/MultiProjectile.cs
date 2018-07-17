using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New ammo", menuName = "Ammo/MultiProjectile")]
public class MultiProjectile : Projectile
{
    public int numProjectiles;

    public float coneRadius;
    public float coneLength;

    public override void Fire(ModularWeapon weapon, Barrel barrel, bool doDamage)
    {
        base.Fire(weapon, barrel, doDamage);

        LineRenderer lr = lineRendererController.lineRenderer;
        lr.positionCount = numProjectiles * 2;

        Dictionary<Health, float> damageTable= new Dictionary<Health, float>();

        for (int i = 0; i < numProjectiles; i++)
        {
            float randomRadius = Random.Range(0, coneRadius);
            float randomAngle = Random.Range(0, 2 * Mathf.PI);

            Vector3 direction = new Vector3(
                randomRadius * Mathf.Cos(randomAngle),
                randomRadius * Mathf.Sin(randomAngle),
                coneLength
            ).normalized;

            lr.SetPosition(i * 2 + 0, Vector3.zero);

            lr.SetPosition(i * 2 + 1, direction * 1000.0f);

            if (!doDamage)
                continue;

            RaycastHit hit;
            Vector3 origin = weapon.camera.transform.position;
            Vector3 worldDirection = weapon.camera.transform.TransformDirection(direction);
            if (Physics.Raycast(origin, worldDirection, out hit))
            {
                if (hit.rigidbody == null)
                {
                    GameObject bulletMark = bulletMarkPool.Get();
                    bulletMark.transform.position = hit.point + hit.normal * 0.001f;
                    bulletMark.transform.rotation = Quaternion.LookRotation(-hit.normal, Vector3.up);
                }
                else if (doDamage)
                {
                    Health health = hit.rigidbody.GetComponent<Health>();
                    if (health != null)
                        health.Damage(damage);
                }
            }
        }

        foreach (Health health in damageTable.Keys)
        {
            health.Damage(damageTable[health]);
        }

        lineRendererController.Fire(lineTime);
        ApplyRecoil(weapon);
    }
}