using UnityEngine;
using GameSparks.RT;

public class ModularWeapon : MonoBehaviour
{
    public string weaponName;
    public WeaponPartSlot bodySlot;
    private BodyBehaviour body;

    public enum OpCode { Fire = WeaponBehaviour.OpCode.Last, Reload, Ads, StopAds, Last }

    [HideInInspector] public Camera camera;
    [HideInInspector] public RectTransform hud;
    [HideInInspector] public WeaponBehaviour weaponBehaviour;
    [HideInInspector] public bool isOwner;

    void Awake()
    {
        bodySlot = GetComponentInChildren<WeaponPartSlot>();
    }

    public void Setup(Camera camera, RectTransform hud, WeaponBehaviour weaponBehaviour)
    {
        this.camera = camera;
        this.hud = hud;
        this.weaponBehaviour = weaponBehaviour;

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
    }

    public void OnWeaponUpdate(RTPacket packet, int code)
    {

    }
}