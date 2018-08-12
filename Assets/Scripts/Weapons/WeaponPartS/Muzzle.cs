using UnityEngine;

public class Muzzle : WeaponPart
{
    public AnimationCurve recoil;
    public float Recoil
    {
        get
        {
            return ApplyLowIsPosModifier(recoil.Evaluate(level));
        }
    }

    public AnimationCurve damage;
    public float Damage
    {
        get
        {
            return ApplyModifier(damage.Evaluate(level));
        }
    }

    public AnimationCurve accuracyModifier;
    public float AccuracyModifier
    {
        get
        {
            return ApplyModifier(accuracyModifier.Evaluate(level));
        }
    }

    public AnimationCurve pitchModifier;
    public float PitchModifier
    {
        get
        {
            return ApplyModifier(pitchModifier.Evaluate(level));
        }
    }

    public AnimationCurve volumeModifier;
    public float VolumeModifier
    {
        get
        {
            return ApplyLowIsPosModifier(volumeModifier.Evaluate(level));
        }
    }
}