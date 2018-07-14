using UnityEngine;

[ExecuteInEditMode]
public class UiWeaponPartSlotPosition : MonoBehaviour
{
    WeaponPartSlot slot;

	void Update ()
    {
        if(slot == null)
            slot = GetComponentInParent<WeaponPartSlot>();

        if (slot != null)
            slot.uiSlotPosition = transform.localPosition;
    }
}
