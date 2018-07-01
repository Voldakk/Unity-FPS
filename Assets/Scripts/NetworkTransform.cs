using UnityEngine;
using GameSparks.RT;
using System.Collections;

public class NetworkTransform : NetworkObject
{
    public float updateRate = 0.1f;

    private Vector3 prevPos;
    private Quaternion prevRot;

    private Vector3 goToPos;
    private Vector2 goToRot;

    private bool isHost;

    protected override void Awake()
    {
        base.Awake();

        prevPos = transform.position;
        prevRot = transform.rotation;
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

    public override void OnPacket(RTPacket packet)
    {
        if (packet.Data.GetVector3(1).HasValue)
            goToPos = packet.Data.GetVector3(1).Value;

        if (packet.Data.GetVector2(2).HasValue)
            goToRot = packet.Data.GetVector2(2).Value;
    }

    private IEnumerator SendMovement()
    {
        if ((transform.position != prevPos) || transform.rotation != prevRot)
        {
            using (RTData data = RTData.Get())
            {
                if (transform.position != prevPos)
                    data.SetVector3(1, transform.position);

                if (transform.rotation != prevRot)
                    data.SetVector2(2, new Vector2(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y));

                SendPacket(data, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED);
            }

            prevPos = transform.position;
            prevRot = transform.rotation;
        }

        yield return new WaitForSeconds(updateRate);
        StartCoroutine(SendMovement());
    }

    public void SetPosition(Vector3 newPosition)
    {
        OnSetPosition(newPosition);
    }
    
    void OnSetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
        goToPos = newPosition;
    }
}