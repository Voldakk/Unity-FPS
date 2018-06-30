using GameSparks.RT;
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

    public AudioClip fireSound, reloadSound;
    private AudioSource audioSource;

    private Image hudWeaponIcon;
    private Text hudWeaponAmmo;

    private bool reloading;

    [HideInInspector] public Transform barrelEnd;

    private Crosshair crosshair;

    public float adsFov;
    private float prevFov;

    protected Animator animator;

    public override void OnStart()
    {
        base.OnStart();

        if (isLocal)
        {
            hudWeaponIcon = uiObject.Find("Icon").GetComponent<Image>();
            hudWeaponAmmo = uiObject.Find("Ammo").GetComponent<Text>();
        }

        barrelEnd = weaponObject.Find("BarrelEnd");
        if(barrelEnd == null)
            Debug.LogError("Gun::OnStart - Missing 'BarrelEnd'");

        animator = weaponObject.GetComponent<Animator>();
        if(animator != null)
        {
            animator.SetFloat("ReloadTime", 1f / reloadTime);
        }

        audioSource = weaponHolder.GetComponent<AudioSource>();

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
        if (!isLocal)
            return;

        hudWeaponAmmo.text = magCurrent + " / " + magSize;
    }

    void UpdateHudWeapon()
    {
        if (!isLocal)
            return;

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
                Fire();

                SendWeaponUpdate();
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

    public void Fire()
    {
        if (isLocal)
        {
            magCurrent--;
            UpdateHudAmmo();
        }

        ammo.Fire(this, isLocal);

        if(fireSound != null)
        {
            audioSource.clip = fireSound;
            audioSource.Play();
        }

        if (animator != null)
            animator.SetTrigger("Fire");
    }

    public void StartReload()
    {
        StopAds();

        if(reloadSound != null)
        {
            audioSource.clip = reloadSound;
            audioSource.Play();
        }

        if (animator != null)
            animator.SetTrigger("Reload");

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

        //prevFov = camera.fieldOfView;
        //camera.fieldOfView = adsFov;

        if (animator != null)
            animator.SetBool("Ads", true);
    }
    public void StopAds()
    {
        crosshair.Show();

        //camera.fieldOfView = prevFov;

        if (animator != null)
            animator.SetBool("Ads", false);
    }

    public override void OnWeaponUpdate(RTPacket packet)
    {
        base.OnWeaponUpdate(packet);

        Fire();
    }
}