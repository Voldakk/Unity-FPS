using UnityEngine;
using GameSparks.RT;

[RequireComponent(typeof(Health))]
public class Enemy : NetworkObject
{
    protected enum UpdateCode { None }

    public float updateRate = 0.1f;

    private new Rigidbody rigidbody;
    [HideInInspector]
    public Health health;

    protected override void Awake()
    {
        base.Awake();

        rigidbody = GetComponent<Rigidbody>();

        health = GetComponent<Health>();
        health.Initialize(false);
    }

    void Start()
    { 

    }

    void Update()
    {
        
    }

    void OnDeath()
    {
        Destroy(gameObject);
    }

    public override void OnPacket(RTPacket packet, int code)
    {
        switch ((UpdateCode)code)
        {
            case UpdateCode.None:
                break;

            default:
                Debug.LogError("Enemy::OnPacket - Unknown update code " + code);
                break;
        }
    }    
}