using UnityEngine;
using System.Collections;

public class SceneButtonInteractions : MonoBehaviour
{
    public void LoadLevel(string _scene)
    {
        Application.LoadLevel(_scene);
    }

    public void LoadLevelAdditive(string _scene)
    {
        Application.LoadLevelAdditive(_scene);
    }

    public void LoadLevelAsync(string _scene)
    {
        Application.LoadLevelAsync(_scene);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
