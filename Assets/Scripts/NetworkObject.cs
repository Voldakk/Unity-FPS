using GameSparks.RT;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager
{
    public enum DataIndex { NetworkId = 100, ObjectCode }

    private static Dictionary<string, NetworkObject> table = new Dictionary<string, NetworkObject>();

    public static void Register(NetworkObject networkObject)
    {
        if(table.ContainsKey(networkObject.networkId))
        {
            Debug.LogError("NetworkManager::Register - Duplicate key: " + networkObject.networkId);
            return;
        }

        table.Add(networkObject.networkId, networkObject);
    }

    public static void OnPacket(RTPacket packet)
    {
        NetworkObject networkObject = table[packet.Data.GetString((int)DataIndex.NetworkId)];
        if (networkObject != null)
        {
            networkObject.OnPacket(packet);
        }
    }

    public static void SendPacket(NetworkObject networkObject, RTData data, GameSparksRT.DeliveryIntent intent)
    {
        data.SetString((int)DataIndex.NetworkId, networkObject.networkId);
        GameManager.Instance().SendRTData(OpCodes.NetworkObject, intent, data);
    }
}

public abstract class NetworkObject : MonoBehaviour
{
    public string networkId;

    public abstract void OnPacket(RTPacket packet);

    protected void SendPacket(RTData data, GameSparksRT.DeliveryIntent intent)
    {
        NetworkManager.SendPacket(this, data, intent);
    }

    protected virtual void OnValidate()
    {
        networkId = Guid.NewGuid().ToString("N");
    }

    protected virtual void Awake()
    {
        NetworkManager.Register(this);
    }

    protected void SendInt(uint index, int value)
    {
        using (RTData data = RTData.Get())
        {
            data.SetInt(index, value);
            SendPacket(data, GameSparksRT.DeliveryIntent.RELIABLE);
        }
    }

    protected void SendFloat(uint index, float value)
    {
        using (RTData data = RTData.Get())
        {
            data.SetFloat(index, value);
            SendPacket(data, GameSparksRT.DeliveryIntent.RELIABLE);
        }
    }
}