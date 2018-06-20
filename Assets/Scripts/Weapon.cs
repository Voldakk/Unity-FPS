using UnityEngine;

[CreateAssetMenu(fileName = "New weapon", menuName = "Items/Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public Sprite icon;
    public GameObject prefab;
    [HideInInspector] public GameObject gameObject;

    public Ammo ammo;

    public float firerate;
    [HideInInspector] public float fireTimer;

    public float reloadTime;
    [HideInInspector] public float reloadTimer;

    public int magSize;
    [HideInInspector] public int magCurrent;
}