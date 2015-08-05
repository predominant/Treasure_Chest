using UnityEngine;
using System.Collections;

public class PrintFPS : MonoBehaviour 
{
	public bool  m_IsFrameRateUnlocked = false;
    public float m_Frequency = 0.5f;
    public int 	 m_FramesPerSec { get; protected set; }
    string 		 m_Fps;

    void Awake() 
	{
        Application.targetFrameRate = -1;
    }
	
	private IEnumerator FPS()
    {
        for (; ; )
        {
            // Capture frame-per-second
            int 	lastFrameCount = Time.frameCount;
            float 	lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(m_Frequency);
            float 	timeSpan = Time.realtimeSinceStartup - lastTime;
            int 	frameCount = Time.frameCount - lastFrameCount;

            // Display it
            m_FramesPerSec = Mathf.RoundToInt(frameCount / timeSpan);
            m_Fps = m_FramesPerSec.ToString() + " fps";
        }
    }

	void Start()
	{
		StartCoroutine(FPS());
	}

    void OnGUI()
    {
		GUI.Label(new Rect(10, 50, 150, 25), m_Fps);
    }
}
