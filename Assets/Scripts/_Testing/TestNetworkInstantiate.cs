using UnityEngine;

public class TestNetworkInstantiate : MonoBehaviour
{
    public GameObject prefab;

	void Update ()
    {
		if(Input.GetKeyDown(KeyCode.P) && GetComponent<Player>().peerId == GameSparksManager.PeerId())
        {
            Vector3 spawnPos = transform.position + transform.forward * 2f;
            GameObject copy = NetworkManager.NetworkInstantiate(prefab, position: spawnPos);
        }
    }
}