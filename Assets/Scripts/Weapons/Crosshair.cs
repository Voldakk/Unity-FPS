using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    private Image image;

	// Use this for initialization
	void Awake ()
    {
        image = GetComponent<Image>();
    }

    public void Show()
    {
        image.enabled = true;
    }

    public void Hide()
    {
        image.enabled = false;
    }
}