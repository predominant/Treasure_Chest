using UnityEngine;
using System.Collections.Generic;

namespace PrefabEvolution
{
	public enum PrefabHideMode
	{
		Open,
		Closed,
	}

	[SelectionBase]
	[AddComponentMenu("")]
	public class PEPrefabScript : MonoBehaviour, ISerializationCallbackReceiver
	{
		#region ISerializationCallbackReceiver implementation

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			this.Properties.PrefabScript = this;
			this.Properties.InheritedProperties = null;
		}

		#endregion

		[HideInInspector]
		public PEExposedProperties Properties = new PEExposedProperties();

		[HideInInspector]
		public PELinkage Links = new PELinkage();
		[HideInInspector]
		public PEModifications Modifications;

		public string ParentPrefabGUID;

		public string PrefabGUID;

		public GameObject ParentPrefab
		{
			get
			{
				return EditorBridge.GetAssetByGuid(ParentPrefabGUID);
			}
			set
			{
				var guid = EditorBridge.GetAssetGuid(value);
				if (!string.IsNullOrEmpty(guid))
					ParentPrefabGUID = guid;
			}
		}
		public GameObject Prefab
		{
			get
			{
				return  EditorBridge.GetAssetByGuid(PrefabGUID);
			}
			set
			{
				var guid = EditorBridge.GetAssetGuid(value);
				if (!string.IsNullOrEmpty(guid))
					PrefabGUID = guid;
			}
		}

		void OnValidate()
		{
			if (EditorBridge.OnValidate != null)
				EditorBridge.OnValidate(this);
		}

		public void SetHideInternalObjects(bool value)
		{
			foreach(var obj in this.GetComponentsInChildren<Component>(true))
			{
				if (obj is PEPrefabScript || obj == this.transform)
					continue;
				if (obj is Transform)
				{
					SetObjectHide(obj.gameObject, value);
					continue;
				}

				SetObjectHide(obj, value);
			}
		}

		static public void SetObjectHide(Object obj, bool value)
		{
			obj.HideFlagsSet(HideFlags.HideInHierarchy | HideFlags.HideInInspector, value);
		}

		public event System.Action OnBuildModifications;

		public void InvokeOnBuildModifications()
		{
			if (OnBuildModifications != null)
				OnBuildModifications();
		}

		static public class EditorBridge
		{
			public static System.Action<PEPrefabScript> OnValidate;
			public static System.Func<GameObject, string> GetAssetGuid;
			public static System.Func<string, GameObject> GetAssetByGuid;
		}
	}
}