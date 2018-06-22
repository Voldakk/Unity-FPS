using UnityEngine;

[CreateAssetMenu(fileName = "New ammo", menuName = "Ammo/SingleProjectile")]
public class SingleProjectile : Ammo
{
    public float damage;
    public float lineTime = 0.01f;
    public float lineLength = 1000.0f;

    public GameObject lineRendererPrefab;
    private LineRendererController lineRendererController;

    public override void Load(Weapon currentWeapon)
    {
        lineRendererController = Instantiate(lineRendererPrefab, currentWeapon.gameObject.transform.Find("BarrelEnd")).GetComponent<LineRendererController>();
        lineRendererController.lineRenderer.SetPositions(new Vector3[] { Vector3.zero, Vector3.forward * lineLength });
    }

    public override void Unload()
    {
        Destroy(lineRendererController.gameObject);
    }
    public override void Fire(Weapon currentWeapon, PlayerShooting ps, bool doDamage)
    {
        lineRendererController.Fire(lineTime);

        RaycastHit hit;
        if(Physics.Raycast(ps.eyes.position, ps.eyes.forward, out hit))
        {
            Player player = hit.transform.GetComponent<Player>(); 
            if(player != null)
            {
                GameManager.Instance().DamagePlayer(player, damage);
            }
        }
    }
}