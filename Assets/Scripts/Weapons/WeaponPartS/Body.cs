using UnityEngine;

public class Body : WeaponPart
{
    public enum FireMode
    {
        Single, Burst, Auto
    }

    public AnimationCurve fireRate; // RPM
    public FireMode fireMode;

    public int FireRate
    {
        get
        {
            return Mathf.RoundToInt(ApplyModifier(fireRate.Evaluate(level)));
        }
    }
}