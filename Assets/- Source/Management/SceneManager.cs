using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SceneManager : MonoBehaviour 
{
#region Sub Objects
	public class SceneTransition
	{
		public string FromScene = string.Empty;
		public string ToScene = string.Empty;
		public string LocatorName = string.Empty;

		public SceneTransition(string fromScene, string toScene, string locatorName)
		{
			FromScene = fromScene;
			ToScene = toScene;
			LocatorName = locatorName;
		}
	}
#endregion

#region Member Variables
	public AsyncOperation m_SceneLoadProgress = null;
	public List<SceneTransition> m_SceneTransitions = new List<SceneTransition>();
	public static SceneManager instance = null;
	protected EMTransition m_TransitionOutEffect = null;
	protected EMTransition m_TransitionInEffect = null;
#endregion

#region Properties
	public SceneTransition NewestTransition { get { return m_SceneTransitions.LastOrDefault(); } }
#endregion

#region Unity Events
	void Awake()
	{
		if( null == instance )
		{
			instance = this;
			InitializeSceneReferences();
			InitialTransitionIn();
		}
		else
			DestroyImmediate( gameObject );

		//DontDestroyOnLoad( GameObject.Find("- GUI") );
	}

	void OnLevelWasLoaded(int level)
	{
		InitializeSceneReferences();

		if( m_SceneTransitions.Count == 0 )
		{
			return;
		}

		m_SceneLoadProgress = null;
		SceneTransition transition = m_SceneTransitions.Last();
		PlayTransitionInEffect();
	}
#endregion

#region Mutators
	protected void InitializeSceneReferences()
	{
		GameObject[] managers = GameObject.FindGameObjectsWithTag("GameManager");
		GameObject transition = managers.FirstOrDefault((i) => { return i.name == "Transitions"; });
		Transform inTransition = transition.transform.FindChild("In");
		Transform outTransition = transition.transform.FindChild("Out");
		m_TransitionInEffect = inTransition.GetComponent<EMTransition>();
		m_TransitionOutEffect = outTransition.GetComponent<EMTransition>();
		m_TransitionInEffect.onTransitionComplete.AddListener(() => { OnTransitionInEffectComplete(); });
		m_TransitionOutEffect.onTransitionComplete.AddListener(() => { OnTransitionOutEffectComplete(); });
	}

	protected void InitialTransitionIn()
	{
		PlayTransitionInEffect();
	}

	public void PlayTransitionInEffect()
	{
		m_TransitionInEffect.Play();
	}

	protected void OnTransitionInEffectComplete()
	{
	}

	public void PlayTransitionOutEffect()
	{
		m_TransitionOutEffect.Play();
	}

	protected void OnTransitionOutEffectComplete()
	{
		m_SceneLoadProgress = Application.LoadLevelAsync(m_SceneTransitions.Last().ToScene);
	}

	public void ChangeScene( SceneTransition transition )
	{
		m_SceneTransitions.Add( transition );
		PlayTransitionOutEffect();
	}
#endregion
}