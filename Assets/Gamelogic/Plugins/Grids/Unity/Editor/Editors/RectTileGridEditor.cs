//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

namespace Gamelogic.Grids.Editor.Internal
{
	using Grids;
	using UnityEditor;

	[CustomEditor(typeof (RectTileGridBuilder))]
	public class RectTileGridEditor : SimpleGridEditor<RectTileGridBuilder, RectPoint>
	{
		protected override bool ShowSize(int shape)
		{
			var shapeEnum = (RectTileGridBuilder.Shape) shape;

			if (shapeEnum == RectTileGridBuilder.Shape.Circle) return true;

			return false;
		}

		protected override bool ShowDimensions(int shape)
		{
			var shapeEnum = (RectTileGridBuilder.Shape) shape;

			switch (shapeEnum)
			{
				case RectTileGridBuilder.Shape.Rectangle:
				case RectTileGridBuilder.Shape.Parallelogram:
				case RectTileGridBuilder.Shape.CheckerBoard:
					return true;
			}

			return false;
		}

		protected override bool IsCustomShape(int shape)
		{
			var shapeEnum = (RectTileGridBuilder.Shape)shape;

			if (shapeEnum == RectTileGridBuilder.Shape.Custom) return true;

			return false;
		}
	}
}