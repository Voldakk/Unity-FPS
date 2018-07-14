using UnityEngine;
using UnityEngine.EventSystems;
using Voldakk.DragAndDrop;

public class UiWeaponPartSlotContainer : DragAndDropContainer
{
    WeaponPartSlot slot;
    UiWeaponPartSlotPanel panel;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    protected override void Initialize()
    {
        base.Initialize();

        dragPanel = GameObject.FindGameObjectWithTag("WeaponPartItemPanel").GetComponent<DragAndDropPanel>();

        slot = GetComponentInParent<WeaponPartSlot>();
        panel = GetComponent<UiWeaponPartSlotPanel>();
        panel.SetIndeces(containerIndex, new int[] { 0 });
        panel.SetObject(slot.part);
    }

    /// <summary>
    /// Return the object in the given indices
    /// </summary>
    /// <param name="indices">The indeces in which the object is stored</param>
    /// <returns>The object in the given indices</returns>
    public override object GetObject(int[] indices, bool isFromContainer)
    {
        if (indices.Length == 1 && indices[0] == 0)
            return slot.part;

        return null;
    }

    /// <summary>
    /// Try to recieve an object form another container
    /// </summary>
    /// <param name="o">The object to recieve</param>
    /// <param name="indices">The indeces in which the object should be stored</param>
    /// <returns>Whether the container sucsessfully recieved the object</returns>
    public override bool RecieveObject(object o, int[] indices)
    {
        // If it's not an item, or it's thw wrong number of indices
        if (!(o is WeaponPart) || indices.Length != 1)
            return false;

        // Get the index
        int index = indices[0];

        // Make sure the index is within range
        if (index != 0)
            return false;

        // Get the item
        WeaponPart part = o as WeaponPart;

        // If the recieving slot is empty
        if (slot.part == null)
        {
            if (slot.SetPart(part))
            {
                panel.SetObject(part);
                return true;
            }
        }

        // Failed to recieve the object
        return false;
    }

    /// <summary>
    /// Remove the object in the given indices
    /// </summary>
    /// <param name="indices">The indeces in which the object is stored</param>
    public override void RemoveObject(int[] indices)
    {
        // Check the indices
        if (indices.Length == 1 && indices[0] == 0)
        {
            // Remove the object
            slot.SetPart(null);
            panel.SetObject(null);
        }
    }

    /// <summary>
    /// When an object is clicked
    /// </summary>
    /// <param name="button">The mouse button that was pressed</param>
    /// <param name="indices">The indeces in which the object is stored</param>
    public override void OnObjectMouseDown(PointerEventData.InputButton button, int[] indices)
    {
        if (indices.Length != 1)
            return;

        if (button == PointerEventData.InputButton.Left)
            Debug.LogFormat("Left click on index {0}", indices[0]);
        else if (button == PointerEventData.InputButton.Right)
            Debug.LogFormat("Right click on index {0}", indices[0]);
    }
}
