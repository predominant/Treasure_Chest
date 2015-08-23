//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using UnityEditor;

namespace Gamelogic.Grids.Editor.Internal
{
	[CustomEditor(typeof (PolarPointyBrickTileGridBuilder))]
	public class PolarPointyBrickTileGridEditor : SimpleGridEditor<PolarPointyBrickTileGridBuilder, PointyHexPoint>
	{
		protected override bool ShowSize(int shape)
		{
			return false;
		}

		protected override bool ShowDimensions(int shape)
		{
			var shapeEnum = (PolarPointyBrickTileGridBuilder.Shape) shape;

			if (shapeEnum == PolarPointyBrickTileGridBuilder.Shape.Parallelogram) return true;

			return false;
		}

		protected override bool IsCustomShape(int shape)
		{
			return false;
		}
	}
}