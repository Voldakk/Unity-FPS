using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New gun", menuName = "Items/Weapons/Gun")]
public class Gun : Weapon
{
    public Ammo ammo;

    public float firerate;
    [HideInInspector] public float fireTimer;

    public float reloadTime;
    [HideInInspector] public float reloadTimer;

    public int magSize;
    [HideInInspector] public int magCurrent;

    public Vector3 adsPosition;

    private Image hudWeaponIcon;
    private Text hudWeaponAmmo;

    private bool reloading;

    [HideInInspector] public Transform barrelEnd;

    private Crosshair crosshair;

    public float adsFov;
    private float prevFov;

    public override void OnStart()
    {
        base.OnStart();

        hudWeaponIcon = uiObject.Find("Icon").GetComponent<Image>();
        hudWeaponAmmo = uiObject.Find("Ammo").GetComponent<Text>();

        barrelEnd = weaponObject.Find("BarrelEnd");
        if(barrelEnd == null)
            Debug.LogError("Gun::OnStart - Missing 'BarrelEnd'");

        ammo = Instantiate(ammo);
        ammo.Load(this);

        UpdateHudWeapon();
        EndReload();

        crosshair = GameObject.Find("Crosshair").GetComponent<Crosshair>();
    }

    public override void OnDestroy()
    {
        ammo.Unload();

        base.OnDestroy();
    }

    void UpdateHudAmmo()
    {
        hudWeaponAmmo.text = magCurrent + " / " + magSize;
    }

    void UpdateHudWeapon()
    {
        hudWeaponIcon.sprite = icon;
        hudWeaponIcon.type = Image.Type.Simple;
        hudWeaponIcon.preserveAspect = true;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        reloadTimer += Time.deltaTime;
        fireTimer += Time.deltaTime;

        if (reloading && reloadTimer >= reloadTime)
        {
            EndReload();
        }

        if (Input.GetMouseButton(0) && !reloading)
        {
            if (fireTimer >= 60.0f / firerate && magCurrent > 0)
            {
                fireTimer = 0;
                Fire(true);
                //GameManager.Instance().Fire(player);
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !reloading)
        {
            StartReload();
        }

        if (Input.GetMouseButtonDown(1))
            Ads();
        else if (Input.GetMouseButtonUp(1))
            StopAds();
    }

    public void Fire(bool doDamage)
    {
        magCurrent--;
        UpdateHudAmmo();

        ammo.Fire(this, doDamage);
    }

    public void StartReload()
    {
        reloadTimer = 0.0f;
        reloading = true;

        hudWeaponAmmo.text = "Reloading...";
    }

    public void EndReload()
    {
        reloading = false;

        magCurrent = magSize;

        UpdateHudAmmo();
    }

    public void Ads()
    {
        crosshair.Hide();

        weaponObject.transform.localPosition = adsPosition;

        prevFov = camera.fieldOfView;
        camera.fieldOfView = adsFov;
    }
    public void StopAds()
    {
        crosshair.Show();

        weaponObject.transform.localPosition = Vector3.zero;

        camera.fieldOfView = prevFov;
    }
}