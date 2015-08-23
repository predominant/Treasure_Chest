//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System.Collections.Generic;
using System.Linq;

namespace Gamelogic.Grids.Examples
{
	public static partial class MazeAlgorithms
	{
		public static IEnumerable<PointyHexPoint> GetEdgeFaces(PointyHexPoint point)
		{
			var color = point.GetColor2_4();

			var faces = new PointList<PointyHexPoint>();

			switch (color)
			{
				case 0:
					//error!
					break;
				case 1:
					faces.Add(point + PointyHexPoint.East);
					faces.Add(point + PointyHexPoint.West);
					break;
				case 2:
					faces.Add(point + PointyHexPoint.SouthWest);
					faces.Add(point + PointyHexPoint.NorthEast);
					break;
				case 3:
					faces.Add(point + PointyHexPoint.SouthEast);
					faces.Add(point + PointyHexPoint.NorthWest);
					break;
			}

			return faces;
		}

		/*
	public static IEnumerable<PointyHexPoint> GenerateMazeWalls(PointyHexGrid<Cell> grid)
	{
		var walls = grid.CloneStructure<bool>(); //true indicates passable
		
		foreach(var point in walls)
		{
			walls[point] = point.GetColor2_4() == 0;
		}
		
		var wallList = new List<PointyHexPoint>();
		
		var newMaizePoint = grid.Where(p => p.GetColor2_4() == 0).RandomItem();
		var inMaze = new List<PointyHexPoint> {newMaizePoint};

		var edges = newMaizePoint.GetNeighbors();
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
				edges = newMaizePoint.GetNeighbors().Where(edge => !(walls[edge]));
				wallList.AddRange(edges);
			}
			else
			{
				wallList.Remove(randomWall);
			}
		}
	}
	*/

		public static IEnumerable<PointyHexPoint> GenerateMazeWalls<TCell>(PointyHexGrid<TCell> grid)
		{
			var walls = grid.CloneStructure<bool>(); //true indicates passable

			foreach (var point in walls)
			{
				walls[point] = point.GetColor2_4() == 0;
			}

			var wallList = new PointList<PointyHexPoint>();

			var newMaizePoint = grid.Where(p => p.GetColor2_4() == 0).RandomItem();
			var inMaze = new PointList<PointyHexPoint> {newMaizePoint};

			var edges = newMaizePoint.GetNeighbors();
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
					edges = newMaizePoint.GetNeighbors().Where(edge => !(walls[edge]));
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