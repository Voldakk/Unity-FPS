using UnityEngine;

public class PartPickup : Interactable
{
    public WeaponPart part;

    public void SetPart(WeaponPart newPart)
    {
        part = newPart;

        var partTransform = Instantiate(part.prefab, transform).GetComponent<Transform>();
        partTransform.localPosition = Vector3.zero;
        partTransform.localRotation = Quaternion.identity;
    }

    public override void OnSetAsTarget()
    {
        base.OnSetAsTarget();

        UiWeaponPartHoverPanel.Show(part);
    }

    public override void OnRemoveTarget()
    {
        base.OnRemoveTarget();

        UiWeaponPartHoverPanel.Hide();
    }

    public override void Interact(Interactor interactor)
    {
        base.Interact(interactor);

        PlayerData.instance.AddPart(part);

        OnRemoveTarget();

        Destroy(gameObject);
    }
}
