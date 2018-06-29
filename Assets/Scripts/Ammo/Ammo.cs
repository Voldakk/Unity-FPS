using UnityEngine;

public abstract class Ammo : ScriptableObject
{
    public string ammoName;
    public Sprite icon;

    public virtual void Load(Gun gun)
    {

    }

    public virtual void Unload()
    {

    }

    public virtual void Fire(Gun gun, bool doDamage)
    {

    }
}