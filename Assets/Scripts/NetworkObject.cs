using UnityEngine;
using GameSparks.RT;
using System;
using System.Collections.Generic;

public class NetworkManager
{
    public enum DataIndex { NetworkId = 100, ObjectCode }

    private static Dictionary<string, NetworkObject> table = new Dictionary<string, NetworkObject>();

    public static void Register(NetworkObject networkObject)
    {
        if (table.ContainsKey(networkObject.networkId))
        {
            Debug.LogError("NetworkManager::Register - Duplicate key: " + networkObject.networkId);
            return;
        }

        table.Add(networkObject.networkId, networkObject);
    }

    public static void Remove(NetworkObject networkObject)
    {
        if (table.ContainsKey(networkObject.networkId))
        {
            table.Remove(networkObject.networkId);
        }
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
        GameSparksManager.Instance().SendRTData(OpCodes.NetworkObject, intent, data);
    }

    public static GameObject NetworkInstantiate(GameObject original, int owner = 0)
    {
        // Copy
        GameObject copy = UnityEngine.Object.Instantiate(original);

        // Get all the NetworkObjects
        NetworkObject[] nos = copy.GetComponentsInChildren<NetworkObject>();

        using (RTData data = RTData.Get())
        {
            // Set the prefab index
            data.SetInt(1, NetworkPrefabs.instance.prefabs.IndexOf(original));
            data.SetInt(2, owner);

            // Set all the network ids
            for (uint i = 0; i < nos.Length; i++)
            {
                nos[i].SetId(NetworkObject.GenerateId());
                nos[i].owner = owner;
                data.SetString(i + 3, nos[i].networkId);
            }

            // Send
            GameSparksManager.Instance().SendRTData(OpCodes.NetworkInstantiate, GameSparksRT.DeliveryIntent.RELIABLE, data);
        }

        return copy;
    }

    public static void OnNetworkInstantiate(RTPacket packet)
    {
        int prefabIndex = packet.Data.GetInt(1).Value;
        int owner = packet.Data.GetInt(2).Value;

        // Get the prefab
        GameObject prefab = NetworkPrefabs.instance.prefabs[prefabIndex];

        if (prefab != null)
        {
            // Copy
            Transform copy = UnityEngine.Object.Instantiate(prefab).transform;

            // Get all the NetworkObjects and set the ids
            NetworkObject[] nos = copy.GetComponentsInChildren<NetworkObject>();
            for (uint i = 0; i < nos.Length; i++)
            {
                nos[i].SetId(packet.Data.GetString(i + 3));
                nos[i].owner = owner;
            }
        }
    }
}

public abstract class NetworkObject : MonoBehaviour
{
    public string networkId;
    public int owner;

    public virtual void OnPacket(RTPacket packet)
    {

    }

    protected void SendPacket(RTData data, GameSparksRT.DeliveryIntent intent)
    {
        NetworkManager.SendPacket(this, data, intent);
    }

    void OnValidate()
    {
        networkId = GenerateId();
    }

    protected virtual void Awake()
    {
        SetId(networkId);       
    }

    public static string GenerateId()
    {
        return Guid.NewGuid().ToString("N");
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

    public void SetId(string newId)
    {
        if (string.IsNullOrEmpty(newId))
            return;

        NetworkManager.Remove(this);
        networkId = newId;
        NetworkManager.Register(this);
    }
}