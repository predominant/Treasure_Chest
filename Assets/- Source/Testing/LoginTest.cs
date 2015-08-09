using UnityEngine;
using System.Collections;
using Soomla.Profile;
using Soomla;

public class LoginTest : MonoBehaviour 
{
    public GameObject m_OfflinePanel = null;
    public GameObject m_OnlinePanel = null;

    private bool m_ProfileReady = false;

    void Start()
    {
        m_OnlinePanel.SetActive(false);
        m_OfflinePanel.SetActive(true);
    }

    #region Offline
    public void BtnLoginClicked()
    {
        //// If the user clicks on the login button you provide, call the Login function:
        //SoomlaProfile.Login(
        //    Provider.FACEBOOK                        // Provider
        //);

        // If you'd like to give your users a reward for logging in, use:
        SoomlaProfile.Login(
            Provider.FACEBOOK                       // Provider
            //"",                                      // Payload
            //new BadgeReward("reward", "Logged In!")  // Reward
        );

        //// If the user would like to logout:
        //SoomlaProfile.Logout(
        //    Provider.FACEBOOK                        // Provider
        //);

        m_OnlinePanel.SetActive(true);
        m_OfflinePanel.SetActive(false);
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
    #endregion
}