using UnityEngine;
using System.Collections.Generic;

public class NetworkPrefabs : MonoBehaviour
{
    public static NetworkPrefabs instance;

    public List<GameObject> prefabs;

    void Awake ()
    {
        instance = this;
    }
}