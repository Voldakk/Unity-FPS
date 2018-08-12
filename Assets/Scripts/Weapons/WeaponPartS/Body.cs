using UnityEngine;

public class Body : WeaponPart
{
    public enum FireMode
    {
        Single, Burst, Auto
    }
    public FireMode fireMode;
}