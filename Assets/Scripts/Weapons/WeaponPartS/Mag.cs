using UnityEngine;

public class Mag : WeaponPart
{
    public AudioClip reloadSound;
    public Ammo ammo;

    public AnimationCurve magSize;
    public AnimationCurve reloadTime;    // s

    public int MagSize
    {
        get
        {
            return Mathf.RoundToInt(ApplyModifier(magSize.Evaluate(level)));
        }
    }

    public float ReloadTime
    {
        get
        {
            return ApplyLowIsPosModifier(reloadTime.Evaluate(level));
        }
    }
}