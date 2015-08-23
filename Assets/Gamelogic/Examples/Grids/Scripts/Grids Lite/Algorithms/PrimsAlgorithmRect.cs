//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd         //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	[AddComponentMenu("Gamelogic/Examples/PrimsAlgorithmHex")]
	public class PrimsAlgorithmRect : GridBehaviour<RectPoint>
	{
		public Color wallColor = ExampleUtils.Colors[6];
		public Color floorColor = ExampleUtils.Colors[4];

		public override void InitGrid()
		{
			if (((RectTileGridBuilder) GridBuilder).GridShape != RectTileGridBuilder.Shape.Rectangle)
			{
				Debug.LogError("Shape must be Rectangle");

				return;
			}

			if ((GridBuilder.Dimensions.X%2 != 1) && (GridBuilder.Dimensions.Y%2 != 1))
			{
				Debug.LogError("Both dimensions must be even!");

				return;
			}

			foreach (var point in Grid)
			{
				var color = point.GetColor3() == 0 ? floorColor : wallColor;
				Grid[point].Color = color;
			}

			foreach (var point in MazeAlgorithms.GenerateMazeWalls((RectGrid<TileCell>) Grid))
			{
				Grid[point].Color = floorColor;
			}
		}
	}
}