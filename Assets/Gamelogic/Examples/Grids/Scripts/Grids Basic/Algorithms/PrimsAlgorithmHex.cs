//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	[AddComponentMenu("Gamelogic/Examples/PrimsAlgorithmHex")]
	public class PrimsAlgorithmHex : GridBehaviour<PointyHexPoint>
	{
		public override void InitGrid()
		{
			if (((PointyHexTileGridBuilder) GridBuilder).GridShape != PointyHexTileGridBuilder.Shape.Hexagon)
			{
				Debug.LogError("Shape must be Hexagon");

				return;
			}

			if (GridBuilder.Size%2 != 0)
			{
				Debug.LogError("The size must be even!");

				return;
			}

			foreach (var point in Grid)
			{
				int color = point.GetColor2_4();
				((SpriteCell) Grid[point]).FrameIndex = color;
			}

			foreach (var point in MazeAlgorithms.GenerateMazeWalls<TileCell>((PointyHexGrid<TileCell>) Grid))
			{
				((SpriteCell) Grid[point]).FrameIndex = 0;
			}
		}
	}
}