using UnityEngine;

public class Interactable : MonoBehaviour
{
    public new string name = "Interactable";
    public float maxDistance = 5f;

    public virtual void Interact(Interactor interactor)
    {
        Debug.Log("Interacted with " + name);
    }

    public virtual void OnSetAsTarget()
    {

    }

    public virtual void OnRemoveTarget()
    {

    }
}