using UnityEngine;

public abstract class Ammo : ScriptableObject
{
    public string ammoName;
    public Sprite icon;

    public virtual void Load(ModularWeapon weapon, Transform barrelEnd)
    {

    }

    public virtual void Unload()
    {

    }

    public virtual void Fire(ModularWeapon weapon, bool doDamage)
    {

    }
}