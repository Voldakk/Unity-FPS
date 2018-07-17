using UnityEngine;

public abstract class Ammo : ScriptableObject
{
    public string ammoName;
    public Sprite icon;

    public virtual void Load(ModularWeapon weapon, Barrel barrel)
    {

    }

    public virtual void Unload()
    {

    }

    public virtual void Fire(ModularWeapon weapon, Barrel barrel, bool doDamage)
    {

    }
}