using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{

    public Weapon startingWeapon;
    private Weapon currentWeapon;

    public Transform weaponHolder;

    public GameObject lineRendererPrefab;
    private LineRenderer lineRenderer;

    public float lineTime;
    private float lineTimer;

    public Image hudWeaponIcon;
    public Text hudWeaponAmmo;

    private bool reloading = false;

    void Awake()
    {
        lineRenderer = Instantiate(lineRendererPrefab).GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    void Start ()
    {
        SetWeapon(startingWeapon);

        currentWeapon.magCurrent = currentWeapon.magSize;
        UpdateHudAmmo();
    }
	
    void SetWeapon(Weapon newWeapon)
    {
        if(currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
        }

        currentWeapon = Instantiate(newWeapon);

        currentWeapon.gameObject = Instantiate(currentWeapon.prefab, weaponHolder);

        lineRenderer.transform.SetParent(currentWeapon.gameObject.transform.Find("BarrelEnd"), false);

        hudWeaponIcon.sprite = currentWeapon.icon;
        hudWeaponIcon.type = Image.Type.Simple;
        hudWeaponIcon.preserveAspect = true;
        UpdateHudAmmo();
    }

	void Update ()
    {
        currentWeapon.reloadTimer += Time.deltaTime;
        currentWeapon.fireTimer += Time.deltaTime;
        lineTimer += Time.deltaTime;

        if(reloading && currentWeapon.reloadTimer >= currentWeapon.reloadTime)
        {
            reloading = false;

            currentWeapon.magCurrent = currentWeapon.magSize;
            UpdateHudAmmo();
        }

        if(lineTimer >= lineTime && lineRenderer.enabled)
        {
            lineRenderer.enabled = false;
        }

        if (Input.GetMouseButton(0) && !reloading)
        {
            if(currentWeapon.fireTimer >= 60.0f / currentWeapon.firerate && currentWeapon.magCurrent > 0)
            {
                currentWeapon.fireTimer = 0;

                currentWeapon.magCurrent--;

                lineTimer = 0;
                lineRenderer.enabled = true;

                UpdateHudAmmo();
            }
        }

        if(Input.GetKeyDown(KeyCode.R) && !reloading)
        {
            currentWeapon.reloadTimer = 0.0f;
            reloading = true;
            hudWeaponAmmo.text = "Reloading...";
        }
	}

    void UpdateHudAmmo()
    {
        hudWeaponAmmo.text = currentWeapon.magCurrent + " / " + currentWeapon.magSize;
    }
}