using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Look : MonoBehaviour
{
    public float XSensitivity = 2f;
    public float YSensitivity = 2f;

    public Transform look;

    private readonly float halfsqrt = Mathf.Sqrt(0.5f);

    void Update ()
    {
        // Horizontal
        float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
        transform.localRotation *= Quaternion.Euler(0f, yRot, 0f);

        // Vertical
        float xRot = -CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;
        look.localRotation *= Quaternion.Euler(xRot, 0f, 0f);

        float lookX = look.localRotation.x;
        if (lookX < -halfsqrt || lookX > halfsqrt)
        {
            float x = Mathf.Clamp(lookX, -halfsqrt, halfsqrt);
            float w = halfsqrt;

            look.localRotation = new Quaternion(x, 0f, 0f, w);
        }
    }
}