using UnityEngine;
using UnityEngine.UI;

public class InteractPanel : MonoBehaviour
{
    public Text text;

    static InteractPanel instance;

	void Awake ()
    {
        instance = this;
        Hide();
    }

    public static void Show(string text)
    {
        instance.gameObject.SetActive(true);
        instance.text.text = text;
    }

    public static void Hide()
    {
        instance.gameObject.SetActive(false);
    }
}
