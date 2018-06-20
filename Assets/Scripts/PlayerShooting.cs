using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{

    public Weapon startingWeapon;
    private Weapon currentWeapon;

    public Transform weaponHolder;

    public Image hudWeaponIcon;
    public Text hudWeaponAmmo;

    private bool reloading = false;

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
            // Ammo
            currentWeapon.ammo.Unload();

            // Weapon
            Destroy(currentWeapon.gameObject);
        }

        // Weapon
        currentWeapon = Instantiate(newWeapon);
        currentWeapon.gameObject = Instantiate(currentWeapon.prefab, weaponHolder);

        // Ammo
        currentWeapon.ammo.Load(currentWeapon);

        // HUD
        hudWeaponIcon.sprite = currentWeapon.icon;
        hudWeaponIcon.type = Image.Type.Simple;
        hudWeaponIcon.preserveAspect = true;
        UpdateHudAmmo();
    }

	void Update ()
    {
        currentWeapon.reloadTimer += Time.deltaTime;
        currentWeapon.fireTimer += Time.deltaTime;

        if(reloading && currentWeapon.reloadTimer >= currentWeapon.reloadTime)
        {
            reloading = false;

            currentWeapon.magCurrent = currentWeapon.magSize;
            UpdateHudAmmo();
        }

        if (Input.GetMouseButton(0) && !reloading)
        {
            if(currentWeapon.fireTimer >= 60.0f / currentWeapon.firerate && currentWeapon.magCurrent > 0)
            {
                currentWeapon.fireTimer = 0;
                currentWeapon.magCurrent--;
                UpdateHudAmmo();

                currentWeapon.ammo.Fire(currentWeapon);
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