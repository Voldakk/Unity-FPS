using UnityEngine;

public class MuzzleBehaviour : WeaponPartBehaviour
{
    [HideInNormalInspector]
    public Muzzle data;

    Transform barrelEnd;

    public override void SetPart(WeaponPart part)
    {
        base.SetPart(part);

        data = part as Muzzle;
    }

    protected override void Awake()
    {
        base.Awake();

        barrelEnd = transform.Find("BarrelEnd");
    }

    public Transform GetBarrelEnd()
    {
        return barrelEnd;
    }
}
