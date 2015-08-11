using UnityEngine;
using System.Collections;
using Soomla.Profile;

public class ProfileButtonInteractions : MonoBehaviour
{
    #region Facebook
    public void LoginFacebook()
    {
        if (!ProfileManager.IsLoggedIn)
            ProfileManager.Login(Provider.FACEBOOK);
    }

    public void LogoutFacebook()
    {
        if (ProfileManager.IsLoggedIn)
            ProfileManager.Logout(Provider.FACEBOOK);
    }
    #endregion

    #region Google+
    public void LoginGoogle()
    {
        if (!ProfileManager.IsLoggedIn)
            ProfileManager.Login(Provider.GOOGLE);
    }

    public void LogoutGoogle()
    {
        if (ProfileManager.IsLoggedIn)
            ProfileManager.Logout(Provider.FACEBOOK);
    }
    #endregion

    #region Twitter
    public void LoginTwitter()
    {
        if (!ProfileManager.IsLoggedIn)
            ProfileManager.Login(Provider.TWITTER);
    }

    public void LogoutTwitter()
    {
        if (ProfileManager.IsLoggedIn)
            ProfileManager.Logout(Provider.FACEBOOK);
    }
    #endregion

    #region Universal
    public void Logout()
    {
        if (ProfileManager.IsLoggedIn)
            ProfileManager.Logout(ProfileManager.CurrentProvider);
    }
    #endregion
}
