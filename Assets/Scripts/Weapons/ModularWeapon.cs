using UnityEngine;

public class ModularWeapon : MonoBehaviour
{
    public string weaponName;
    public WeaponPartSlot bodySlot;

    void Awake()
    {
        bodySlot = GetComponentInChildren<WeaponPartSlot>();
    }
}