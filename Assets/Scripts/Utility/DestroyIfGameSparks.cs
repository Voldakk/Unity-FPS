using UnityEngine;

public class DestroyIfGameSparks : MonoBehaviour
{
	void Awake ()
    {
        var gsm = GameSparksManager.Instance();
        if (gsm != null)
            Destroy(gameObject);
    }
}