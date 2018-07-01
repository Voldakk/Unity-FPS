using UnityEngine;
using UnityEngine.SceneManagement;

using GameSparks.Core;
using GameSparks.Api.Messages;
using GameSparks.Api.Responses;
using GameSparks.RT;

using System.Collections.Generic;

public enum OpCodes
{
    None,
    PlayerPosition, PlayerDamage, PlayerSetWeapon, PlayerWeapon, NetworkObject,
    TimeStamp = 101, ClockSync = 102,
    PlayerReady = 200, LoadGame, PlayerLoaded, MatchStartTimer, StartMatch
};

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

    public static int PeerId()
    {
        if (Instance() != null && Instance().GetRTSession() != null && Instance().GetRTSession().PeerId.HasValue)
            return Instance().GetRTSession().PeerId.Value;

        return -1;
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
            .Send((response) =>
            {
                if (response.HasErrors)
                {
                    Debug.LogError("GSM| MatchMaking Error \n" + response.Errors.JSON);
                }
            });
    }
    #endregion

    #region Session

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

    private void OnRTReady(bool _isReady)
    {
        if (_isReady)
        {
            Debug.Log("GSM| RT Session Connected...");

            SceneManager.LoadScene("Lobby");
        }

    }

    private void OnPlayerConnectedToGame(int _peerId)
    {
        Debug.Log("GSM| Player Connected, " + _peerId);
    }

    private void OnPlayerDisconnected(int _peerId)
    {
        Debug.Log("GSM| Player Disconnected, " + _peerId);
    }

    #endregion

    # region Packet Analytics

    public int packetSize_sent;
    public int totalSent = 0;

    /// <summary>
    /// Sends RTData and records the packet size
    /// </summary>
    /// <param name="_opcode">Opcode.</param>
    /// <param name="_intent">Intent.</param>
    /// <param name="_data">Data.</param>
    /// <param name="_targetPeers">Target peers.</param>
    public void SendRTData(OpCodes opcode, GameSparksRT.DeliveryIntent intent, RTData data, int[] targetPeers)
    {
        packetSize_sent = GetRTSession().SendData((int)opcode, intent, data, targetPeers);
        totalSent += packetSize_sent;
    }
    public void SendRTData(OpCodes opcode, GameSparksRT.DeliveryIntent intent, int[] targetPeers)
    {
        using (RTData data = RTData.Get())
        {
            SendRTData(opcode, intent, data, targetPeers);
        }
    }

    /// <summary>
    /// Sends RTData to all players
    /// </summary>
    /// <param name="_opcode">Opcode.</param>
    /// <param name="_intent">Intent.</param>
    /// <param name="_data">Data.</param>
    public void SendRTData(OpCodes opcode, GameSparksRT.DeliveryIntent intent, RTData data)
    {
        packetSize_sent = GetRTSession().SendData((int)opcode, intent, data);
        totalSent += packetSize_sent;
    }

    public void SendRTData(OpCodes opcode, GameSparksRT.DeliveryIntent intent)
    {
        using (RTData data = RTData.Get())
        {
            SendRTData(opcode, intent, data);
        }
    }

    public int packetSize_incoming;
    public int totalReceived = 0;

    /// <summary>
    /// Records the incoming packet size
    /// </summary>
    /// <param name="_packetSize">Packet size.</param>
    public void PacketReceived(int _packetSize)
    {
        packetSize_incoming = _packetSize;
        totalReceived += packetSize_incoming;
    }

    #endregion

    private void OnPacketReceived(RTPacket packet)
    {
        PacketReceived(packet.PacketSize);

        switch ((OpCodes)packet.OpCode)
        {
            case OpCodes.None:
                break;

            case OpCodes.PlayerPosition:
                GameManager.Instance().UpdatePlayerPosition(packet);
                break;

            case OpCodes.PlayerDamage:
                GameManager.Instance().OnPlayerDamage(packet);
                break;

            case OpCodes.PlayerSetWeapon:
                GameManager.Instance().OnPlayerSetWeapon(packet);
                break;

            case OpCodes.PlayerWeapon:
                GameManager.Instance().OnPlayerWeaponUpdate(packet);
                break;

            case OpCodes.NetworkObject:
                NetworkManager.OnPacket(packet);
                break;

            case OpCodes.TimeStamp:
                // GameManager.Instance().CalculateTimeDelta(packet);
                break;

            case OpCodes.ClockSync:
                // GameManager.Instance().SyncClock(packet);
                break;

            case OpCodes.PlayerReady:
                // ---
                break;

            case OpCodes.LoadGame:
                OnLoadGame();
                break;

            case OpCodes.PlayerLoaded:
                // ---
                break;

            case OpCodes.MatchStartTimer:
                SetMatchTimer(packet);
                break;

            case OpCodes.StartMatch:
                OnStartMatch();
                break;
        }
    }

    public void SetPlayerReady(bool value)
    {
        Debug.Log("GameSparksManager::SetReadyState");
        if (value == true)
        {
            using (RTData data = RTData.Get())
            {
                data.SetInt(1, PeerId());
                SendRTData(OpCodes.PlayerReady, GameSparksRT.DeliveryIntent.RELIABLE, new int[] { 0 });
            }
        }
        else
        {

        }
    }

    void OnLoadGame()
    {
        Debug.Log("GameSparksManager::LoadGame");
        SceneManager.LoadScene("Main");
    }

    public void SetPlayerLoaded()
    {
        Debug.Log("GameSparksManager::SetPlayerLoaded");
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, PeerId());
            SendRTData(OpCodes.PlayerLoaded, GameSparksRT.DeliveryIntent.RELIABLE, new int[] { 0 });
        }
    }

    void SetMatchTimer(RTPacket packet)
    {
        Debug.Log("GameSparksManager::SetMatchTimer - " + packet.Data.GetInt(1).Value);
        GameManager.Instance().SetMatchStartTimer(packet.Data.GetInt(1).Value / 1000f);
    }

    void OnStartMatch()
    {
        Debug.Log("GameSparksManager::StartMatch");
        GameManager.Instance().StartMatch();
    }
}