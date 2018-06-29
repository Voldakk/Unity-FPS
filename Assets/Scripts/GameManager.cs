using UnityEngine;
using GameSparks.RT;
using System.Collections.Generic;

enum OpCodes { None, Chat, PlayerPosition, PlayerDamage, PlayerSetWeapon, PlayerWeapon, NpcPosition };

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
}