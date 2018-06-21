using GameSparks.RT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;

    public Transform[] spawnPoints;

    private Player[] playerList;
    public Player[] GetAllPlayers()
    {
        return playerList;
    }

    private static GameManager instance;
    public static GameManager Instance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Updates the opponent's position, rotation
    /// </summary>
    /// <param name="_packet">Packet Received From Opponent Player</param>
    public void UpdateOpponentPosition(RTPacket _packet)
    {
        for (int i = 0; i < playerList.Length; i++)
        {
            // Check the name of the tank matches the sender
            if (playerList[i].name == _packet.Sender.ToString())
            { 
                playerList[i].SetPosition(_packet.Data.GetVector3(1).Value, _packet.Data.GetVector3(2).Value);
                break;
            }
        }
    }

    void Start()
    {
        #region Setup Players

        GameObject p = GameObject.Find("Player");
        if (p != null)
            Destroy(p);

        playerList = new Player[(int)GameSparksManager.Instance().GetSessionInfo().GetPlayerList().Count];

        for (int i = 0; i < playerList.Length; i++)
        {
            playerList[i] = Instantiate(playerPrefab).GetComponent<Player>();

            playerList[i].transform.position = spawnPoints[i].position;
            playerList[i].transform.rotation = spawnPoints[i].rotation;

            playerList[i].name = GameSparksManager.Instance().GetSessionInfo().GetPlayerList()[i].peerId.ToString();

            if (GameSparksManager.Instance().GetSessionInfo().GetPlayerList()[i].peerId == GameSparksManager.Instance().GetRTSession().PeerId)
            {
                playerList[i].SetIsPlayer(true);
            }
            else
            {
                playerList[i].SetIsPlayer(false);
            }
        }

        #endregion
    }
}