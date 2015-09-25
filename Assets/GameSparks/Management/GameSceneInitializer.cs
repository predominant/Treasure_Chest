using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSceneInitializer : MonoBehaviour
{
	public List<GameObject> m_Prefabs = new List<GameObject>();
	protected static bool m_Initialized = false;

	void Awake()
	{
		if( !m_Initialized )
		{
			foreach( GameObject go in m_Prefabs )
				GameObject.Instantiate( go );

			m_Initialized = true;
		}
	}
}