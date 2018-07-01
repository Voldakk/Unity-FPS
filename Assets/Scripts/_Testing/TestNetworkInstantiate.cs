using UnityEngine;

public class TestNetworkInstantiate : MonoBehaviour
{
    public GameObject prefab;

	void Update ()
    {
		if(Input.GetKeyDown(KeyCode.P) && GetComponent<Player>().peerId == GameSparksManager.PeerId())
        {
            Debug.Log("TestNetworkInstantiate::Update - instantiating '" + prefab.name + "'");
            GameObject copy = NetworkManager.NetworkInstantiate(prefab);

            Debug.Log("TestNetworkInstantiate::Update - Instantiated");

            copy.transform.position = transform.position + transform.forward * 2f;
        }
    }
}