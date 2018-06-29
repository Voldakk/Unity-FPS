using UnityEngine;
using GameSparks.RT;
using System.Collections;
using System.Collections.Generic;
using System;

enum OpCodes { None, Chat, PlayerPosition, PlayerDamage, PlayerSetWeapon, PlayerWeapon, NpcPosition, TimeStamp = 101, ClockSync = 102 };

public class GameManager : MonoBehaviour
{

    public GameObject playerPrefab;

    public string sceneName;

    private GameObject[] spawnPoints;

    private Player[] playerList;

    private List<Enemy> enemies;
    private Dictionary<string, Enemy> enemyTable;

    public Player[] GetAllPlayers()
    {
        return playerList;
    }

    public Player Player()
    {
        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i].peerId == GameSparksManager.PeerId())
            {
                return playerList[i];
            }
        }

        return null;
    }

    private static GameManager instance;

    public bool IsHost
    {
        get
        {
            return GameSparksManager.PeerId() == 1;
        }
        private set
        {
           
        }
    }

    public static GameManager Instance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;

        enemies = new List<Enemy>();
        enemyTable = new Dictionary<string, Enemy>();

        if (!string.IsNullOrEmpty(sceneName) && FindObjectOfType<AutoConnect>() == null)
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    void Start()
    {
        StartCoroutine(SendTimeStamp());

        if (!string.IsNullOrEmpty(sceneName) && FindObjectOfType<AutoConnect>() == null)
            SetupPlayers();
    }

    public void SetupPlayers()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        GameObject p = GameObject.Find("Player");
        if (p != null)
            Destroy(p);

        playerList = new Player[GameSparksManager.Instance().GetSessionInfo().GetPlayerList().Count];

        for (int i = 0; i < playerList.Length; i++)
        {
            playerList[i] = Instantiate(playerPrefab).GetComponent<Player>();

            playerList[i].transform.position = spawnPoints[i].transform.position;
            playerList[i].transform.rotation = spawnPoints[i].transform.rotation;

            playerList[i].peerId = GameSparksManager.Instance().GetSessionInfo().GetPlayerList()[i].peerId;
            playerList[i].name = GameSparksManager.Instance().GetSessionInfo().GetPlayerList()[i].peerId.ToString();

            if (playerList[i].peerId == GameSparksManager.PeerId())
            {
                playerList[i].Initialize(true);
                playerList[i].gameObject.SetActive(false);
            }
            else
            {
                playerList[i].Initialize(false);
            }
        }
    }

    /// <summary>
    /// Updates the players's position, rotation
    /// </summary>
    /// <param name="_packet">Packet received from other player</param>
    public void UpdatePlayerPosition(RTPacket _packet)
    {
        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i].peerId == _packet.Sender)
            {
                playerList[i].SetPosition(_packet.Data.GetVector3(1).Value, _packet.Data.GetVector2(2).Value);
                break;
            }
        }
    }

    public void PlayerWeaponUpdate(Player player, RTData data)
    {
        GameSparksManager.Instance().GetRTSession().SendData((int)OpCodes.PlayerWeapon, GameSparksRT.DeliveryIntent.RELIABLE, data);
    }


    public void OnPlayerSetWeapon(RTPacket _packet)
    {
        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i].peerId == _packet.Sender)
            {
                playerList[i].WeaponBehaviour.SetWeapon(_packet.Data.GetInt(1).Value);
                break;
            }
        }
    }

    public void SetPlayerWeapon(int index)
    {
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, index);

            GameSparksManager.Instance().GetRTSession().SendData((int)OpCodes.PlayerSetWeapon, GameSparksRT.DeliveryIntent.RELIABLE, data);
        }
    }

    public void OnPlayerWeaponUpdate(RTPacket _packet)
    {
        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i].peerId == _packet.Sender)
            {
                playerList[i].WeaponBehaviour.OnWeaponUpdate(_packet);
                break;
            }
        }
    }

    public void DamagePlayer(Player player, float amount)
    {
        DamagePlayer(player.peerId, amount);
    }

    public void DamagePlayer(int peerId, float amount)
    {
        Debug.LogFormat("GameManager::DamagePlayer - Player {0} damaging player {1} for {2} damage", GameSparksManager.PeerId(), peerId, amount);

        using (RTData data = RTData.Get())
        {
            data.SetInt(1, peerId);
            data.SetFloat(2, amount);

            GameSparksManager.Instance().GetRTSession().SendData((int)OpCodes.PlayerDamage, GameSparksRT.DeliveryIntent.RELIABLE, data);
        }
    }

    public void OnPlayerDamage(RTPacket _packet)
    {
        Debug.LogFormat("GameManager::OnPlayerDamage - Player {0} damaging player {1} for {2} damage", _packet.Sender, _packet.Data.GetInt(1).Value, _packet.Data.GetFloat(2).Value);

        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i].peerId == _packet.Data.GetInt(1).Value)
            {
                playerList[i].Health.Damage(_packet.Data.GetFloat(2).Value);
                break;
            }
        }
    }

    public int NumPlayers()
    {
        return playerList.Length;
    }

    public void RegisterEnemy(Enemy enemy)
    {
        enemyTable.Add(enemy.id, enemy);
    }

    public void SendNpcPosition(Enemy enemy)
    {
        using (RTData data = RTData.Get())
        {
            data.SetString(1, enemy.id);
            data.SetVector3(2, enemy.transform.position);
            data.SetVector2(3, new Vector2(enemy.transform.rotation.eulerAngles.x, enemy.transform.rotation.eulerAngles.y));

            GameSparksManager.Instance().GetRTSession().SendData((int)OpCodes.NpcPosition, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
    }

    public void OnNpcPositionUpdate(RTPacket packet)
    {
        Enemy enemy = enemyTable[packet.Data.GetString(1)];
        if(enemy != null)
        {
            enemy.UpdateTransform(packet.Data.GetVector3(2).Value, packet.Data.GetVector2(3).Value);
        }
    }

    /// <summary>
    /// Sends a Unix timestamp in milliseconds to the server
    /// </summary>
    private IEnumerator SendTimeStamp()
    {

        // send a packet with our current time first //
        using (RTData data = RTData.Get())
        {
            data.SetLong(1, (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds); // get the current time as unix timestamp
            GameSparksManager.Instance().GetRTSession().SendData(101, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE, data, new int[] { 0 }); // send to peerId -> 0, which is the server
        }
        yield return new WaitForSeconds(5f); // wait 5 seconds
        StartCoroutine(SendTimeStamp()); // send the timestamp again
    }

    DateTime serverClock;
    private int timeDelta, latency, roundTrip;

    /// <summary>
    /// Calculates the time-difference between the client and server
    /// </summary>
    public void CalculateTimeDelta(RTPacket _packet)
    {
        // calculate the time taken from the packet to be sent from the client and then for the server to return it //
        roundTrip = (int)((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds - _packet.Data.GetLong(1).Value);
        latency = roundTrip / 2; // the latency is half the round-trip time
        // calculate the server-delta from the server time minus the current time
        int serverDelta = (int)(_packet.Data.GetLong(2).Value - (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds);
        timeDelta = serverDelta + latency; // the time-delta is the server-delta plus the latency
    }

    /// <summary>
    /// Syncs the local clock to server-time
    /// </summary>
    /// <param name="_packet">Packet.</param>
    public void SyncClock(RTPacket _packet)
    {
        DateTime dateNow = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc); // get the current time
        serverClock = dateNow.AddMilliseconds(_packet.Data.GetLong(1).Value + timeDelta).ToLocalTime(); // adjust current time to match clock from server
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 50, 400, 30), "Server Time: " + serverClock.TimeOfDay);
        GUI.Label(new Rect(10, 70, 400, 30), "Latency: " + latency.ToString() + "ms");
        GUI.Label(new Rect(10, 90, 400, 30), "Time Delta: " + timeDelta.ToString() + "ms");
    }
}