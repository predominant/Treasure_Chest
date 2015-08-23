

using UnityEditor;

namespace Gamelogic.Grids.Editor.Internal
{
	[CustomEditor(typeof (PointListTileGridBuilder))]
	public class PointListTileGridEditor : SimpleGridEditor<PointListTileGridBuilder, LinePoint>
	{
		protected override bool ShowSize(int shape)
		{
			return false;
		}

		protected override bool ShowDimensions(int shape)
		{
			return false;
		}
	}
}