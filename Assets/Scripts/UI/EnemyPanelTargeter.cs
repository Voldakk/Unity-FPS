using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPanelTargeter : MonoBehaviour
{
    public new Camera camera;
    public float maxDistance = 10f;

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, maxDistance))
        {
            var i = hit.transform.GetComponentInParent<Enemy>();
            if (i != null)
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
    }

    void SetTarget(Enemy newTarget)
    {
        if (newTarget == null)
        {
            //UiEnemyPanel.Hide();
        }
        else
        {
            UiEnemyPanel.Show(newTarget);
        }
    }
}
