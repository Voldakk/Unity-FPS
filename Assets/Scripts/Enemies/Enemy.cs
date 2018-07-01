using UnityEngine;
using GameSparks.RT;
using System.Collections;

[RequireComponent(typeof(Health))]
public class Enemy : NetworkObject
{
    protected enum UpdateCode { PosAndRot }

    public float updateRate = 0.1f;

    private Vector3 prevPos;
    private Quaternion prevRot;

    private Vector3 goToPos;
    private Vector2 goToRot;

    private new Rigidbody rigidbody;
    private Health health;

    private bool isHost;

    protected override void Awake()
    {
        base.Awake();

        rigidbody = GetComponent<Rigidbody>();

        health = GetComponent<Health>();
        health.Initialize(false);
    }

    void Start()
    {
        isHost = GameManager.Instance().IsHost;

        if (isHost)
            StartCoroutine(SendMovement());
    }

    void Update()
    {
        if (!isHost)
        {
            float t = Time.deltaTime / updateRate;

            transform.position = Vector3.Lerp(transform.position, goToPos, t);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(goToRot.x, goToRot.y, 0.0f), t);
        }
    }

    private void UpdatePosition(Vector3 position)
    {
        goToPos = position;
    }

    private void UpdateRotation(Vector2 rotation)
    {
        goToRot = rotation;
    }

    void OnDeath()
    {
        Destroy(gameObject);
    }

    private IEnumerator SendMovement()
    {
        // If we are moving
        if ((transform.position != prevPos) || transform.rotation != prevRot)
        {
            using (RTData data = RTData.Get())
            {
                if (transform.position != prevPos)
                    data.SetVector3(1, transform.position);

                if(transform.rotation != prevRot)
                    data.SetVector2(2, new Vector2(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y));

                SendPacket(UpdateCode.PosAndRot, data, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED);
            }

            prevPos = transform.position;
            prevRot = transform.rotation;
        }

        yield return new WaitForSeconds(updateRate);
        StartCoroutine(SendMovement());
    }

    protected void SendPacket(UpdateCode updateCode, RTData data, GameSparksRT.DeliveryIntent intent = GameSparksRT.DeliveryIntent.RELIABLE)
    {
        data.SetInt((int)NetworkManager.DataIndex.ObjectCode, (int)updateCode);
        SendPacket(data, intent);
    }

    protected void SendPacket(UpdateCode updateCode, GameSparksRT.DeliveryIntent intent = GameSparksRT.DeliveryIntent.RELIABLE)
    {
        using (RTData data = RTData.Get())
        {
            data.SetInt((int)NetworkManager.DataIndex.ObjectCode, (int)updateCode);
            SendPacket(data, intent);
        }
    }

    public override void OnPacket(RTPacket packet)
    {
        int code = packet.Data.GetInt((int)NetworkManager.DataIndex.ObjectCode).Value;
        switch ((UpdateCode)code)
        {
            case UpdateCode.PosAndRot:
                if (packet.Data.GetVector3(1).HasValue)
                    UpdatePosition(packet.Data.GetVector3(1).Value);

                if (packet.Data.GetVector2(2).HasValue)
                    UpdateRotation(packet.Data.GetVector2(2).Value);
                break;

            default:
                Debug.LogError("Enemy::OnPacket - Unknown update code " + code);
                break;
        }
    }

    
}