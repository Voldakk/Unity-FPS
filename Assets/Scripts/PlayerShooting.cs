using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    public Weapon startingWeapon;
    private Weapon currentWeapon;

    public Transform eyes;
    public Transform weaponHolder;

    private Image hudWeaponIcon;
    private Text hudWeaponAmmo;

    private bool reloading = false;

    [HideInInspector]
    public bool isPlayer;
    private Player player;

    public void Initialize(bool _isPlayer, Player _player)
    {
        isPlayer = _isPlayer;
        player = _player; 

        if(isPlayer)
        {
            hudWeaponIcon = GameObject.Find("HudWeaponIcon").GetComponent<Image>();
            hudWeaponAmmo = GameObject.Find("HudWeaponAmmo").GetComponent<Text>();
        }

        SetWeapon(startingWeapon);

        EndReload();
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
        currentWeapon.ammo = Instantiate(currentWeapon.ammo);
        currentWeapon.ammo.Load(currentWeapon);

        // HUD
        UpdateHudWeapon();
        UpdateHudAmmo();
    }

	void Update ()
    {
        if (!isPlayer)
            return;

        currentWeapon.reloadTimer += Time.deltaTime;
        currentWeapon.fireTimer += Time.deltaTime;

        if(reloading && currentWeapon.reloadTimer >= currentWeapon.reloadTime)
        {
            EndReload();
        }

        if (Input.GetMouseButton(0) && !reloading)
        {
            if(currentWeapon.fireTimer >= 60.0f / currentWeapon.firerate && currentWeapon.magCurrent > 0)
            {
                currentWeapon.fireTimer = 0;
                Fire(true);
                GameManager.Instance().Fire(player);
            }
        }

        if(Input.GetKeyDown(KeyCode.R) && !reloading)
        {
            StartReload();
        }
	}

    void UpdateHudAmmo()
    {
        if (!isPlayer)
            return;

        hudWeaponAmmo.text = currentWeapon.magCurrent + " / " + currentWeapon.magSize;
    }

    void UpdateHudWeapon()
    {
        if (!isPlayer)
            return;
        hudWeaponIcon.sprite = currentWeapon.icon;
        hudWeaponIcon.type = Image.Type.Simple;
        hudWeaponIcon.preserveAspect = true;
    }

    public void Fire(bool doDamage)
    {
        currentWeapon.magCurrent--;
        UpdateHudAmmo();

        currentWeapon.ammo.Fire(currentWeapon, this, doDamage);
    }

    public void StartReload()
    {
        currentWeapon.reloadTimer = 0.0f;
        reloading = true;

        hudWeaponAmmo.text = "Reloading...";
    }

    public void EndReload()
    {
        reloading = false;

        currentWeapon.magCurrent = currentWeapon.magSize;

        UpdateHudAmmo();
    }
}