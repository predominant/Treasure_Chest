using Gamelogic.Grids.Editor.Internal;
using UnityEditor;

namespace Gamelogic.Grids.Examples.Editor
{
	[CustomEditor(typeof (AnimatedGrid))]
	public class AnimatedGridEditor : GLEditor<AnimatedGrid>
	{
		public void OnEnable()
		{
			EditorApplication.update += UpdateAnimation;
		}

		public void OnDisable()
		{
			EditorApplication.update -= UpdateAnimation;
		}

		private void UpdateAnimation()
		{
			if (Target.enabled /*&& !EditorApplication.isPlaying*/)
			{
				Target.Animate();
			}
		}
	}
}