using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class LobbyManager : MonoBehaviour
{
    public Text matchDetails;
    public Button readyButton;

    public static LobbyManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        readyButton.onClick.AddListener(() => 
        {
            OnReadyButton();
        });

        // Player list
        StringBuilder sBuilder = new StringBuilder();
        var playerList = GameSparksManager.Instance().GetSessionInfo().GetPlayerList();

        sBuilder.AppendLine("Players:");
        foreach (var player in playerList)
        {
            sBuilder.AppendLine(player.displayName);
        }

        matchDetails.text = sBuilder.ToString();
    }

    void OnReadyButton()
    {
        GameSparksManager.Instance().SetPlayerReady(true);
        readyButton.interactable = false;
    }
}