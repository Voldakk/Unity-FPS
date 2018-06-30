using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;

public class LobbyManager : MonoBehaviour
{
    public Text matchDetails;
    public Button startButton;

    void Start()
    {
        startButton.onClick.AddListener(() => 
        {
            OnStartButton();
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

    void OnStartButton()
    {
        SceneManager.LoadScene("Main");
    }
}