//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System.Linq;
using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	public class Pipes : GridBehaviour<PointyHexPoint>, IResetable
	{
		/*
		In this example each tile image has a binary presentation with 
		six digits that corresponds to the six edges of the hex and
		indicate whether edges have pipes through them or not.
		
		This arrays maps the image number to the sprite frame index.
	*/

		private readonly int[] FrameIndices =
		{
			0, 1, -1, 2, -1, 3, -1, 4,
			-1, 5, -1, 6, -1, -1, -1, 7,
			-1, -1, -1, 8, -1, 9, -1, 10,
			-1, -1, -1, 11, -1, -1, -1, 12,
			-1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, 13
		};

		private PointyHexGrid<PipesCell> grid;
		private IMap3D<PointyHexPoint> map;

		public void OnGUI()
		{
			if (GUILayout.Button("Reset"))
			{
				Reset();
			}
		}

		public override void InitGrid()
		{
			grid = (PointyHexGrid<PipesCell>) Grid.CastValues<PipesCell, PointyHexPoint>();

			Reset();
		}

		public void Reset()
		{
			var edgeGrid = grid.MakeEdgeGrid<int>();

			foreach (PointyRhombPoint point in edgeGrid)
			{
				edgeGrid[point] = Random.Range(0, 2);
			}

			foreach (var point in grid)
			{
				var cell = grid[point];

				cell.SetAngle(-30);
				SetCellSprite(edgeGrid, point, cell);
			}

			RandomRotateCells();
			UpdateHighlights();
		}

		private void RandomRotateCells()
		{
			foreach (PointyHexPoint point in grid)
			{
				int rotationCount = Random.Range(0, 6);

				for (int i = 0; i < rotationCount; i++)
				{
					grid[point].RotateCW();
				}
			}
		}

		private void UpdateHighlights()
		{
			foreach (var point in grid)
			{
				UpdateHighlight(point);
			}
		}

		private void SetCellSprite(IGrid<int, PointyRhombPoint> edgeGrid, PointyHexPoint point, PipesCell cell)
		{
			var edges =
				from edgePoint in point.GetEdges()
				select edgeGrid[edgePoint];

			int imageIndex = edges.Reverse().Aggregate((x, y) => (x << 1) + y);

			// Because images are flat hex, not pointy, and we want them pointy
			float zRotation = -30;

			for (int i = 0; i < 6; i++)
			{
				if (FrameIndices[imageIndex] != -1) //we found it
				{
					//so we can use it
					cell.FrameIndex = FrameIndices[imageIndex];
					cell.SetAngle(zRotation);

					break;
				}

				//While we cannot find the sprite, transform and check again
				zRotation += 60;
				imageIndex = RotateEdgeNumberClockWise(imageIndex);
			}

			cell.EdgeData = edges.ToList();
		}

		public int RotateEdgeNumberClockWise(int edge)
		{
			return ((edge & 1) << 5) + (edge >> 1);
		}

		public int RotateEdgeNumberCounterClockWise(int edge)
		{
			return ((edge << 1) & 63) + (edge >> 5);
		}

		public void OnRightClick(PointyHexPoint point)
		{
			grid[point].RotateCW();
			UpdateHighlight(point);

			foreach (var neighbor in grid.GetNeighbors(point))
			{
				UpdateHighlight(neighbor);
			}
		}

		public void OnLeftClick(PointyHexPoint point)
		{
			grid[point].RotateCCW();
			UpdateHighlight(point);

			foreach (var neighbor in grid.GetNeighbors(point))
			{
				UpdateHighlight(neighbor);
			}
		}

		public void Update()
		{
			if (HasGameFinished())
			{
				Debug.Log("Game Ended!");
			}
		}

		public bool HasGameFinished()
		{
			return grid.All(IsClosed);
		}

		public void UpdateHighlight(PointyHexPoint point)
		{
			grid[point].HighlightOn = IsClosed(point);
		}

		public bool IsClosed(PointyHexPoint point)
		{
			var neighbors = grid.GetAllNeighbors(point).ToList();
			bool isClosed = true;

			//We use for loop so that we can use the index in the access operation below
			for (int i = 0; i < neighbors.Count(); i++)
			{
				if (grid.Contains(neighbors[i]))
				{
					//(i + 3) % 6 is the opposite neighbor
					//Here we abuse the fact that neighbors are returned in (anti-clockwise) order.
					if (grid[point].EdgeData[i] != grid[neighbors[i]].EdgeData[(i + 3)%6])
					{
						isClosed = false;
					}
				}
			}

			return isClosed;
		}
	}
}