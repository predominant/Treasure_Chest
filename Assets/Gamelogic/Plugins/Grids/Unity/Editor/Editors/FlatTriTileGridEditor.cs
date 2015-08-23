//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using UnityEditor;

namespace Gamelogic.Grids.Editor.Internal
{
	[CustomEditor(typeof (FlatTriTileGridBuilder))]
	public class FlatTriTileGridEditor : SimpleGridEditor<FlatTriTileGridBuilder, FlatTriPoint>
	{
		protected override bool ShowSize(int shape)
		{
			var shapeEnum = (FlatTriTileGridBuilder.Shape) shape;

			if (shapeEnum == FlatTriTileGridBuilder.Shape.UpTriangle) return true;
			if (shapeEnum == FlatTriTileGridBuilder.Shape.DownTriangle) return true;
			if (shapeEnum == FlatTriTileGridBuilder.Shape.Hexagon) return true;
			if (shapeEnum == FlatTriTileGridBuilder.Shape.Star) return true;

			return false;
		}

		protected override bool ShowDimensions(int shape)
		{
			var shapeEnum = (FlatTriTileGridBuilder.Shape) shape;

			if (shapeEnum == FlatTriTileGridBuilder.Shape.Rectangle) return true;
			//if (shapeEnum == FlatTriTileGridBuilder.Shape.FatRectangle) return true;
			//if (shapeEnum == FlatTriTileGridBuilder.Shape.ThinRectangle) return true;
			if (shapeEnum == FlatTriTileGridBuilder.Shape.Parallelogram) return true;

			return false;
		}

		protected override bool IsCustomShape(int shape)
		{
			var shapeEnum = (FlatTriTileGridBuilder.Shape)shape;

			if (shapeEnum == FlatTriTileGridBuilder.Shape.Custom) return true;

			return false;
		}
	}
}