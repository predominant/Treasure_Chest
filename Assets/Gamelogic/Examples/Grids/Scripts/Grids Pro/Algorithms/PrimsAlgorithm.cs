//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	[AddComponentMenu("Gamelogic/Examples/PrimsAlgorithm")]
	public class PrimsAlgorithm : GridBehaviour<FlatTriPoint>
	{
		public MazeCell edgePrefab;

		private readonly Vector2 cellDimensions = new Vector2(140, 120);

		public override void InitGrid()
		{
			var grid = (FlatTriGrid<TileCell>) Grid;

			var edgeGrid = grid.MakeEdgeGrid<MazeCell>();

			var edgeMap = new PointyRhombMap(cellDimensions)
				.WithWindow(ExampleUtils.ScreenRect)
				.AlignMiddleCenter(edgeGrid)
				.To3DXY();

			foreach (var point in edgeGrid)
			{
				MazeCell cell = GridBuilderUtils.Instantiate(edgePrefab);
				cell.transform.parent = transform;
				cell.transform.localScale = Vector3.one;
				cell.transform.localPosition = edgeMap[point];
				cell.SetOrientation(point.I, false);
				cell.name = "M";
				edgeGrid[point] = cell;
			}

			foreach (var point in MazeAlgorithms.GenerateMazeWalls(grid))
			{
				if (edgeGrid[point] != null)
				{
					edgeGrid[point].SetOrientation(point.I, true);
				}
			}
		}
	}
}