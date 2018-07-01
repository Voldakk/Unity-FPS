using GameSparks.RT;
using UnityEngine;

public class TestPrefab : NetworkObject
{
    bool update = true;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("TestPrefab::Awake");
    }

    void Start()
    {
        Debug.Log("TestPrefab::Start");
    }

    void Update ()
    {
		if(update)
        {
            update = false;
            Debug.Log("TestPrefab::Update - First update");
        }
	}
    public override void OnPacket(RTPacket packet)
    {
        base.OnPacket(packet);
    }
}