using UnityEngine;
using System.Collections.Generic;
using Soomla.Highway;
using Soomla.Profile;
using Soomla;
using GameSparks.Core;
using GameSparks.Api;
using GameSparks.Api.Messages;
using System.Linq;

public class GameInitializationManager : MonoBehaviour
{
	public static GameInitializationManager instance = null;
    private static bool m_Initialized = false;
    private Queue<string> myLogQueue = new Queue<string>();
    private string myLog = "";

	void Awake()
	{
		if (null == instance)
		{
			DontDestroyOnLoad(this);
			instance = this;
		}
		else
		{
			DestroyImmediate(gameObject);
			return;
		}

		DontDestroyOnLoad(this);

		Screen.orientation = ScreenOrientation.Landscape;
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		Screen.autorotateToPortrait = false;
		//Screen.autorotateToPortraitUpsideDown = false;
	}

	void Start()
    {
        if (m_Initialized)
            return;

		GSMessageHandler._AllMessages = HandleGameSparksMessageReceived;
		ProfileManager.Initialize();
        m_Initialized = true;
    }

    #region GameSparks Handlers
    void HandleGameSparksMessageReceived(GSMessage message)
    {
        HandleLog("MSG:" + message.JSONString);
    }

    void HandleLog(string logString)
    {
        GS.GSPlatform.ExecuteOnMainThread(() =>
        {
            HandleLog(logString, null, LogType.Log);
        });
    }

    void HandleLog(string logString, string stackTrace, LogType logType)
    {
        if (myLogQueue.Count > 30)
        {
            myLogQueue.Dequeue();
        }
        myLogQueue.Enqueue(logString);
        myLog = "";

        foreach (string logEntry in myLogQueue.ToArray())
        {
            myLog = logEntry + "\n\n" + myLog;
        }
    }
    #endregion
}
