//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System.Linq;
using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	[AddComponentMenu("Gamelogic/Examples/VertexMatching")]
	public class VertexMatching : GridBehaviour<PointyHexPoint>
	{
		private const int Water = 0;
		private const int Land = 1;

		private readonly int[] frameIndices =
		{
			0, 1, -1, 2, -1, 3, -1, 4,
			-1, 5, -1, 6, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, 7, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, 8, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, 9, -1, 10, -1,
			11, -1, 12, -1, 13, -1, 14, 15
		};



		/*
			This method is called by the tile grid once 
			the cells have been created.
			We use it here to initialise the cells.
		*/
		public override void InitGrid()
		{
			//This is a grid that corresponds to the vertices of our displayed grid.
			//Since the displayed grid is a pointy hex grid, the vertex
			//grid is a flat tri grid.
			var grid = (PointyHexGrid<TileCell>) Grid;
			var vertexGrid = (FlatTriGrid<int>) grid.MakeVertexGrid<int>();

			//We randomly assign whether vertices are landor water
			foreach (var point in vertexGrid)
			{
				vertexGrid[point] = Random.value > 0.5f ? Land : Water;
			}

			//Then we set the cells to the correct tiles based on the vertex values.
			foreach (var point in grid)
			{
				SetCellSprite(vertexGrid, point, grid[point]);
			}
		}

		private void SetCellSprite(FlatTriGrid<int> vertexGrid, PointyHexPoint point, TileCell tileCell)
		{
			var cell = (SpriteCell) tileCell;

			var vertices =
				from vertexPoint in point.GetVertices()
				select vertexGrid[vertexPoint];

			int imageIndex = vertices.Reverse().Aggregate((x, y) => (x << 1) + y);
			float zRotation = 30;

			for (int i = 0; i < 6; i++)
			{
				if (frameIndices[imageIndex] != -1)
				{
					cell.FrameIndex = frameIndices[imageIndex];
					cell.transform.SetRotationZ(zRotation);

					break;
				}

				zRotation += 60;
				imageIndex = RotateEdgeNumberClockWise(imageIndex);
			}
		}

		public int RotateEdgeNumberClockWise(int edge)
		{
			return ((edge & 1) << 5) + (edge >> 1);
		}

		public int RotateEdgeNumberCounterClockWise(int edge)
		{
			return ((edge << 1) & 63) + (edge >> 5);
		}
	}
}