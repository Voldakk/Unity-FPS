using UnityEngine;

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
}