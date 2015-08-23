//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using UnityEditor;

namespace Gamelogic.Grids.Editor.Internal
{
	[CustomEditor(typeof (PolarFlatBrickTileGridBuilder))]
	public class PolarFlatBrickTileGridEditor : SimpleGridEditor<PolarFlatBrickTileGridBuilder, FlatHexPoint>
	{
		protected override bool ShowSize(int shape)
		{
			return false;
		}

		protected override bool ShowDimensions(int shape)
		{
			var shapeEnum = (PolarFlatBrickTileGridBuilder.Shape) shape;

			if (shapeEnum == PolarFlatBrickTileGridBuilder.Shape.Parallelogram) return true;
			if (shapeEnum == PolarFlatBrickTileGridBuilder.Shape.Rectangle) return true;

			return false;
		}

		protected override bool IsCustomShape(int shape)
		{
			return false;
		}
	}
}