using UnityEngine;

namespace Gamelogic.Grids.Examples
{

	[ExecuteInEditMode]
	public class AnimatedGrid : GridBehaviour<RectPoint>
	{
		public void Update()
		{
			//updates are called through the custom editor
			//if we do not add this condition here, our
			// scene gets set to dirty all the time, which 
			//is not convenient.
#if !UNITY_EDITOR 
		Animate();
#endif
		}

		public void Animate()
		{
			foreach (var point in Grid)
			{
				Grid[point].transform.localPosition = Map[point];
			}
		}
	}
}