using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineRendererController : MonoBehaviour
{
    LineRenderer lineRenderer;
    float lineTimer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (lineTimer <= 0.0f && lineRenderer.enabled)
            lineRenderer.enabled = false;

        lineTimer -= Time.deltaTime;
    }

    public void Fire(float time)
    {
        lineTimer = time;
        lineRenderer.enabled = true;
    }
}
