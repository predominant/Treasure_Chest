using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(RawImage))]
public class EMTransition : MonoBehaviour
{
	[SerializeField] Texture2D m_gradationTexture;
	[SerializeField] float m_duration = 1.0f;
	[SerializeField] bool m_playOnAwake = true;
	[SerializeField] bool m_flipAfterAnimation = false;
	[SerializeField] bool m_flip = false;
	[SerializeField] bool m_invert = false;
	[SerializeField] bool m_ignoreTimeScale = false;
	[SerializeField] bool m_pingPong = false;
	[SerializeField] AnimationCurve m_curve;
	[SerializeField, Range(0, 1.0f)] float m_threshold = 0;
	public UnityEvent onTransitionStart;
	public UnityEvent onTransitionComplete;

	public Texture2D gradationTexture
	{ 
		get { return m_gradationTexture; }
		set {
			m_gradationTexture = value;
			GetComponent<RawImage>().material.SetTexture("_Gradation", m_gradationTexture);
		}
	}
	
	public float duration
	{ 
		get { return m_duration; }
		set { m_duration = Mathf.Max(m_duration, 0); }
	}
	
	public bool playOnAwake
	{ 
		get { return m_playOnAwake; }
		set { m_playOnAwake = value; }
	}
	
	public bool flipAfterAnimation
	{ 
		get { return m_flipAfterAnimation; }
		set { m_flipAfterAnimation = value; }
	}
	
	public bool flip
	{ 
		get { return m_flip; }
		set { m_flip = value; }
	}
	
	public bool invert
	{ 
		get { return m_invert; }
		set { m_invert = value; }
	}
	
	public bool ignoreTimeScale
	{ 
		get { return m_ignoreTimeScale; }
		set { m_ignoreTimeScale = value; }
	}
	

	public AnimationCurve curve
	{ 
		get { return m_curve; }
		set { m_curve = value; }
	}
	
	public float threshold
	{ 
		get { return m_threshold; }
		set { m_threshold = value; }
	}
	
	void Start()
	{
		if(m_flip) m_curve = FlipCurve();
		if(m_playOnAwake) Play();
	}

	void OnValidate()
	{
		var material = GetComponent<RawImage>().material;
		material.SetInt("_Invert", m_invert ? 1 : 0);
		material.SetTexture("_Gradation", m_gradationTexture);
		material.SetFloat("_Cutoff", m_threshold);
		m_duration = Mathf.Max(m_duration, 0);
	}

	public void Play()
	{
		m_threshold = (m_curve.Evaluate(0) > 0.5f) ? 1f : 0;
		if(!m_pingPong) onTransitionStart.Invoke();
		StartCoroutine(PlayCoroutine());
	}
	
	IEnumerator PlayCoroutine()
	{
		var material = GetComponent<RawImage>().material;
		float t = m_ignoreTimeScale ? Time.realtimeSinceStartup : Time.time;

		if(!m_ignoreTimeScale)
		{
			while(Time.time - t < m_duration)
			{
				m_threshold = m_curve.Evaluate((Time.time - t) / m_duration);
				material.SetFloat("_Cutoff", m_threshold);
				yield return null;
			}

		} else {

			while(Time.realtimeSinceStartup - t < m_duration)
			{
				m_threshold = m_curve.Evaluate((Time.realtimeSinceStartup - t) / m_duration);
				material.SetFloat("_Cutoff", m_threshold);
				yield return null;
			}
		}
		
		if(m_pingPong)
		{
			m_flip = !m_flip;
			m_curve = FlipCurve();
			Play();
			
		} else {

			if(m_flipAfterAnimation)
			{
				m_flip = !m_flip;
				m_curve = FlipCurve();
			}

			onTransitionComplete.Invoke();
			
		}
	}

	public void SetColor(Color col)
	{
		GetComponent<RawImage>().color = col;
	}
	
	public void SetTexture(Texture2D tex)
	{
		GetComponent<RawImage>().texture = tex;
	}
	
	public void SetGradationTexture(Texture2D tex)
	{
		gradationTexture = tex;
	}
	
	public void FlipAnimationCurve ()
	{
		m_curve = FlipCurve();
	}
	
	AnimationCurve FlipCurve ()
	{
		AnimationCurve newCurve = new AnimationCurve();
		
		for (int i = 0; i < m_curve.length; i++)
		{
			Keyframe key = m_curve[i];
			key.time = 1f - key.time;
			key.inTangent = key.inTangent * -1f;
			key.outTangent = key.outTangent * -1f;
			newCurve.AddKey(key);
		}
		
		return newCurve;
	}

}
