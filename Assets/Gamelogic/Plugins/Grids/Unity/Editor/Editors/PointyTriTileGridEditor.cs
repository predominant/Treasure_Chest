//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using UnityEditor;

namespace Gamelogic.Grids.Editor.Internal
{
	[CustomEditor(typeof (PointyTriTileGridBuilder))]
	public class PointyTriTileGridEditor : SimpleGridEditor<PointyTriTileGridBuilder, PointyTriPoint>
	{
		protected override bool ShowSize(int shape)
		{
			var shapeEnum = (PointyTriTileGridBuilder.Shape) shape;

			if (shapeEnum == PointyTriTileGridBuilder.Shape.LeftTriangle) return true;
			if (shapeEnum == PointyTriTileGridBuilder.Shape.RightTriangle) return true;
			if (shapeEnum == PointyTriTileGridBuilder.Shape.Star) return true;
			if (shapeEnum == PointyTriTileGridBuilder.Shape.Hexagon) return true;


			return false;
		}

		protected override bool ShowDimensions(int shape)
		{
			var shapeEnum = (PointyTriTileGridBuilder.Shape) shape;

			if (shapeEnum == PointyTriTileGridBuilder.Shape.Rectangle) return true;
			if (shapeEnum == PointyTriTileGridBuilder.Shape.FatRectangle) return true;
			if (shapeEnum == PointyTriTileGridBuilder.Shape.ThinRectangle) return true;
			if (shapeEnum == PointyTriTileGridBuilder.Shape.Parallelogram) return true;

			return false;
		}

		protected override bool IsCustomShape(int shape)
		{
			var shapeEnum = (PointyTriTileGridBuilder.Shape) shape;

			if (shapeEnum == PointyTriTileGridBuilder.Shape.Custom) return true;

			return false;
		}
	}
}