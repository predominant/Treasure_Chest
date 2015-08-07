using UnityEngine;
using Soomla.Highway;
using Soomla.Profile;
using Soomla;

public class GameInitializationManager : MonoBehaviour
{
	void Start()
    {
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;

        SoomlaHighway.Initialize();
        SoomlaProfile.Initialize();
	}
}
