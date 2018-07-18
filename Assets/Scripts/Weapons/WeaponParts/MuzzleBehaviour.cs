using UnityEngine;

public class MuzzleBehaviour : WeaponPartBehaviour
{
    [HideInInspector]
    public Muzzle data;

    public Transform barrelEnd;

    public override void SetPart(WeaponPart part)
    {
        base.SetPart(part);

        data = part as Muzzle;
    }

    public Transform GetBarrelEnd()
    {
        return barrelEnd;
    }
}
