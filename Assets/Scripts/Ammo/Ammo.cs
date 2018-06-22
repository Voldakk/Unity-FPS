using UnityEngine;

public abstract class Ammo : ScriptableObject
{
    public string ammoName;
    public Sprite icon;

    public abstract void Load(Weapon currentWeapon);
    public abstract void Unload();
    public abstract void Fire(Weapon currentWeapon, PlayerShooting ps, bool doDamage);
}