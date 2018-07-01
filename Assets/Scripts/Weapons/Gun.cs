using GameSparks.RT;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New gun", menuName = "Items/Weapons/Gun")]
public class Gun : Weapon
{
    protected enum UpdateCode { Fire, Reload, Ads, StopAds }

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
            crosshair = GameObject.Find("Crosshair").GetComponent<Crosshair>();
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
    }

    public override void OnDestroy()
    {
        ammo.Unload();

        base.OnDestroy();
    }

    void UpdateHudAmmo()
    {
        if (hudWeaponAmmo == null)
            return;

        hudWeaponAmmo.text = magCurrent + " / " + magSize;
    }

    void UpdateHudWeapon()
    {
        if (hudWeaponIcon == null)
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

                SendWeaponUpdate(UpdateCode.Fire);
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !reloading)
        {
            StartReload();
            SendWeaponUpdate(UpdateCode.Reload);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Ads();
            SendWeaponUpdate(UpdateCode.Ads);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            StopAds();
            SendWeaponUpdate(UpdateCode.StopAds);
        }
    }

    public void Fire()
    {
        ammo.Fire(this, isLocal);
        magCurrent--;

        UpdateHudAmmo();

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

        reloadTimer = 0.0f;
        reloading = true;

        if (reloadSound != null)
        {
            audioSource.clip = reloadSound;
            audioSource.Play();
        }

        if (animator != null)
            animator.SetTrigger("Reload");

        if (hudWeaponAmmo != null)
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
        if(crosshair != null)
            crosshair.Hide();

        //prevFov = camera.fieldOfView;
        //camera.fieldOfView = adsFov;

        if (animator != null)
            animator.SetBool("Ads", true);
    }
    public void StopAds()
    {
        if (crosshair != null)
            crosshair.Show();

        //camera.fieldOfView = prevFov;

        if (animator != null)
            animator.SetBool("Ads", false);
    }

    public override void OnWeaponUpdate(RTPacket packet)
    {
        base.OnWeaponUpdate(packet);

        switch ((UpdateCode)packet.Data.GetInt(2).Value)
        {
            case UpdateCode.Fire:
                Fire();
                break;

            case UpdateCode.Reload:
                StartReload();
                break;

            case UpdateCode.Ads:
                Ads();
                break;

            case UpdateCode.StopAds:
                StopAds();
                break;

            default:
                Debug.LogError("Gun::OnWeaponUpdate - Unknown update code " + packet.Data.GetInt(1).Value);
                break;
        }
    }

    protected void SendWeaponUpdate(UpdateCode code)
    {
        SendWeaponUpdate((int)code);
    }
}