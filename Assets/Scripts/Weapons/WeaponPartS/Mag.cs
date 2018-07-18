using UnityEngine;
using System.Collections;

public class Mag : WeaponPart
{
    public int magSize = 10;
    public float reloadTime = 1;
    public float recoilModifier = 1;

    public Ammo ammo;

    public AudioClip reloadSound;
}