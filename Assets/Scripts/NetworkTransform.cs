using UnityEngine;
using GameSparks.RT;
using System.Collections;

public class NetworkTransform : NetworkObject
{
    public float updateRate = 0.1f;

    public bool local = true;

    public bool sendPosition = true;
    public bool posX = true, posY = true, posZ = true;

    public bool sendRotation = true;
    public bool rotX = true, rotY = true, rotZ = true;

    private Vector3 prevPos;
    private Vector3 prevRot;

    private Vector3 goToPos;
    private Vector3 goToRot;

    enum DataIndex { Position = 1, PosX, PosY, PosZ, Rotation, RotX, RotY, RotZ }

    Vector3 GetPosition()
    {
        return local ? transform.localPosition : transform.position;
    }

    Vector3 GetEulerRotation()
    {
        return local ? transform.localRotation.eulerAngles : transform.rotation.eulerAngles;
    }

    Quaternion GetRotation()
    {
        return local ? transform.localRotation : transform.rotation;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        prevPos = GetPosition();
        prevRot = GetEulerRotation();

        goToPos = prevPos;
        goToRot = prevRot;

        if (isOwner)
            StartCoroutine(SendMovement());
    }

    void Update()
    {
        if (!isOwner)
        {
            float t = Time.deltaTime / updateRate;

            if (sendPosition)
            {
                Vector3 currentPosition = GetPosition();

                if (posX && posY && posZ)
                {
                    currentPosition = Vector3.Lerp(currentPosition, goToPos, t);
                }
                else
                {
                    if (posX)
                        currentPosition.x = Mathf.Lerp(currentPosition.x, goToPos.x, t);
                    if (posY)
                        currentPosition.y = Mathf.Lerp(currentPosition.y, goToPos.y, t);
                    if (posZ)
                        currentPosition.z = Mathf.Lerp(currentPosition.z, goToPos.z, t);
                }

                if (local)
                    transform.localPosition = currentPosition;
                else
                    transform.position = currentPosition;
            }

            if(sendRotation)
            {
                
                if(rotX && rotY && rotZ)
                {
                    Quaternion currentRotation = GetRotation();

                    currentRotation = Quaternion.Lerp(currentRotation, Quaternion.Euler(goToRot), t);

                    if (local)
                        transform.localRotation = currentRotation;
                    else
                        transform.rotation = currentRotation;
                }
                else
                {
                    Vector3 targetRotation = GetEulerRotation();

                    if (rotX)
                        targetRotation.x =  goToRot.x;
                    if (rotY)
                        targetRotation.y = goToRot.y;
                    if (rotZ)
                        targetRotation.z = goToRot.z;

                    Quaternion finalRotation = Quaternion.Lerp(GetRotation(), Quaternion.Euler(targetRotation), t);

                    if (local)
                        transform.localRotation = finalRotation;
                    else
                        transform.rotation = finalRotation;
                }
            }
        }
    }

    private IEnumerator SendMovement()
    {
        Vector3 position = GetPosition();
        Vector3 rotation = GetEulerRotation();

        if ((position != prevPos) || rotation != prevRot)
        {
            using (RTData data = RTData.Get())
            {
                if (position != prevPos && sendPosition)
                {
                    if (posX && posY && posZ)
                    {
                        data.SetVector3((int)DataIndex.Position, position);
                    }
                    else
                    {
                        if (posX)
                            data.SetFloat((int)DataIndex.PosX, position.x);
                        if (posY)
                            data.SetFloat((int)DataIndex.PosY, position.y);
                        if (posZ)
                            data.SetFloat((int)DataIndex.PosZ, position.z);
                    }
                }

                if (rotation != prevRot && sendRotation)
                {
                    if (rotX && rotY && rotZ)
                    {
                        data.SetVector3((int)DataIndex.Rotation, rotation);
                    }
                    else
                    {
                        if (rotX)
                            data.SetFloat((int)DataIndex.RotX, rotation.x);
                        if (rotY)
                            data.SetFloat((int)DataIndex.RotY, rotation.y);
                        if (rotZ)
                            data.SetFloat((int)DataIndex.RotZ, rotation.z);
                    }
                }

                SendPacket(data, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED);
            }

            prevPos = position;
            prevRot = rotation;
        }

        yield return new WaitForSeconds(updateRate);
        StartCoroutine(SendMovement());
    }

    public override void OnPacket(RTPacket packet)
    {
        if (packet.Data.GetVector3((int)DataIndex.Position).HasValue)
        {
            goToPos = packet.Data.GetVector3((int)DataIndex.Position).Value;
        }
        else
        {
            if (packet.Data.GetFloat((int)DataIndex.PosX).HasValue)
                goToPos.x = packet.Data.GetFloat((int)DataIndex.PosX).Value;

            if (packet.Data.GetFloat((int)DataIndex.PosY).HasValue)
                goToPos.y = packet.Data.GetFloat((int)DataIndex.PosY).Value;

            if (packet.Data.GetFloat((int)DataIndex.PosZ).HasValue)
                goToPos.z = packet.Data.GetFloat((int)DataIndex.PosZ).Value;
        }

        if (packet.Data.GetVector3((int)DataIndex.Rotation).HasValue)
        {
            goToRot = packet.Data.GetVector3((int)DataIndex.Rotation).Value;
        }
        else
        {
            if (packet.Data.GetFloat((int)DataIndex.RotX).HasValue)
                goToRot.x = packet.Data.GetFloat((int)DataIndex.RotX).Value;

            if (packet.Data.GetFloat((int)DataIndex.RotY).HasValue)
                goToRot.y = packet.Data.GetFloat((int)DataIndex.RotY).Value;

            if (packet.Data.GetFloat((int)DataIndex.RotZ).HasValue)
                goToRot.z = packet.Data.GetFloat((int)DataIndex.RotZ).Value;
        }
    }
}