using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrtoPanAndZoom : MonoBehaviour
{
    public float minSize = 0.1f, maxSize = 10f;
    public float zoomSpeed = 0.1f;
    public float panSpeed = 0.1f;

    new Camera camera;

    void Awake ()
    {
        camera = GetComponent<Camera>();
	}

	void Update ()
    {
		if(Input.GetMouseButton(2))
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");

            transform.position -= new Vector3(x * panSpeed, y * panSpeed, 0f);
        }
        camera.orthographicSize -= Input.mouseScrollDelta.y * zoomSpeed;

        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minSize, maxSize);
	}
}
