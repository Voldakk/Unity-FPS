using UnityEngine;

public class Interactor : MonoBehaviour
{
    public new Camera camera;
    public float maxDistance = 10f;

    Interactable target;

    public string button;

	void Update ()
    {
        RaycastHit hit;
        if(Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, maxDistance))
        {
            var i = hit.transform.GetComponent<Interactable>();
            if(i != null && hit.distance <= i.maxDistance)
            {
                SetTarget(i);
            }
            else
            {
                SetTarget(null);
            }
        }
        else
        {
            SetTarget(null);
        }

        if(target != null && Input.GetButtonDown(button))
        {
            target.Interact(this);
        }
	}

    void SetTarget(Interactable newTarget)
    {
        if (newTarget == target)
            return;

        if (target != null)
            target.OnRemoveTarget();

        target = newTarget;

        if(target == null)
        {
            InteractPanel.Hide();
        }
        else
        {
            InteractPanel.Show("Interact with " + target.name);

            target.OnSetAsTarget();
        }
    }
}
