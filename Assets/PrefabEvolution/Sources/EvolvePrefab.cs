using UnityEngine;

namespace PrefabEvolution
{
	[AddComponentMenu("")]
	public class EvolvePrefab : PEPrefabScript
	{
		void Awake()
		{
			if (Application.isEditor)
				return;

			this.Links = null;
			this.Modifications = null;
		}

		[ContextMenu("HideInternals")]
		void HideInternals()
		{
			this.SetHideInternalObjects(true);
		}

		[ContextMenu("ShowInternals")]
		void ShowInternals()
		{
			this.SetHideInternalObjects(false);
		}
	}
}
