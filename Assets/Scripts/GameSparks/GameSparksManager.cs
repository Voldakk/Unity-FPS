using UnityEngine;
using UnityEngine.SceneManagement;

using GameSparks.Core;
using GameSparks.Api.Messages;
using GameSparks.Api.Responses;
using GameSparks.RT;

using System.Collections.Generic;

public class GameSparksManager : MonoBehaviour
{
    public string sceneName;

    /// <summary>The GameSparks Manager singleton</summary>
    private static GameSparksManager instance = null;

    /// <summary>This method will return the current instance of this class </summary>
    public static GameSparksManager Instance()
    {
        if (instance != null)
        {
            // return the singleton if the instance has been setup
            return instance; 
        }
        else
        { 
            // otherwise return an error
            Debug.LogError("GSM| GameSparksManager Not Initialized...");
        }
        return null;
    }

    void Awake()
    {
        instance = this; // if not, give it a reference to this class...
        DontDestroyOnLoad(this.gameObject); // and make this object persistent as we load new scenes
    }

    #region Login & Registration
    public delegate void AuthCallback(AuthenticationResponse _authresp2);
    public delegate void RegCallback(RegistrationResponse _authResp);
    /// <summary>
    /// Sends an authentication request or registration request to GS.
    /// </summary>
    /// <param name="_callback1">Auth-Response</param>
    /// <param name="_callback2">Registration-Response</param>
    public void AuthenticateUser(string _userName, string _password, RegCallback _regcallback, AuthCallback _authcallback)
    {
        new GameSparks.Api.Requests.RegistrationRequest()
            // this login method first attempts a registration //
            // if the player is not new, we will be able to tell as the registrationResponse has a bool 'NewPlayer' which we can check
            // for this example we use the user-name was the display name also //
            .SetDisplayName(_userName)
            .SetUserName(_userName)
            .SetPassword(_password)
            .Send((regResp) =>
            {
                if (!regResp.HasErrors)
                { // if we get the response back with no errors then the registration was successful
                    Debug.Log("GSM| Registration Successful...");
                    _regcallback(regResp);
                }
                else
                {
                    // if we receive errors in the response, then the first thing we check is if the player is new or not
                    if (!(bool)regResp.NewPlayer) // player already registered, lets authenticate instead
                    {
                        Debug.LogWarning("GSM| Existing User, Switching to Authentication");
                        new GameSparks.Api.Requests.AuthenticationRequest()
                            .SetUserName(_userName)
                            .SetPassword(_password)
                            .Send((authResp) =>
                            {
                                if (!authResp.HasErrors)
                                {
                                    Debug.Log("Authentication Successful...");
                                    _authcallback(authResp);
                                }
                                else
                                {
                                    Debug.LogWarning("GSM| Error Authenticating User \n" + authResp.Errors.JSON);
                                }
                            });
                    }
                    else
                    {
                        // if there is another error, then the registration must have failed
                        Debug.LogWarning("GSM| Error Authenticating User \n" + regResp.Errors.JSON);
                    }
                }
            });
    }
    
    public void DeviceAuthentication(AuthCallback _authcallback)
    {
        new GameSparks.Api.Requests.DeviceAuthenticationRequest().Send((response) => 
        {
            _authcallback(response);
        });
    }
    
    #endregion

    #region Matchmaking Request
    /// <summary>
    /// This will request a match between as many players you have set in the match.
    /// When the max number of players is found each player will receive the MatchFound message
    /// </summary>
    public void FindPlayers(string shortCode)
    {
        Debug.Log("GSM| Attempting Matchmaking...");
        new GameSparks.Api.Requests.MatchmakingRequest()
            .SetMatchShortCode(shortCode)
            .SetSkill(0)
            .Send((response) => {
                if (response.HasErrors)
                {
                    Debug.LogError("GSM| MatchMaking Error \n" + response.Errors.JSON);
                }
            });
    }
    #endregion

    private GameSparksRTUnity gameSparksRTUnity;

    public GameSparksRTUnity GetRTSession()
    {
        return gameSparksRTUnity;
    }

    private RTSessionInfo sessionInfo;

    public RTSessionInfo GetSessionInfo()
    {
        return sessionInfo;
    }

    public void StartNewRTSession(RTSessionInfo _info)
    {
        Debug.Log("GSM| Creating New RT Session Instance...");
        sessionInfo = _info;
        gameSparksRTUnity = this.gameObject.AddComponent<GameSparksRTUnity>(); // Adds the RT script to the game
        // In order to create a new RT game we need a 'FindMatchResponse' //
        // This would usually come from the server directly after a successful MatchmakingRequest //
        // However, in our case, we want the game to be created only when the first player decides using a button //
        // therefore, the details from the response is passed in from the gameInfo and a mock-up of a FindMatchResponse //
        // is passed in. //
        GSRequestData mockedResponse = new GSRequestData()
                                            .AddNumber("port", (double)_info.GetPortID())
                                            .AddString("host", _info.GetHostURL())
                                            .AddString("accessToken", _info.GetAccessToken()); // construct a dataset from the game-details

        FindMatchResponse response = new FindMatchResponse(mockedResponse); // create a match-response from that data and pass it into the game-config
        // So in the game-config method we pass in the response which gives the instance its connection settings //
        // In this example, I use a lambda expression to pass in actions for 
        // OnPlayerConnect, OnPlayerDisconnect, OnReady and OnPacket actions //
        // These methods are self-explanatory, but the important one is the OnPacket Method //
        // this gets called when a packet is received //

        gameSparksRTUnity.Configure(response,
            (peerId) => { OnPlayerConnectedToGame(peerId); },
            (peerId) => { OnPlayerDisconnected(peerId); },
            (ready) => { OnRTReady(ready); },
            (packet) => { OnPacketReceived(packet); });
        gameSparksRTUnity.Connect(); // when the config is set, connect the game

    }

    private void OnPlayerConnectedToGame(int _peerId)
    {
        Debug.Log("GSM| Player Connected, " + _peerId);
    }

    private void OnPlayerDisconnected(int _peerId)
    {
        Debug.Log("GSM| Player Disconnected, " + _peerId);
    }

    private void OnRTReady(bool _isReady)
    {
        if (_isReady)
        {
            Debug.Log("GSM| RT Session Connected...");

            SceneManager.LoadScene("Lobby");
        }

    }

    private void OnPacketReceived(RTPacket _packet)
    {
        if (GameManager.Instance() != null)
            GameManager.Instance().PacketReceived(_packet.PacketSize);

        switch ((OpCodes)_packet.OpCode)
        {
            case OpCodes.Chat:
                break;

            case OpCodes.PlayerPosition:
                GameManager.Instance().UpdatePlayerPosition(_packet);
                break;

            case OpCodes.PlayerDamage:
                GameManager.Instance().OnPlayerDamage(_packet);
                break;

            case OpCodes.PlayerSetWeapon:
                GameManager.Instance().OnPlayerSetWeapon(_packet);
                break;

            case OpCodes.PlayerWeapon:
                GameManager.Instance().OnPlayerWeaponUpdate(_packet);
                break;

            case OpCodes.NpcPosition:
                GameManager.Instance().OnNpcPositionUpdate(_packet);
                break;

            case OpCodes.TimeStamp:
                //GameManager.Instance().CalculateTimeDelta(_packet);
                break;

            case OpCodes.ClockSync:
                //GameManager.Instance().SyncClock(_packet);
                break;
        }
    }

    public static int PeerId()
    {
        if(Instance() != null && Instance().GetRTSession() != null && Instance().GetRTSession().PeerId.HasValue)
            return Instance().GetRTSession().PeerId.Value;

        return -1;
    }
}

public class RTSessionInfo
{
    private string hostURL;
    public string GetHostURL() { return this.hostURL; }
    private string acccessToken;
    public string GetAccessToken() { return this.acccessToken; }
    private int portID;
    public int GetPortID() { return this.portID; }
    private string matchID;
    public string GetMatchID() { return this.matchID; }

    private List<RTPlayer> playerList = new List<RTPlayer>();
    public List<RTPlayer> GetPlayerList()
    {
        return playerList;
    }

    /// <summary>
    /// Creates a new RTSession object which is held until a new RT session is created
    /// </summary>
    /// <param name="_message">Message.</param>
    public RTSessionInfo(MatchFoundMessage _message)
    {
        portID = (int)_message.Port;
        hostURL = _message.Host;
        acccessToken = _message.AccessToken;
        matchID = _message.MatchId;
        
        // We loop through each participant and get their peerId and display name
        foreach (MatchFoundMessage._Participant p in _message.Participants)
        {
            playerList.Add(new RTPlayer(p.DisplayName, p.Id, (int)p.PeerId));
        }
    }

    public class RTPlayer
    {
        public RTPlayer(string _displayName, string _id, int _peerId)
        {
            displayName = _displayName;
            id = _id;
            peerId = _peerId;
        }

        public string displayName;
        public string id;
        public int peerId;
        public bool isOnline;
    }
}