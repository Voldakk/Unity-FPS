using UnityEngine;

public enum WeaponPartType { Barrel, Body, Grip, Mag, Stock }

public class WeaponPart : ScriptableObject
{
    public string stortCode;
    public string partName;
    public GameObject prefab;
    public WeaponPartType partType;
}