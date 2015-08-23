#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using Object = UnityEngine.Object;

namespace Gamelogic.Grids
{
	/**
		Provides utility functions for grid builders.
		@ingroup UnityEditorSupport

		@version1_8
		Renamed from GridBuilderUtil to GridBuilderUtils (1.8.1) 
	*/
	public static class GridBuilderUtils
	{
		/**
			A palette of default colors.
		*/

		public static Color[] DefaultColors
		{
			get { return defaultColors.Clone() as Color[]; }
		}

		private static readonly Color[] defaultColors = new Color[] 
		{	
			ColorFromInt(133, 219, 233),
			ColorFromInt(198, 224, 34),
			ColorFromInt(255, 215, 87),
			ColorFromInt(228, 120, 129),	
		
			ColorFromInt(42, 192, 217),
			ColorFromInt(114, 197, 29),
			ColorFromInt(247, 188, 0),
			ColorFromInt(215, 55, 82),
		
			ColorFromInt(205, 240, 246),
			ColorFromInt(229, 242, 154),
			ColorFromInt(255, 241, 153),
			ColorFromInt(240, 182, 187),
		
			ColorFromInt(235, 249, 252),
			ColorFromInt(241, 249, 204),
			ColorFromInt(255, 252, 193),
			ColorFromInt(247, 222, 217),

			Color.black
		};

		public static Color Red = DefaultColors[7];
		public static Color Yellow = DefaultColors[6];
		public static Color Green = DefaultColors[5];
		public static Color Blue = DefaultColors[4];

		private static Color ColorFromInt(int r, int g, int b)
		{
			return new Color(r / 255.0f, g / 255.0f, b / 255.0f);
		}

		/**
			This function only works if a main camera has been set.
		*/
		public static Vector3 ScreenToWorld(Vector3 screenPosition)
		{
			if (Camera.main == null)
			{
				Debug.LogError("No main camera found in scene");

				return Vector3.zero;
			}
#if GL_NGUI
			return UICamera.currentCamera.ScreenToWorldPoint(screenPosition);
#else
			return Camera.main.ScreenToWorldPoint(screenPosition);
#endif
		}

		/**
			This function only works if a main camera has been set.
		*/
		public static Vector3 ScreenToWorld(GameObject root, Vector3 screenPosition)
		{
			if (Camera.main == null)
			{
				Debug.LogError("No main camera found in scene");

				return Vector3.zero;
			}
#if GL_NGUI
			return root.transform.InverseTransformPoint(UICamera.currentCamera.ScreenToWorldPoint(screenPosition));
#else
			return root.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(screenPosition));
#endif
		}

		public static T Instantiate<T>(T prefab)
			where T : MonoBehaviour
		{
			T instance = null;
			if (Application.isPlaying)
			{
				instance = (T) Object.Instantiate(prefab);
			}
#if UNITY_EDITOR
			else
			{
				instance = (T) PrefabUtility.InstantiatePrefab(prefab);
			}
#endif
			return instance;
		}
	}
}
