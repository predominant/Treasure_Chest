using System;
using Soomla.Profile;

public static class ProfileManager
{
    #region Members
    private static Provider m_CurrentProvider;
    private static bool m_LoggedIn = false;
    #endregion

    #region Properties
    public static bool LoggedIn
    {
        get { return m_LoggedIn; }
        set { if (value) { Login(); } else { Logout(); } }
    }
    #endregion

    #region Events
    public static event Action OnLoggedIn;
    public static event Action OnLoggedOut;
    #endregion

    static ProfileManager()
    {
    }

    public static void Login()
    {
        if (LoggedIn)
            return;

        m_LoggedIn = true;

        if( m_LoggedIn && null != OnLoggedIn )
            OnLoggedIn();
    }

    public static void Logout()
    {
        if (!LoggedIn)
            return;

        m_LoggedIn = false;

        if( null != OnLoggedOut )
            OnLoggedOut();
    }
}