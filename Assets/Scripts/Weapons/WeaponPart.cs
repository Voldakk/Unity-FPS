using UnityEngine;

using Voldakk.DragAndDrop;

public enum WeaponPartType { Barrel, Body, Grip, Mag, Sight, Stock, Muzzle }

[System.Serializable]
public class WeaponPart : Item
{
    public string stortCode;
    public string partName;
    public GameObject prefab;
    public WeaponPartType partType;

    public WeaponPartSlot[] slots;
    public GameObject gameObject;

    // Stats
    public float weight, cost;

    public void Load(Transform parent)
    {
        if (gameObject != null)
            Destroy(gameObject);

        gameObject = Instantiate(prefab, parent);
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;

        slots = gameObject.GetComponentsInChildren<WeaponPartSlot>();
    }

    public void Unload()
    {
        if (gameObject != null)
            Destroy(gameObject);

        slots = null;
    }
}