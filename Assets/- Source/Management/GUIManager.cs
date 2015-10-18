using UnityEngine;
using System.Collections;
using System.Linq;

public class GUIManager : Singleton<GUIManager>
{
	public static GUIManager instance = null;

	void Awake()
	{
		if( null == instance )
		{
			DontDestroyOnLoad(this);
			instance = this;
		}
		else
			Destroy(gameObject);
	}
}