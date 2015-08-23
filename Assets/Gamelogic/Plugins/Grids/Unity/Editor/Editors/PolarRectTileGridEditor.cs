//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEditor;

namespace Gamelogic.Grids.Editor.Internal
{
	[CustomEditor(typeof (PolarRectTileGridBuilder))]
	public class PolarRectTileGridEditor : SimpleGridEditor<PolarRectTileGridBuilder, RectPoint>
	{
		protected override bool ShowSize(int shape)
		{
			return false;
		}

		protected override bool ShowDimensions(int shape)
		{
			var shapeEnum = (PolarRectTileGridBuilder.Shape) shape;

			if (shapeEnum == PolarRectTileGridBuilder.Shape.Rectangle) return true;

			return false;
		}

		protected override bool IsCustomShape(int shape)
		{
			return false;
		}
	}
}
