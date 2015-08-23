//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEditor;

namespace Gamelogic.Grids.Editor.Internal
{
	[CustomEditor(typeof (DiamondTileGridBuilder))]
	public class DiamondTileGridEditor : SimpleGridEditor<DiamondTileGridBuilder, DiamondPoint>
	{
		protected override bool ShowSize(int shape)
		{
			var shapeEnum = (DiamondTileGridBuilder.Shape) shape;

			if (shapeEnum == DiamondTileGridBuilder.Shape.Diamond) return true;

			return false;
		}

		protected override bool ShowDimensions(int shape)
		{
			var shapeEnum = (DiamondTileGridBuilder.Shape) shape;

			if (shapeEnum == DiamondTileGridBuilder.Shape.Rectangle) return true;
			if (shapeEnum == DiamondTileGridBuilder.Shape.FatRectangle) return true;
			if (shapeEnum == DiamondTileGridBuilder.Shape.ThinRectangle) return true;
			if (shapeEnum == DiamondTileGridBuilder.Shape.Parallelogram) return true;

			return false;
		}

		protected override bool IsCustomShape(int shape)
		{
			var shapeEnum = (DiamondTileGridBuilder.Shape)shape;

			if (shapeEnum == DiamondTileGridBuilder.Shape.Custom) return true;

			return false;
		}
	}
}
