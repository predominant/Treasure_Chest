using UnityEngine;
using System.Collections;
using Soomla.Profile;
using Soomla;

public class LoginTest : MonoBehaviour 
{
    public void LoginClicked()
    {
        //// If the user clicks on the login button you provide, call the Login function:
        //SoomlaProfile.Login(
        //    Provider.FACEBOOK                        // Provider
        //);

        // If you'd like to give your users a reward for logging in, use:
        SoomlaProfile.Login(
            Provider.FACEBOOK,                       // Provider
            "",                                      // Payload
            new BadgeReward("reward", "Logged In!")  // Reward
        );

        //// If the user would like to logout:
        //SoomlaProfile.Logout(
        //    Provider.FACEBOOK                        // Provider
        //);

        gameObject.SetActive(false);
    }
}