using UnityEngine;

public class Barrel : WeaponPart
{
    public AnimationCurve accuracy; // 0-100%
    public AnimationCurve recoil;   // +/- recoil
    public AnimationCurve damage;   // +/- damage

    public float Accuracy
    {
        get
        {
            return ApplyModifier(accuracy.Evaluate(level));
        }
    }

    public float Recoil
    {
        get
        {
            return ApplyLowIsPosModifier(recoil.Evaluate(level));
        }
    }

    public float Damage
    {
        get
        {
            return ApplyModifier(damage.Evaluate(level));
        }
    }
}