using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameSparks.Core;
using GameSparks.Api.Responses;

public class LoginManager : MonoBehaviour
{
    public Text userStatus;
    public InputField userNameInput, passwordInput;
    public GameObject loginPanel;
    public Button loginBttn;

    void UpdateUserStatus()
    {
        userStatus.text = 
            (GS.Available ? "Connected" : "Disconnected") +
            ((GS.Authenticated && GameSparksManager.Instance().user != null) ? " | Logged in as " + GameSparksManager.Instance().user.displayName : " | Not authenticated");
    }

    void Start()
    {
        UpdateUserStatus();
        GS.GameSparksAvailable += (isAvailable) =>
        {
            UpdateUserStatus();
        };

        // we add a custom listener to the on-click delegate of the login button so we don't need to create extra methods
        loginBttn.onClick.AddListener(() => 
        {
            GameSparksManager.Instance().AuthenticateUser(userNameInput.text, passwordInput.text, OnRegistration, OnAuthentication);
        });
    }

    // <summary>
    /// this is called when a player is registered
    /// </summary>
    /// <param name="_resp">Resp.</param>
    private void OnRegistration(RegistrationResponse _resp)
    {
        UpdateUserStatus();

        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// This is called when a player is authenticated
    /// </summary>
    /// <param name="_resp">Resp.</param>
    private void OnAuthentication(AuthenticationResponse _resp)
    {
        UpdateUserStatus();

        SceneManager.LoadScene("MainMenu");
    }
}