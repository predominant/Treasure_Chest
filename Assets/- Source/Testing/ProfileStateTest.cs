using UnityEngine;
using System.Collections;
using Soomla.Profile;
using GameSparks.Core;

public class ProfileStateTest : MonoBehaviour
{

    public UnityEngine.UI.Text m_Text = null;

    void Start()
    {
    }

    void Update()
    {
        string profileState = string.Empty;
        if( !GS.Available )
        {
            if( LoginMenuController.GSReconnecting )
                profileState = "Server Reconnecting";
            else
                profileState = "Server Unavailable";
        }
        else
        {
            switch (ProfileManager.State)
            {
                case ProfileManager.ProviderState.LoggedIn:
                    profileState = "Provider logged in";
                    break;
                case ProfileManager.ProviderState.LoggingIn:
                    profileState = "Provider logging in";
                    break;
                case ProfileManager.ProviderState.LoggedOut:
                    profileState = "Provider logged out";
                    break;
                case ProfileManager.ProviderState.LoggingOut:
                    profileState = "Provider logging out";
                    break;
                default:
                    profileState = "Unknown";
                    break;
            }
        }

        m_Text.text = profileState;
    }
}
