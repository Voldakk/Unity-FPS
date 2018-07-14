using UnityEngine;

public class WeaponPartSlot : MonoBehaviour
{
    public string slotName;
    public WeaponPartType slotType;
    public WeaponPart part;

    public GameObject uiSlotPrefab;
    public Vector3 uiSlotPosition;


    void OnValidate()
    {
        UiWeaponPartSlotPanel uiSlot = GetComponentInChildren<UiWeaponPartSlotPanel>();
        if(uiSlot != null)
        {
            uiSlot.slotName.text = slotName;
        }
    }

    public bool SetPart(WeaponPart newPart)
    {
        if (newPart != null && newPart.partType != slotType)
            return false;


        if(part != null)
            part.Unload();

        part = newPart;

        if (part != null)
            part.Load(transform);

        return true;
    }

    void Awake()
    {
        UiWeaponPartSlotPanel uiSlot = Instantiate(uiSlotPrefab, transform).GetComponent<UiWeaponPartSlotPanel>();
        uiSlot.transform.localPosition = uiSlotPosition;
        uiSlot.slotName.text = slotName;
    }
}
