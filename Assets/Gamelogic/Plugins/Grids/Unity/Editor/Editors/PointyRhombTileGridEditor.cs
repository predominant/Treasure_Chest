//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using UnityEditor;

namespace Gamelogic.Grids.Editor.Internal
{
	[CustomEditor(typeof (PointyRhombTileGridBuilder))]
	public class PointyRhombTileGridEditor : SimpleGridEditor<PointyRhombTileGridBuilder, PointyRhombPoint>
	{
		protected override bool ShowSize(int shape)
		{
			var shapeEnum = (PointyRhombTileGridBuilder.Shape) shape;

			if (shapeEnum == PointyRhombTileGridBuilder.Shape.Hexagon) return true;

			return false;
		}

		protected override bool ShowDimensions(int shape)
		{
			var shapeEnum = (PointyRhombTileGridBuilder.Shape) shape;

			if (shapeEnum == PointyRhombTileGridBuilder.Shape.Rectangle) return true;
			if (shapeEnum == PointyRhombTileGridBuilder.Shape.FatRectangle) return true;
			if (shapeEnum == PointyRhombTileGridBuilder.Shape.ThinRectangle) return true;
			if (shapeEnum == PointyRhombTileGridBuilder.Shape.Parallelogram) return true;

			return false;
		}

		protected override bool IsCustomShape(int shape)
		{
			var shapeEnum = (PointyRhombTileGridBuilder.Shape)shape;

			if (shapeEnum == PointyRhombTileGridBuilder.Shape.Custom) return true;

			return false;
		}
	}
}