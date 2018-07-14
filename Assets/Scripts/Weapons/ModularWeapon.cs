using UnityEngine;

public class ModularWeapon : MonoBehaviour
{
    public WeaponPartSlot bodySlot;

    void Awake()
    {
        bodySlot = GetComponentInChildren<WeaponPartSlot>();
    }
}