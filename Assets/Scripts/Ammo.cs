using UnityEngine;

[CreateAssetMenu(fileName = "New ammo", menuName = "Items/Ammo")]
public class Ammo : ScriptableObject
{
    public string ammoName;
    public Sprite icon;

    public float damage;
}