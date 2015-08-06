using UnityEngine;
using System.Collections;
using Soomla.Highway;

public class GameInitializationManager : MonoBehaviour
{
	void Start()
    {
        SoomlaHighway.Initialize();
	}
}
