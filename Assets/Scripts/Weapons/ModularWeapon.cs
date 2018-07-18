using UnityEngine;
using GameSparks.RT;

public class ModularWeapon : MonoBehaviour
{
    public string weaponName;
    public WeaponPartSlot bodySlot;
    private BodyBehaviour body;

    public enum OpCode { Fire = WeaponBehaviour.OpCode.Last, Reload, Ads, StopAds, Last }

    [HideInInspector] public Camera camera;
    [HideInInspector] public Camera weaponCamera;
    [HideInInspector] public RectTransform hud;
    [HideInInspector] public WeaponBehaviour weaponBehaviour;
    [HideInInspector] public bool isOwner;

    SightBehaviour sight;

    void Awake()
    {
        bodySlot = GetComponentInChildren<WeaponPartSlot>();
    }

    public void Setup( RectTransform hud, WeaponBehaviour weaponBehaviour)
    {
        this.hud = hud;
        this.weaponBehaviour = weaponBehaviour;
        camera = weaponBehaviour.eyes;
        weaponCamera = weaponBehaviour.weaponCamera;

        isOwner = weaponBehaviour.isOwner;
    }

    public void OnStart()
    {
        if (!isOwner)
        {
            MeshRenderer[] mrs = GetComponentsInChildren<MeshRenderer>();
            foreach (var mr in mrs)
            {
                mr.gameObject.layer = 0;
            }
        }

        body = GetComponentInChildren<BodyBehaviour>();
        if (body != null)
            body.OnStart();

        sight = GetComponentInChildren<SightBehaviour>();
        if (sight != null)
            sight.OnStart();
    }

    public void OnUpdate()
    {
        if (Input.GetButton("Fire1"))
        {
            if (body != null)
                body.Fire(true);
        }
        if(Input.GetButtonDown("Reload"))
        {
            body.Reload();
        }

        if (sight != null)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                sight.Ads();
            }
            if (Input.GetButtonUp("Fire2"))
            {
                sight.StopAds();
            }
        }
    }

    public void OnWeaponUpdate(RTPacket packet, int code)
    {

    }
}