using GameSparks.RT;
using UnityEngine;

public class Weapon : ScriptableObject
{
    [HideInInspector] public Camera camera;
    [HideInInspector] public RectTransform hud;
    [HideInInspector] public Transform transform;
    [HideInInspector] public Transform weaponHolder;
    [HideInInspector] public WeaponBehaviour weaponBehaviour;

    public string weaponName;
    public Sprite icon;

    public GameObject weaponPrefab;
    [HideInInspector] public Transform weaponObject;

    public GameObject uiPrefab;
    [HideInInspector] public Transform uiObject;

    [HideInInspector] public bool isLocal;

    public void Setup(Camera camera, RectTransform hud, Transform transform, Transform weaponHolder, WeaponBehaviour weaponBehaviour)
    {
        this.camera = camera;
        this.hud = hud;
        this.transform = transform;
        this.weaponHolder = weaponHolder;
        this.weaponBehaviour = weaponBehaviour;

        isLocal = weaponBehaviour.isLocal;
    }

    public virtual void OnStart()
    {
        if (weaponPrefab != null)
        {
            weaponObject = Instantiate(weaponPrefab, weaponHolder, false).transform;

            if(!isLocal)
            {
                MeshRenderer[] mrs = weaponObject.GetComponentsInChildren<MeshRenderer>();
                foreach (var mr in mrs)
                {
                    mr.gameObject.layer = 0;
                }
            }
        }

        if (uiPrefab != null && isLocal)
        {
            uiObject = Instantiate(uiPrefab, hud, false).transform;
        }
    }

    public virtual void OnDestroy()
    {
        if (weaponObject != null)
            Destroy(weaponObject.gameObject);

        if (uiObject != null)
            Destroy(uiObject.gameObject);
    }

    public virtual void OnUpdate()
    {

    }

    public void SendWeaponUpdate()
    {
        using (RTData data = RTData.Get())
        {
            SendWeaponUpdate(data);
        }
    }

    public void SendWeaponUpdate(RTData data)
    {
        if (weaponBehaviour.isLocal)
            GameManager.Instance().PlayerWeaponUpdate(weaponBehaviour.player, data);
    }

    public virtual void OnWeaponUpdate(RTPacket packet)
    {

    }
}