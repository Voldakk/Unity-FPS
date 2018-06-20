using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public Weapon weapon;
    public Transform weaponHolder;

    public GameObject lineRendererPrefab;
    private LineRenderer lineRenderer;

    public float lineTime;
    private float lineTimer;

	// Use this for initialization
	void Start ()
    {
        weapon.gameObject = Instantiate(weapon.prefab, weaponHolder);

        lineRenderer = Instantiate(lineRendererPrefab, weapon.gameObject.transform.Find("BarrelEnd")).GetComponent<LineRenderer>();

        lineRenderer.enabled = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        weapon.reloadTimer += Time.deltaTime;
        weapon.fireTimer += Time.deltaTime;
        lineTimer += Time.deltaTime;

        if(lineTimer >= lineTime && lineRenderer.enabled)
        {
            lineRenderer.enabled = false;
        }

        if (Input.GetMouseButton(0))
        {
            if(weapon.fireTimer >= 60.0f / weapon.firerate)
            {
                weapon.fireTimer = 0;
                lineTimer = 0;
                lineRenderer.enabled = true;
            }
        }
	}
}
