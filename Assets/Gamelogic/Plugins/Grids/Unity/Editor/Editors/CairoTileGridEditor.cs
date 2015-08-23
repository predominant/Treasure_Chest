//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using UnityEditor;

namespace Gamelogic.Grids.Editor.Internal
{
	[CustomEditor(typeof (CairoTileGridBuilder))]
	public class CairoTileGridEditor : SimpleGridEditor<CairoTileGridBuilder, CairoPoint>
	{
		override protected bool ShowSize(int shape)
		{
			var shapeEnum = (CairoTileGridBuilder.Shape) shape;

			if (shapeEnum == CairoTileGridBuilder.Shape.Diamond) return true;
			if (shapeEnum == CairoTileGridBuilder.Shape.DownTriangle) return true;
			if (shapeEnum == CairoTileGridBuilder.Shape.UpTriangle) return true;
			if (shapeEnum == CairoTileGridBuilder.Shape.Hexagon) return true;

			return false;
		}

		override protected bool ShowDimensions(int shape)
		{
			var shapeEnum = (CairoTileGridBuilder.Shape)shape;

			if (shapeEnum == CairoTileGridBuilder.Shape.Rectangle) return true;
			if (shapeEnum == CairoTileGridBuilder.Shape.FatRectangle) return true;
			if (shapeEnum == CairoTileGridBuilder.Shape.ThinRectangle) return true;
			if (shapeEnum == CairoTileGridBuilder.Shape.Parallelogram) return true;

			return false;
		}

		protected override bool IsCustomShape(int shape)
		{
			var shapeEnum = (CairoTileGridBuilder.Shape)shape;

			if (shapeEnum == CairoTileGridBuilder.Shape.Custom) return true;

			return false;
		}
	}
}