//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using UnityEditor;

namespace Gamelogic.Grids.Editor.Internal
{
	[CustomEditor(typeof (PointyHexTileGridBuilder))]
	public class PointyHexTileGridEditor : SimpleGridEditor<PointyHexTileGridBuilder, PointyHexPoint>
	{
		override protected bool ShowSize(int shape)
		{
			var shapeEnum = (PointyHexTileGridBuilder.Shape) shape;

			if (shapeEnum == PointyHexTileGridBuilder.Shape.Diamond) return true;
			if (shapeEnum == PointyHexTileGridBuilder.Shape.DownTriangle) return true;
			if (shapeEnum == PointyHexTileGridBuilder.Shape.UpTriangle) return true;
			if (shapeEnum == PointyHexTileGridBuilder.Shape.Hexagon) return true;

			return false;
		}

		override protected bool ShowDimensions(int shape)
		{
			var shapeEnum = (PointyHexTileGridBuilder.Shape)shape;

			if (shapeEnum == PointyHexTileGridBuilder.Shape.Rectangle) return true;
			if (shapeEnum == PointyHexTileGridBuilder.Shape.FatRectangle) return true;
			if (shapeEnum == PointyHexTileGridBuilder.Shape.ThinRectangle) return true;
			if (shapeEnum == PointyHexTileGridBuilder.Shape.Parallelogram) return true;

			return false;
		}

		protected override bool IsCustomShape(int shape)
		{
			var shapeEnum = (PointyHexTileGridBuilder.Shape)shape;

			if (shapeEnum == PointyHexTileGridBuilder.Shape.Custom) return true;

			return false;
		}
	}
}