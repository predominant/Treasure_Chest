using System;
using UnityEngine;
using Soomla.Highway;
using Soomla.Profile;

public static class ProfileManager
{
    #region Members
    private static Provider m_CurrentProvider;
    private static ProviderState m_State = ProviderState.LoggedOut;
    #endregion

    #region Properties
    public static ProviderState State
    {
        set { m_State = value; if (null != OnStateChanged) { OnStateChanged(value); } }
        get { return m_State; }
    }

    public static bool IsLoggedIn { get { return State == ProviderState.LoggedIn; } }

    public static Provider CurrentProvider { get { return m_CurrentProvider; } }
    #endregion

    #region Enums
    public enum ProviderState {
        LoggedIn,
        LoggingIn,
        LoggedOut,
        LoggingOut,
    }
    #endregion

    #region Events
    public static event Action<string> OnLoginStarted;
    public static event Action<UserProfile, string> OnLoginFinished;
    public static event Action<string> OnLoginCancelled;
    public static event Action<string, string> OnLoginFailed;
    public static event Action OnLogoutStarted;
    public static event Action OnLogoutFinished;
    public static event Action<string> OnLogoutFailed;
    public static event Action<ProviderState> OnStateChanged;
    #endregion

    static ProfileManager()
    {
        ProfileEvents.OnLoginStarted += Profile_LoginStarted;
        ProfileEvents.OnLoginFinished += Profile_LoginFinished;
        ProfileEvents.OnLoginCancelled += Profile_LoginCancelled;
        ProfileEvents.OnLoginFailed += Profile_LoginFailed;
        ProfileEvents.OnLogoutStarted += Profile_LogoutStarted;
        ProfileEvents.OnLogoutFinished += Profile_LogoutFinished;
        ProfileEvents.OnLogoutFailed += Profile_LogoutFailed;
    }

#region Event Handlers
    private static void Profile_LoginStarted(Provider _provider, string _payload)
    {
        m_CurrentProvider = _provider;
        State = ProviderState.LoggingIn;
        if (null != OnLoginStarted) OnLoginStarted(_payload);
    }

    private static void Profile_LoginFinished(UserProfile _profile, string _payload)
    {
        m_CurrentProvider = _profile.Provider;
        State = ProviderState.LoggedIn;
        if (null != OnLoginFinished) OnLoginFinished(_profile, _payload);
    }

    private static void Profile_LoginCancelled(Provider _provider, string _payload)
    {
        State = ProviderState.LoggedOut;
        if (null != OnLoginCancelled) OnLoginCancelled(_payload);
    }

    private static void Profile_LoginFailed(Provider _provider, string _errorMessage, string _payload)
    {
        State = ProviderState.LoggedOut;
        if (null != OnLoginFailed) OnLoginFailed(_errorMessage, _payload);
    }

    private static void Profile_LogoutStarted(Provider _provider)
    {
        State = ProviderState.LoggingOut;
        if (null != OnLogoutStarted) OnLogoutStarted();
    }

    private static void Profile_LogoutFinished(Provider _provider)
    {
        State = ProviderState.LoggedOut;
        if (null != OnLogoutFinished) OnLogoutStarted();
    }

    private static void Profile_LogoutFailed(Provider _provider, string _errorMessage)
    {
        State = ProviderState.LoggedOut;
        if (null != OnLogoutFailed) OnLogoutFailed(_errorMessage);
    }
#endregion

    public static void Initialize()
    {
#if !UNITY_EDITOR
        SoomlaHighway.Initialize();
#endif
        SoomlaProfile.Initialize();
    }

    public static void Login( Provider _provider )
    {
        SoomlaProfile.Login(_provider);
    }

	public static void Logout(Provider _provider) 
	{
		SoomlaProfile.Logout(_provider);
	}

	public static void Logout()
	{
		SoomlaProfile.Logout(m_CurrentProvider);
	}
}