using UnityEngine;

[CreateAssetMenu(fileName = "New ammo", menuName = "Ammo/SingleProjectile")]
public class SingleProjectile : Projectile
{
    public override void Load(Gun gun)
    {
        base.Load(gun);

        lineRendererController.lineRenderer.useWorldSpace = true;
        lineRendererController.lineRenderer.SetPositions(new Vector3[] { Vector3.zero, Vector3.forward * lineLength });
    }

    public override void Fire(Gun gun, bool doDamage)
    {
        base.Fire(gun, doDamage);

        RaycastHit hit;
        if (Physics.Raycast(gun.camera.transform.position, gun.camera.transform.forward, out hit))
        {
            lineRendererController.lineRenderer.SetPosition(0, gun.barrelEnd.position);
            lineRendererController.lineRenderer.SetPosition(1, hit.point);
            lineRendererController.Fire(lineTime);

            if (hit.rigidbody == null)
            {
                GameObject bulletMark = bulletMarkPool.Get();
                bulletMark.transform.position = hit.point + hit.normal * 0.001f;
                bulletMark.transform.rotation = Quaternion.LookRotation(-hit.normal, Vector3.up);
            }
            else if(doDamage)
            {
                Health health = hit.rigidbody.GetComponent<Health>();
                if (health != null)
                    health.Damage(damage);
            }
        }
        else
        {
            lineRendererController.lineRenderer.SetPosition(0, gun.barrelEnd.position);
            lineRendererController.lineRenderer.SetPosition(1, gun.barrelEnd.position + gun.camera.transform.forward * lineLength);
            lineRendererController.Fire(lineTime);
        }

        ApplyRecoil(gun);
    }
}