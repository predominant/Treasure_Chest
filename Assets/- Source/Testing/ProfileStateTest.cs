using UnityEngine;
using System.Collections;
using Soomla.Profile;

public class ProfileStateTest : MonoBehaviour
{

    public UnityEngine.UI.Text m_Text = null;

    void Start()
    {
        //try
        //{
        //    PackageInfo info = PackageManger.getPackageManager().getPackageInfo(
        //            "com.example.packagename", 
        //            PackageManager.GET_SIGNATURES);
            
        //    for (Signature signature : info.signatures)
        //    {
        //        MessageDigest md = MessageDigest.getInstance("SHA");
        //        md.update(signature.toByteArray());
        //        Log.d("KeyHash:", Base64.encodeToString(md.digest(), Base64.DEFAULT));
        //    }
        //}
        //catch (NameNotFoundException e)
        //{
        //}
        //catch (NoSuchAlgorithmException e)
        //{
        //}
    }

    void Update()
    {
        if (!FB.IsInitialized)
            m_Text.text = "[Facebook] Not Initialized";
        else if (SoomlaProfile.IsLoggedIn(Provider.FACEBOOK))
            m_Text.text = "[Facebook] Logged In";
        else
            m_Text.text = "[Facebook] Logged Out";
    }
}
