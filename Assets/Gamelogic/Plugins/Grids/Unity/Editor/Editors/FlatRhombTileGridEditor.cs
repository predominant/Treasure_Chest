//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using UnityEditor;

namespace Gamelogic.Grids.Editor.Internal
{
	[CustomEditor(typeof (FlatRhombTileGridBuilder))]
	public class FlatRhombTileGridEditor : SimpleGridEditor<FlatRhombTileGridBuilder, FlatRhombPoint>
	{
		override protected bool ShowSize(int shape)
		{
			var shapeEnum = (FlatRhombTileGridBuilder.Shape)shape;

			if (shapeEnum == FlatRhombTileGridBuilder.Shape.Hexagon) return true;

			return false;
		}

		override protected bool ShowDimensions(int shape)
		{
			var shapeEnum = (FlatRhombTileGridBuilder.Shape)shape;

			if (shapeEnum == FlatRhombTileGridBuilder.Shape.Rectangle) return true;
			if (shapeEnum == FlatRhombTileGridBuilder.Shape.FatRectangle) return true;
			if (shapeEnum == FlatRhombTileGridBuilder.Shape.ThinRectangle) return true;
			if (shapeEnum == FlatRhombTileGridBuilder.Shape.Parallelogram) return true;

			return false;
		}

		protected override bool IsCustomShape(int shape)
		{
			var shapeEnum = (FlatRhombTileGridBuilder.Shape)shape;

			if (shapeEnum == FlatRhombTileGridBuilder.Shape.Custom) return true;

			return false;
		}
	}
}