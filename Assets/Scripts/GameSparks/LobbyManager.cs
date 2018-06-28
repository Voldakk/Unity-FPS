using UnityEngine;
using UnityEngine.UI;
using GameSparks.Core;
using GameSparks.Api.Responses;
using System.Text;
using System.Linq;

public class LobbyManager : MonoBehaviour
{
    public Text userId, connectionStatus;
    public InputField userNameInput, passwordInput;
    public GameObject loginPanel;
    public Button loginBttn, soloMatchmakingBttn, duoMatchmakingBttn, startGameBttn;
    public Text matchDetails;
    public GameObject matchDetailsPanel;

    private RTSessionInfo tempRTSessionInfo;

    void Start()
    {

        // we won't start with a user logged in so lets show this also
        userId.text = "No User Logged In...";

        // we won't immediately have connection, so at the start of the lobby we
        // will set the connection status to show this
        connectionStatus.text = "No Connection...";
        GS.GameSparksAvailable += (isAvailable) => {
            if (isAvailable)
            {
                connectionStatus.text = "GameSparks Connected...";
            }
            else
            {
                connectionStatus.text = "GameSparks Disconnected...";
            }
        };
        // only the login panel and login button is needed at the start of the scene, so disable any other objects //
        matchDetailsPanel.SetActive(false);
        soloMatchmakingBttn.gameObject.SetActive(false);
        duoMatchmakingBttn.gameObject.SetActive(false);
        startGameBttn.gameObject.SetActive(false);
        // we add a custom listener to the on-click delegate of the login button so we don't need to create extra methods //
        loginBttn.onClick.AddListener(() => {
            GameSparksManager.Instance().AuthenticateUser(userNameInput.text, passwordInput.text, OnRegistration, OnAuthentication);
        });

        soloMatchmakingBttn.onClick.AddListener(() => {
            GameSparksManager.Instance().FindPlayers("solo");
            matchDetails.text = "Searching For Players...";
        });
        duoMatchmakingBttn.onClick.AddListener(() => {
            GameSparksManager.Instance().FindPlayers("duo");
            matchDetails.text = "Searching For Players...";
        });

        // this listener will update the text in the player-list field if no match was found //
        GameSparks.Api.Messages.MatchNotFoundMessage.Listener = (message) => {
            matchDetails.text = "No Match Found...";
        };

        GameSparks.Api.Messages.MatchFoundMessage.Listener += OnMatchFound;

        // this is a listener for the startGameBttn. Onclick, we will will pass the stored RTSessionInfo to the GameSparksManager to create a new RT session //
        startGameBttn.onClick.AddListener(() => {
            GameSparksManager.Instance().StartNewRTSession(tempRTSessionInfo);
        });
    }

    // <summary>
    /// this is called when a player is registered
    /// </summary>
    /// <param name="_resp">Resp.</param>
    private void OnRegistration(RegistrationResponse _resp)
    {
        userId.text = "User ID: " + _resp.UserId;
        connectionStatus.text = "New User Registered...";
        loginPanel.SetActive(false);
        loginBttn.gameObject.SetActive(false);
        soloMatchmakingBttn.gameObject.SetActive(true);
        duoMatchmakingBttn.gameObject.SetActive(true);
        matchDetailsPanel.SetActive(true);
    }
    /// <summary>
    /// This is called when a player is authenticated
    /// </summary>
    /// <param name="_resp">Resp.</param>
    private void OnAuthentication(AuthenticationResponse _resp)
    {
        userId.text = "User ID: " + _resp.UserId;
        connectionStatus.text = "User Authenticated...";
        loginPanel.SetActive(false);
        loginBttn.gameObject.SetActive(false);
        soloMatchmakingBttn.gameObject.SetActive(true);
        duoMatchmakingBttn.gameObject.SetActive(true);
        matchDetailsPanel.SetActive(true);
    }

    private void OnMatchFound(GameSparks.Api.Messages.MatchFoundMessage _message)
    {
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

        tempRTSessionInfo = new RTSessionInfo(_message); // we'll store the match data until we need to create an RT session instance
        soloMatchmakingBttn.gameObject.SetActive(false);
        duoMatchmakingBttn.gameObject.SetActive(false);
        startGameBttn.gameObject.SetActive(true);
    }
}