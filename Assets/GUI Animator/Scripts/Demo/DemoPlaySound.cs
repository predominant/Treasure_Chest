using UnityEngine;
using System.Collections;

public class DemoPlaySound : MonoBehaviour {
	
	public int m_AudioSourceCount = 2;
	
	AudioSource[] m_AudioSource = null;
	
	public AudioClip m_Audio_Button1 = null;
	public AudioClip m_Audio_Button2 = null;
	
	// Use this for initialization
	void Start () {

		// Create AudioSource list
		if(m_AudioSource==null)
		{
			m_AudioSource = new AudioSource[m_AudioSourceCount];
			
			for(int i=0;i<m_AudioSource.Length;i++)
			{
				AudioSource pAudioSource =  this.gameObject.AddComponent<AudioSource>();
				pAudioSource.rolloffMode = AudioRolloffMode.Linear;
				m_AudioSource[i] = pAudioSource;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	// Play AudioClip
	void PlayOneShot(AudioClip pAudioClip)
	{

		for(int i=0;i<m_AudioSource.Length;i++)
		{
			if(m_AudioSource[i].isPlaying == false)
			{
				m_AudioSource[i].PlayOneShot(pAudioClip);
				break;
			}
		}
	}

	// Play m_Audio_Button1 audio clip
	public void PlaySoundButton1()
	{
		PlayOneShot(m_Audio_Button1);
	}
	
	// Play m_Audio_Button2 audio clip
	public void PlaySoundButton2()
	{
		PlayOneShot(m_Audio_Button2);
	}
}

