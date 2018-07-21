using UnityEngine;

public class LevelLoaded : MonoBehaviour
{
	void Start ()
    {
        var gsm = GameSparksManager.Instance();
        if(gsm != null)
            GameSparksManager.Instance().SetPlayerLoaded();
	}
}
