using UnityEngine;
using TMPro;

public class Countdown : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float time;

    public void SetCountdown(float value)
    {
        time = value;
    }

	void Update ()
    {
		if(time != 0f)
        {
            time -= Time.deltaTime;
            if (time < 0f)
                time = 0f;

            text.text = "Starting in " + time.ToString("0.0") + " seconds";
        }
	}
}