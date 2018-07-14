using UnityEngine;

[ExecuteInEditMode]
public class LineToParent : MonoBehaviour
{
    LineRenderer lr;
	
	void Update ()
    {
        if(lr == null)
            lr = GetComponent<LineRenderer>();

        if (lr == null || transform.parent == null)
            return;

        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.parent.position);
    }
}
