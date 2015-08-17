using UnityEngine;
using System.Collections;
using Soomla.Profile;
using Soomla;
using GameSparks.Core;
using GameSparks.Api.Requests;
using System.Collections.Generic;


public class LoginMenuController : MonoBehaviour
{
    public GameObject m_OfflinePanel = null;
    public GameObject m_WaitingPanel = null;
    public GameObject m_OnlinePanel = null;

    [HideInInspector]
    public static bool GSReconnecting = false;

    private bool m_ProfileReady = false;
    private bool m_UIHidden = false;

    #region Unity Events
    void Start()
    {
        m_OnlinePanel.SetActive(false);
        m_WaitingPanel.SetActive(false);
        m_OfflinePanel.SetActive(true);

        ProfileManager.OnStateChanged += ProfileManager_OnStateChanged;
        GS.GameSparksAvailable += GS_OnStateChanged;

        if( ProfileManager.IsLoggedIn )
        {
            m_OnlinePanel.SetActive(true);
            m_WaitingPanel.SetActive(false);
            m_OfflinePanel.SetActive(false);
        }
    }

    void OnDestroy()
    {
        ProfileManager.OnStateChanged -= ProfileManager_OnStateChanged;        
    }
    #endregion

    #region GS Event Handlers
    protected void GS_OnStateChanged(bool isAvailable)
    {
        GSReconnecting = false;

        if( !isAvailable )
           GS_Reconnect_Cor();
    }

    protected IEnumerable GS_Reconnect_Cor()
    {
        yield return new WaitForSeconds(1);
        GSReconnecting = true;
        GS.Reconnect();
    }
    
    #endregion

    #region Profile Event Handlers
    void ProfileManager_OnStateChanged(ProfileManager.ProviderState _state)
    {
        switch (_state)
        {
            case ProfileManager.ProviderState.LoggedIn:
                m_OnlinePanel.SetActive(true);
                m_WaitingPanel.SetActive(false);
                m_OfflinePanel.SetActive(false);

                if( ProfileManager.CurrentProvider == Provider.FACEBOOK )
                {
                    new FacebookConnectRequest().SetAccessToken(FB.AccessToken).Send((response) =>
                    {
                        if (response.HasErrors)
                            Debug.LogError("[GS] Facebook auth error: " + response.Errors.ToString());
                    });
                }

                else if( ProfileManager.CurrentProvider == Provider.GOOGLE )
                {
                }

                else if( ProfileManager.CurrentProvider == Provider.TWITTER )
                {
                }

                break;

            case ProfileManager.ProviderState.LoggingIn:
                m_OnlinePanel.SetActive(false);
                m_WaitingPanel.SetActive(true);
                m_OfflinePanel.SetActive(false);
                break;

            case ProfileManager.ProviderState.LoggedOut:
                m_OnlinePanel.SetActive(false);
                m_WaitingPanel.SetActive(false);
                m_OfflinePanel.SetActive(true);
                break;

            case ProfileManager.ProviderState.LoggingOut:
                m_OnlinePanel.SetActive(false);
                m_WaitingPanel.SetActive(true);
                m_OfflinePanel.SetActive(false);
                break;
        }
    }
    #endregion

    #region Offline
    public void BtnFBLoginClicked()
    {
        ProfileManager.Login(Provider.FACEBOOK);
    }

    public void BtnGPLoginClicked()
    {
        ProfileManager.Login(Provider.GOOGLE);
    }

    public void BtnTWLoginClicked()
    {
        ProfileManager.Login(Provider.TWITTER);
    }
    #endregion

    #region Online
    public void BtnStatusClicked()
    {
        if (!FB.IsLoggedIn) return;

        SoomlaProfile.UpdateStatus(Provider.FACEBOOK, "This is a Treasure Chest test status!");
    }

    public void BtnPostClicked()
    {
        if (!FB.IsLoggedIn) return;

        SoomlaProfile.UpdateStory(Provider.FACEBOOK, "This is a Treasure Chest test message!", "Huzzah!", "Caption", "Description", "", "http://www.alansjourney.com/wp-content/uploads/2015/02/Oh-Hell-Yeah.png");
    }

    public void BtnFriendsClicked()
    {
        if (!FB.IsLoggedIn) return;

        SoomlaProfile.GetContacts(Provider.FACEBOOK, true);
    }

    public void BtnLogoutClicked()
    {
        if (!FB.IsLoggedIn) return;

        SoomlaProfile.Logout(Provider.FACEBOOK);
        m_OnlinePanel.SetActive(false);
        m_OfflinePanel.SetActive(true);
    }

    public void BtnToggleUI()
    {
        if (!FB.IsLoggedIn) return;

        m_UIHidden = !m_UIHidden;

        Transform onlinePanel = transform.FindChild("Online Panel");
        onlinePanel.FindChild("Status").gameObject.SetActive(!m_UIHidden);
        onlinePanel.FindChild("Post").gameObject.SetActive(!m_UIHidden);
        onlinePanel.FindChild("Logout").gameObject.SetActive(!m_UIHidden);

        string btnText = m_UIHidden ? "Show" : "Hide";
        onlinePanel.FindChild("Status").gameObject.SetActive(!m_UIHidden);
        onlinePanel.FindChild("Hide UI").FindChild("Text").GetComponent<UnityEngine.UI.Text>().text = btnText;
    }
    #endregion
}