using UnityEngine;

public class Sight : WeaponPart
{
    public GameObject scopePrefab;

    public static float baseFov = 60;

    public AnimationCurve zoom;
    public float Zoom
    {
        get
        {
            return Mathf.Max(1f, ApplyModifier(zoom.Evaluate(level)));
        }
    }

    public AnimationCurve adsTime;
    public float AdsTime
    {
        get
        {
            return Mathf.Max(0f, ApplyLowIsPosModifier(adsTime.Evaluate(level)));
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
}