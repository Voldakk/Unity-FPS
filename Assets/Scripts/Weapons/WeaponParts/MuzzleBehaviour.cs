using UnityEngine;

public class MuzzleBehaviour : WeaponPartBehaviour
{
    Transform barrelEnd;

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
