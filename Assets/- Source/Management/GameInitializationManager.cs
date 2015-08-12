using UnityEngine;
using Soomla.Highway;
using Soomla.Profile;
using Soomla;

public class GameInitializationManager : MonoBehaviour
{
    private static bool m_Initialized = false;

	void Start()
    {
        if (m_Initialized)
            return;

        Screen.orientation = ScreenOrientation.Landscape;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;

        //Screen.autorotateToPortraitUpsideDown = false;

        ProfileManager.Initialize();
        m_Initialized = true;
	}
}
