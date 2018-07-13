using UnityEngine;
using UnityEngine.EventSystems;
using Voldakk.DragAndDrop;

public class UiWeaponParts : DragAndDropContainer
{
    public ModularWeapon weapon;

    public UiWeaponPartSlot[] weaponPartSlots;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    protected override void Initialize()
    {
        base.Initialize();

        for (int i = 0; i < weaponPartSlots.Length; i++)
        {
            weaponPartSlots[i].SetIndeces(containerIndex, new int[] { i });
            weaponPartSlots[i].SetObject(weapon.GetPart(i));
        }
    }

    /// <summary>
    /// Return the object in the given indices
    /// </summary>
    /// <param name="indices">The indeces in which the object is stored</param>
    /// <returns>The object in the given indices</returns>
    public override object GetObject(int[] indices, bool isFromContainer)
    {
        if (indices.Length == 1 && indices[0] >= 0 && indices[0] < weaponPartSlots.Length)
            return weapon.GetPart(indices[0]);

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
        if (index < 0 && index >= weapon.numSlots)
            return false;

        // Get the item
        WeaponPart part = o as WeaponPart;

        // If the recieving slot is empty
        if (weapon.GetPart(index) == null)
        {
            if (weapon.SetPart(index, part))
            {
                weaponPartSlots[index].SetObject(part);
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
        if (indices.Length == 1 && indices[0] >= 0 && indices[0] < weaponPartSlots.Length)
        {
            // Remove the object
            weapon.SetPart(indices[0], null);
            weaponPartSlots[indices[0]].SetObject(null);
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
