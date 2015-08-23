

using UnityEditor;

namespace Gamelogic.Grids.Editor.Internal
{
	[CustomEditor(typeof (LineTileGridBuilder))]
	public class LineTileGridEditor : SimpleGridEditor<LineTileGridBuilder, LinePoint>
	{
		protected override bool ShowSize(int shape)
		{
			return true;
		}

		protected override bool ShowDimensions(int shape)
		{
			return false;
		}

		protected override bool IsCustomShape(int shape)
		{
			var shapeEnum = (LineTileGridBuilder.Shape)shape;

			if (shapeEnum == LineTileGridBuilder.Shape.Custom) return true;

			return false;
		}
	}
}