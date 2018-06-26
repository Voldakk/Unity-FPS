using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ammo", menuName = "Ammo/MultiProjectile")]
public class MultiProjectile : Ammo
{
    public float damage;
    public float lineTime = 0.01f;
    public float lineLength = 1000.0f;

    public int numProjectiles;

    public float coneRadius;
    public float coneLength;

    public GameObject lineRendererPrefab;
    private LineRendererController lineRendererController;

    public override void Load(Weapon currentWeapon)
    {
        lineRendererController = Instantiate(lineRendererPrefab, currentWeapon.gameObject.transform.Find("BarrelEnd")).GetComponent<LineRendererController>();
    }

    public override void Unload()
    {
        Destroy(lineRendererController.gameObject);
    }
    public override void Fire(Weapon currentWeapon, PlayerShooting ps, bool doDamage)
    {
        LineRenderer lr = lineRendererController.lineRenderer;
        lr.positionCount = numProjectiles * 2;

        float[] playerDamage = new float[GameManager.Instance().NumPlayers()];

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
            Vector3 origin = ps.eyes.position;
            Vector3 worldDirection = ps.eyes.TransformDirection(direction);
            if (Physics.Raycast(origin, worldDirection, out hit))
            {
                Player player = hit.transform.GetComponent<Player>();
                if (player != null)
                {
                    playerDamage[player.peerId-1] += damage;
                    
                }
            }
        }

        for (int i = 0; i < playerDamage.Length; i++)
        {
            if(playerDamage[i] != 0.0f)
                GameManager.Instance().DamagePlayer(i+1, playerDamage[i]);
        }

        lineRendererController.Fire(lineTime);
    }
}