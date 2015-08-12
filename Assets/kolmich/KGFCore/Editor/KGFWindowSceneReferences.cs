// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2013-07-02</date>
// <summary>Find references of scripts in scene. You have to right click the component header in the inspector. Only search through direct members.</summary>

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class KGFWindowSceneReferences : EditorWindow
{
	UnityEngine.Object itsObjectNeedle;
	FoundRef []itsObjectsFound;
	class FoundRef
	{
		public FoundRef(GameObject theGO, string theComponentName, string theFieldName)
		{
			itsGO = theGO;
			itsComponentName = theComponentName;
			itsFieldName = theFieldName;
		}
		
		public GameObject itsGO;
		public string itsComponentName;
		public string itsFieldName;
	}
	
	[MenuItem ("CONTEXT/Component/FindReferencesToComponent")]
	static void DoSomething (MenuCommand theCommand)
	{
		Search(theCommand.context);
	}
	
	[MenuItem ("CONTEXT/Component/FindReferencesToAllComponents")]
	static void DoSomethingAll (MenuCommand theCommand)
	{
		List<Object> aList = new List<Object>();
		aList.Add(((Component)theCommand.context).gameObject);
		aList.AddRange(((Component)theCommand.context).GetComponents<Component>());
		Search(aList.ToArray());
	}
	
	[MenuItem ("Edit/KGF/FindReferences")]
	static void Init ()
	{
		if (Selection.activeGameObject != null)
		{
			List<Object> aList = new List<Object>();
			aList.Add(Selection.activeGameObject);
			aList.AddRange(Selection.activeGameObject.GetComponents<Component>());
			Search(aList.ToArray());
		}
		else
			Debug.LogError("empty selection");
	}
	
	/// <summary>
	/// Best known way to get all root transforms. Still does not cover trees that are all deactivated.
	/// </summary>
	static IEnumerable<Transform> GetSceneRootItems()
	{
		List<Transform> aFoundRootItems = new List<Transform>();
		
		Transform [] aListTransforms = (Transform[])Object.FindObjectsOfType(typeof(Transform));
		foreach (Transform aT in aListTransforms)
		{
			if (aT.root == null)
				continue;
			if (!aFoundRootItems.Contains(aT.root))
			{
				aFoundRootItems.Add(aT.root);
				yield return aT.root;
			}
		}
		
		yield break;
	}
	
	/// <summary>
	/// Search needles in the whole scene
	/// </summary>
	static void Search(params Object[] theNeedles)
	{
		if (theNeedles.Length == 0)
		{
			Debug.LogError("Could not search: no components on gameobject");
			return;
		}
		
		List<Component> aList = new List<Component>();
		foreach (Transform aTRoot in GetSceneRootItems())
			aList.AddRange(aTRoot.GetComponentsInChildren<Component>(true));
		List<FoundRef> aListFound = new List<FoundRef>();
		
		foreach (Object aNeedle in theNeedles)
		{
			foreach (Component aB in aList)
			{
				aListFound.AddRange(SearchRef(aB,aB,aNeedle,""));
			}
		}
		
		if (aListFound.Count == 0)
		{
			if (theNeedles[0] is Component)
				Debug.LogError("Did not find any references to:"+((Component)theNeedles[0]).gameObject.GetObjectPath());
			else if (theNeedles[0] is GameObject)
				Debug.LogError("Did not find any references to:"+((GameObject)theNeedles[0]).GetObjectPath());
			else
				Debug.LogError("Did not find any references to object of type:"+theNeedles[0].GetType());
		}
		else if (aListFound.Count == 1)
		{
			Selection.objects = new Object[]{aListFound[0].itsGO};
		}
		else
		{
			OpenSearchWindow(theNeedles[0],aListFound.ToArray());
		}
	}
	
	/// <summary>
	/// Search object in all fields of a component
	/// </summary>
	static FoundRef[] SearchRef(Component theComponent, object theHaystack, Object theNeedle, string thePath)
	{
		List<FoundRef> aListFound = new List<FoundRef>();
		if (theHaystack != null)
		{
			FieldInfo[] aFieldsList = theHaystack.GetType().GetFields();
			foreach (FieldInfo aF in aFieldsList)
			{
				var aValue = aF.GetValue(theHaystack);
				if (aValue == theNeedle)
				{
					aListFound.Add(new FoundRef(theComponent.gameObject,theComponent.GetType().Name,thePath + aF.Name));
				}
				else if (typeof(UnityEngine.Object).IsAssignableFrom(aF.FieldType))
				{
					// ignore unity types
				}
				else if (aF.FieldType.IsValueType)
				{
					// ignore value types
				}
				else if (aValue is IEnumerable)
				{
					IEnumerable aValueEnumerable = (IEnumerable)aValue;
					foreach (object anEnumeratedValue in aValueEnumerable)
					{
						if (anEnumeratedValue == theNeedle)
						{
							aListFound.Add(new FoundRef(theComponent.gameObject,theComponent.GetType().Name,thePath + aF.Name + "[]"));
						}
						if (aF.FieldType.HasElementType)
						{
							System.Type anElementType = aF.FieldType.GetElementType();
							if (!typeof(UnityEngine.Object).IsAssignableFrom(anElementType) && !anElementType.IsValueType && anElementType.IsSerializable)
							{
								aListFound.AddRange(SearchRef(theComponent,anEnumeratedValue,theNeedle,thePath + aF.Name + "[]->"));
							}
						}
					}
				}
				else if (aF.FieldType.IsSerializable)
				{
//					Debug.Log("Found serializable type:"+aF.FieldType+"/"+aF.Name);
					aListFound.AddRange(SearchRef(theComponent,aValue,theNeedle,thePath+aF.Name+"->"));
				}
			}
		}
		return aListFound.ToArray();
	}
	
	static void OpenSearchWindow(UnityEngine.Object theNeedle, FoundRef[] theFoundObjects)
	{
		KGFWindowSceneReferences aWindow = EditorWindow.GetWindow<KGFWindowSceneReferences>();
		aWindow.itsObjectNeedle = theNeedle;
		aWindow.itsObjectsFound = theFoundObjects;
	}
	
	void OnGUI ()
	{
		GUILayout.BeginVertical();
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.Label ("Find references of:", EditorStyles.boldLabel);
				EditorGUILayout.ObjectField(itsObjectNeedle,typeof(Component),true);
			}
			GUILayout.EndHorizontal();
			if (itsObjectsFound.Length == 0)
			{
				GUILayout.Label("Could not find any references. (deactivated gameobjects will not be searched)");
			}
			else
			{
				GUILayout.Label("REFERENCES:");
				foreach (FoundRef aFoundRef in itsObjectsFound)
				{
					GUILayout.BeginHorizontal();
					{
						GUILayout.Label("#",GUILayout.ExpandWidth(false));
						EditorGUILayout.ObjectField(aFoundRef.itsGO,typeof(GameObject),true);
						EditorGUILayout.LabelField("Component="+aFoundRef.itsComponentName+",Field="+aFoundRef.itsFieldName);
					}
					GUILayout.EndHorizontal();
				}
			}
		}
		GUILayout.EndVertical();
	}
}