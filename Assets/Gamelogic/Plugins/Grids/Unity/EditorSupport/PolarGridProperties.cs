using System;

namespace Gamelogic.Grids
{
	/**
		A class for storing properties to setup a polar grid.
		@version1_8
		@ingroup UnityEditorSupport
	*/
	[Serializable]
	public class PolarGridProperties
	{
		public float innerRadius = 50;
		public float outerRadius = 350;
		public float border = 0f;
		public float quadSize = 15f;
	}
}