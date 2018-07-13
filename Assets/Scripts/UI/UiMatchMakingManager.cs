using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Linq;
using GameSparks.Core;

public class UiMatchMakingManager : MonoBehaviour
{
    public Button soloMatchmakingButton, duoMatchmakingButton, cancelButton;
    public Text userStatus, matchDetails;
    public GameObject findPanel, searchingPanel, foundPanel;

    string currentMatchCode;

    void UpdateUserStatus()
    {
        userStatus.text = (GS.Available ? "Connected" : "Disconnected") + " | Logged in as " +
        GameSparksManager.Instance().user.displayName;
    }

    void Start()
    {
        UpdateUserStatus();
        GS.GameSparksAvailable += (isAvailable) =>
        {
            UpdateUserStatus();
        };

        findPanel.SetActive(true);
        searchingPanel.SetActive(false);
        foundPanel.SetActive(false);

        soloMatchmakingButton.onClick.AddListener(() =>
        {
            FindMatch("solo");
        });

        duoMatchmakingButton.onClick.AddListener(() =>
        {
            FindMatch("duo");
        });

        cancelButton.onClick.AddListener(() =>
        {
            findPanel.SetActive(true);
            searchingPanel.SetActive(false);

            GameSparksManager.Instance().CancelMatchmaking(currentMatchCode);
        });

        // this listener will update the text in the player-list field if no match was found
        GameSparks.Api.Messages.MatchNotFoundMessage.Listener = (message) => 
        {
            searchingPanel.SetActive(false);
            foundPanel.SetActive(true);
            matchDetails.text = "No Match Found...";
        };

        GameSparks.Api.Messages.MatchFoundMessage.Listener += OnMatchFound;
    }

    private void OnMatchFound(GameSparks.Api.Messages.MatchFoundMessage _message)
    {
        searchingPanel.SetActive(false);
        foundPanel.SetActive(true);

        Debug.Log("Match Found!...");
        StringBuilder sBuilder = new StringBuilder();
        sBuilder.AppendLine("Match Found...");
        sBuilder.AppendLine("Host URL:" + _message.Host);
        sBuilder.AppendLine("Port:" + _message.Port);
        sBuilder.AppendLine("Access Token:" + _message.AccessToken);
        sBuilder.AppendLine("MatchId:" + _message.MatchId);
        sBuilder.AppendLine("Opponents:" + _message.Participants.Count());
        sBuilder.AppendLine("_________________");
        sBuilder.AppendLine(); // we'll leave a space between the player-list and the match data
        foreach (GameSparks.Api.Messages.MatchFoundMessage._Participant player in _message.Participants)
        {
            sBuilder.AppendLine("Player:" + player.PeerId + " User Name:" + player.DisplayName); // add the player number and the display name to the list
        }
        matchDetails.text = sBuilder.ToString(); // set the string to be the player-list field

        GameSparksManager.Instance().StartNewRTSession(new RTSessionInfo(_message));
    }

    private void FindMatch(string code)
    {
        findPanel.SetActive(false);
        searchingPanel.SetActive(true);

        currentMatchCode = code;
        GameSparksManager.Instance().FindPlayers(currentMatchCode);
    }
}