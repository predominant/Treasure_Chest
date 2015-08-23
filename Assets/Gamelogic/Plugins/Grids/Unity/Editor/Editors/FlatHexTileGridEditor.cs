//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using UnityEditor;

namespace Gamelogic.Grids.Editor.Internal
{
	[CustomEditor(typeof (FlatHexTileGridBuilder))]
	public class FlatHexTileGridEditor : SimpleGridEditor<FlatHexTileGridBuilder, FlatHexPoint>
	{
		protected override bool ShowSize(int shape)
		{
			var shapeEnum = (FlatHexTileGridBuilder.Shape) shape;

			if (shapeEnum == FlatHexTileGridBuilder.Shape.Diamond) return true;
			if (shapeEnum == FlatHexTileGridBuilder.Shape.LeftTriangle) return true;
			if (shapeEnum == FlatHexTileGridBuilder.Shape.RightTriangle) return true;
			if (shapeEnum == FlatHexTileGridBuilder.Shape.Hexagon) return true;

			return false;
		}

		protected override bool ShowDimensions(int shape)
		{
			var shapeEnum = (FlatHexTileGridBuilder.Shape) shape;

			if (shapeEnum == FlatHexTileGridBuilder.Shape.Rectangle) return true;
			if (shapeEnum == FlatHexTileGridBuilder.Shape.FatRectangle) return true;
			if (shapeEnum == FlatHexTileGridBuilder.Shape.ThinRectangle) return true;
			if (shapeEnum == FlatHexTileGridBuilder.Shape.Parallelogram) return true;

			return false;
		}

		protected override bool IsCustomShape(int shape)
		{
			var shapeEnum = (FlatHexTileGridBuilder.Shape)shape;

			if (shapeEnum == FlatHexTileGridBuilder.Shape.Custom) return true;

			return false;
		}
	}
}