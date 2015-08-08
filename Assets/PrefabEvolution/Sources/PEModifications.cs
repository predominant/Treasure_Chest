using UnityEngine;
using System.Collections.Generic;

namespace PrefabEvolution
{
	[System.Serializable]
	public class PEModifications
	{
		[System.Serializable]
		public class PropertyData
		{
			public enum PropertyMode
			{
				Default,
				Keep,
				Ignore,
			}

			public Object Object;
			public int ObjeckLink;
			public string PropertyPath;
			public PropertyMode Mode;

			public object UserData;
		}

		[System.Serializable]
		public class HierarchyData
		{
			public Transform child;
			public Transform parent;
		}

		[System.Serializable]
		public class ComponentsData
		{
			public Component child;
			public GameObject parent;
		}

		public List<PropertyData> Modificated = new List<PropertyData>();
		public List<HierarchyData> NonPrefabObjects = new List<HierarchyData>();
		public List<ComponentsData> NonPrefabComponents = new List<ComponentsData>();
		public List<int> RemovedObjects = new List<int>();
		public List<HierarchyData> TransformParentChanges = new List<HierarchyData>();
	}

}