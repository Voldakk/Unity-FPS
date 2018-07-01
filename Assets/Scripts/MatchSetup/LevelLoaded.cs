using UnityEngine;

public class LevelLoaded : MonoBehaviour
{
	void Start ()
    {
        GameSparksManager.Instance().SetPlayerLoaded();
	}
}
