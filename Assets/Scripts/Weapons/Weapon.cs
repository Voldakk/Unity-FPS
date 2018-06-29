using UnityEngine;

public class Weapon : ScriptableObject
{
    [HideInInspector] public Camera camera;
    [HideInInspector] public RectTransform hud;
    [HideInInspector] public Transform transform;
    [HideInInspector] public Transform weaponHolder;

    public string weaponName;
    public Sprite icon;

    public GameObject weaponPrefab;
    [HideInInspector] public Transform weaponObject;

    public GameObject uiPrefab;
    [HideInInspector] public Transform uiObject;

    public void Setup(Camera camera, RectTransform hud, Transform transform, Transform weaponHolder)
    {
        this.camera = camera;
        this.hud = hud;
        this.transform = transform;
        this.weaponHolder = weaponHolder;
    }

    public virtual void OnStart()
    {
        if (weaponPrefab != null)
        {
            weaponObject = Instantiate(weaponPrefab, weaponHolder, false).transform;
        }

        if (uiPrefab != null)
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
}