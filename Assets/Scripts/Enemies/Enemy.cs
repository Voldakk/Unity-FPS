using GameSparks.RT;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour
{
    protected enum UpdateCode { PosAndRot }

    public string id;
    public float updateRate = 0.1f;

    private Vector3 prevPos;
    private Quaternion prevRot;

    private Vector3 goToPos;
    private Vector2 goToRot;

    private new Rigidbody rigidbody;
    private Health health;

    private bool isHost;

    void Reset()
    {
        id = Guid.NewGuid().ToString("N");
    }

    void OnValidate()
    {
        id = Guid.NewGuid().ToString("N");
    }

	void Awake ()
    {
        rigidbody = GetComponent<Rigidbody>();

        health = GetComponent<Health>();
        health.Initialize(false);
    }

    void Start()
    {
        GameManager.Instance().RegisterEnemy(this);

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

    private IEnumerator SendMovement()
    {
        // If we are moving
        if ((transform.position != prevPos) || transform.rotation != prevRot)
        {
            using (RTData data = RTData.Get())
            {
                if (transform.position != prevPos)
                    data.SetVector3(10, transform.position);

                if(transform.rotation != prevRot)
                    data.SetVector2(11, new Vector2(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y));

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
        if (!isHost)
            return;

        data.SetInt(2, (int)updateCode);
        GameManager.Instance().NpcSendPacket(this, data, intent);
    }

    public virtual void OnPacket(RTPacket packet)
    {
        switch ((UpdateCode)packet.Data.GetInt(2).Value)
        {
            case UpdateCode.PosAndRot:
                if (packet.Data.GetVector3(10).HasValue)
                    UpdatePosition(packet.Data.GetVector3(10).Value);

                if (packet.Data.GetVector2(11).HasValue)
                    UpdateRotation(packet.Data.GetVector2(11).Value);
                break;

            default:
                Debug.LogError("Enemy::OnPacket - Unknown update code " + packet.Data.GetInt(2).Value);
                break;
        }
    }
}