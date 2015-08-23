//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System.Collections.Generic;
using System.Linq;

namespace Gamelogic.Grids.Examples
{
	public static class RectPointExtensions2
	{
		public static int GetColor3(this RectPoint point)
		{
			int color = point.GetColor1_2();

			if (GLMathf.Mod(point.X, 2) == 0)
			{
				color += 2;
			}

			return color;
		}
	}

	public static partial class MazeAlgorithms
	{
		public static IEnumerable<RectPoint> GetEdgeFaces(RectPoint point)
		{
			var color = point.GetColor3();

			var faces = new PointList<RectPoint>();

			switch (color)
			{
				case 0:
					//error!
					break;
				case 1:
					faces.Add(point + RectPoint.North);
					faces.Add(point + RectPoint.South);
					break;
				case 3:
					faces.Add(point + RectPoint.East);
					faces.Add(point + RectPoint.West);
					break;
					/*case 2:
				faces.Add(point + RectPoint.NorthEast);
				faces.Add(point + RectPoint.NorthWest);
				faces.Add(point + RectPoint.SouthEast);
				faces.Add(point + RectPoint.SouthWest);
				break;*/
			}

			return faces;
		}

		public static IEnumerable<RectPoint> GenerateMazeWalls<TCell>(RectGrid<TCell> grid)
		{
			var walls = grid.CloneStructure<bool>(); //true indicates passable

			foreach (var point in walls)
			{
				walls[point] = point.GetColor3() == 0;

				//Debug.Log(point);
			}

			var wallList = new PointList<RectPoint>();

			var newMaizePoint = grid.Where(p => p.GetColor3() == 0).RandomItem();
			var inMaze = new PointList<RectPoint> {newMaizePoint};

			var edges = grid.GetNeighbors(newMaizePoint);
			wallList.AddRange(edges);

			while (wallList.Any())
			{
				var randomWall = wallList.RandomItem();
				var faces = GetEdgeFaces(randomWall).Where(grid.Contains);

				//At least one of the two faces must be in the maze
				if (faces.Any(point => !inMaze.Contains(point)))
				{
					newMaizePoint = faces.First(point => !inMaze.Contains(point));
					inMaze.Add(newMaizePoint);
					walls[randomWall] = true;

					yield return randomWall;

					// Add all edges that are not passages
					edges = grid.GetNeighbors(newMaizePoint).Where(edge => !(walls[edge]));
					wallList.AddRange(edges);
				}
				else
				{
					wallList.Remove(randomWall);
				}
			}
		}
	}
}