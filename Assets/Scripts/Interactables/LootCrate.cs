using UnityEngine;

public class LootCrate : Interactable
{
    public GameObject partPickupPrefab;

    public Transform partLocation;
    
    bool opened = false;
    public override void Interact(Interactor interactor)
    {
        base.Interact(interactor);

        if (opened)
            return;
        opened = true;


        var part = Instantiate(partPickupPrefab, partLocation.position, partLocation.rotation).GetComponent<PartPickup>();
        part.SetPart(WeaponParts.GetRandomPart(PlayerData.instance.level));
    }
}