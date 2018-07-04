using UnityEngine;
using UnityEngine.UI;
using GameSparks.RT;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    public Countdown matchStartCountdown;
    public GameObject inGameUi;

    public GameObject playerPrefab;

    public string sceneName;

    private GameObject[] spawnPoints;

    public Player[] playerList;

    public Player[] GetAllPlayers()
    {
        return playerList;
    }

    public Player Player()
    {
        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i].owner == GameSparksManager.PeerId())
            {
                return playerList[i];
            }
        }

        return null;
    }

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

    private static GameManager instance;

    public static GameManager Instance()
    {
        return instance;
    }

    public int NumPlayers()
    {
        return playerList.Length;
    }

    void Awake()
    {
        inGameUi.SetActive(false);
        instance = this;

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        playerList = new Player[GameSparksManager.Instance().GetSessionInfo().GetPlayerList().Count];
    }

    public void SetMatchStartTimer(float time)
    {
        Debug.Log("GameManager::SetMatchStartTimer - Match timer:" + time);

        if (matchStartCountdown != null)
            matchStartCountdown.SetCountdown(time);
    }

    public void StartMatch()
    {
        matchStartCountdown.gameObject.SetActive(false);
        inGameUi.SetActive(true);

        if (!IsHost)
            return;

        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        for (int i = 0; i < playerList.Length; i++)
        {
            int peerId = GameSparksManager.Instance().GetSessionInfo().GetPlayerList()[i].peerId;
            Vector3 spawnPos = spawnPoints[i].transform.position;
            Quaternion spawnRot = spawnPoints[i].transform.rotation;

            playerList[i] = NetworkManager.NetworkInstantiate(playerPrefab, peerId, spawnPos, spawnRot).GetComponent<Player>();
        }
    }

    #region Player updates

    /// <summary>
    /// Updates the players's position, rotation
    /// </summary>
    /// <param name="_packet">Packet received from other player</param>
    public void UpdatePlayerPosition(RTPacket _packet)
    {
        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i].owner == _packet.Sender)
            {
                playerList[i].SetPosition(_packet.Data.GetVector3(1).Value, _packet.Data.GetVector2(2).Value);
                break;
            }
        }
    }


    public void PlayerWeaponUpdate(Player player, RTData data)
    {
        GameSparksManager.Instance().SendRTData(OpCodes.PlayerWeapon, GameSparksRT.DeliveryIntent.RELIABLE, data);
    }

    public void OnPlayerWeaponUpdate(RTPacket _packet)
    {
        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i].owner == _packet.Sender)
            {
                playerList[i].WeaponBehaviour.OnWeaponUpdate(_packet);
                break;
            }
        }
    }


    public void SetPlayerWeapon(int index)
    {
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, index);

            GameSparksManager.Instance().SendRTData(OpCodes.PlayerSetWeapon, GameSparksRT.DeliveryIntent.RELIABLE, data);
        }
    }

    public void OnPlayerSetWeapon(RTPacket _packet)
    {
        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i].owner == _packet.Sender)
            {
                playerList[i].WeaponBehaviour.SetWeapon(_packet.Data.GetInt(1).Value);
                break;
            }
        }
    }


    public void DamagePlayer(Player player, float amount)
    {
        DamagePlayer(player.owner, amount);
    }

    public void DamagePlayer(int peerId, float amount)
    {
        Debug.LogFormat("GameManager::DamagePlayer - Player {0} damaging player {1} for {2} damage", GameSparksManager.PeerId(), peerId, amount);

        using (RTData data = RTData.Get())
        {
            data.SetInt(1, peerId);
            data.SetFloat(2, amount);

            GameSparksManager.Instance().SendRTData(OpCodes.PlayerDamage, GameSparksRT.DeliveryIntent.RELIABLE, data);
        }
    }

    public void OnPlayerDamage(RTPacket _packet)
    {
        Debug.LogFormat("GameManager::OnPlayerDamage - Player {0} damaging player {1} for {2} damage", _packet.Sender, _packet.Data.GetInt(1).Value, _packet.Data.GetFloat(2).Value);

        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i].owner == _packet.Data.GetInt(1).Value)
            {
                playerList[i].Health.Damage(_packet.Data.GetFloat(2).Value);
                break;
            }
        }
    }

    #endregion
}