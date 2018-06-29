using UnityEngine;
using GameSparks.Api.Responses;
using GameSparks.Core;

public class AutoConnect : MonoBehaviour
{
    public string matchShortCode;
    public string userName, password;
    public bool useDeviceAuth;
    private RTSessionInfo tempRTSessionInfo;

    void Awake ()
    {
        GameSparks.Api.Messages.MatchNotFoundMessage.Listener = (message) => 
        {
            Debug.Log("No Match Found");
        };

        GameSparks.Api.Messages.MatchFoundMessage.Listener += OnMatchFound;

        GS.GameSparksAvailable += (isAvailable) => 
        {
            if (isAvailable && GameSparksManager.PeerId() == -1)
            {
                if(useDeviceAuth)
                    GameSparksManager.Instance().DeviceAuthentication(OnAuthentication);
                else
                    GameSparksManager.Instance().AuthenticateUser(userName, password, OnRegistration, OnAuthentication);
            }
        };        
    }

    private void OnRegistration(RegistrationResponse _resp)
    {
        GameSparksManager.Instance().FindPlayers(matchShortCode);
    }

    private void OnAuthentication(AuthenticationResponse _resp)
    {
        GameSparksManager.Instance().FindPlayers(matchShortCode);
    }

    private void OnMatchFound(GameSparks.Api.Messages.MatchFoundMessage _message)
    {
        tempRTSessionInfo = new RTSessionInfo(_message);
        GameSparksManager.Instance().StartNewRTSession(tempRTSessionInfo);

        Debug.Log("Match Found");
    }
}
