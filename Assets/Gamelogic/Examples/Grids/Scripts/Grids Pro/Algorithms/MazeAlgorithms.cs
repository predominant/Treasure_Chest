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
		/**
			Generates a maze using a randomized version of Prim's algorithm.

			@returns a IEnumerable of passages 
		*/
		//For some reason, the generic version gives a TypeLoadException in Unity (but not when run from visual studio).
		/*
	public static IEnumerable<TEdge> GenerateMazeWalls<TGrid, TPoint, TEdge>(TGrid grid)
		where TEdge : IGridPoint<TEdge>, IEdge<TPoint>
		where TPoint : IGridPoint<TPoint>, ISupportsEdges<TEdge>
		where TGrid : ISupportsEdgeGrid<TEdge>, IGridSpace<TPoint>
	{	
		IGrid<bool, TEdge> walls = grid.MakeEdgeGrid<bool>(); //true indicates passable
		var wallList = new List<TEdge>();
		
		TPoint newMaizePoint = grid.RandomItem<TPoint>();
		var inMaze = new List<TPoint>();
		inMaze.Add(newMaizePoint);

		var edges = newMaizePoint.GetEdges();
		wallList.AddRange(edges);
		
		while (wallList.Any())
		{
			var randomWall = wallList.RandomItem();
			IEnumerable<TPoint> faces = (randomWall as IEdge<TPoint>).GetEdgeFaces().Where(x => grid.Contains(x));

			//At least one of the two faces must be in the maze
			if (faces.Any(point => !inMaze.Contains(point)))
			{
				newMaizePoint = faces.First(point => !inMaze.Contains(point));
				inMaze.Add(newMaizePoint);
				walls[randomWall] = true;
				
				yield return randomWall;

				// Add all edges that are not passages
				edges = newMaizePoint.GetEdges().Where(edge => !(walls[edge]));
				wallList.AddRange(edges);
			}
			else
			{
				wallList.Remove(randomWall);
			}
		}
		yield return (TEdge)(object) g.First();
		yield break;
	}
	*/

		/**
			Generates a maze using a randomized version of Prim's algorithm.

			@returns a IEnumerable of passages 
		*/
		public static IEnumerable<PointyRhombPoint> GenerateMazeWalls<TCell>(FlatTriGrid<TCell> grid)
		{
			var walls = grid.MakeEdgeGrid<bool>(); //true indicates passable
			var wallList = new PointList<PointyRhombPoint>();

			var newMaizePoint = grid.RandomItem();
			var inMaze = new PointList<FlatTriPoint> {newMaizePoint};

			var edges = newMaizePoint.GetEdges();
			wallList.AddRange(edges);

			while (wallList.Any())
			{
				var randomWall = wallList.RandomItem();
				var faces = (randomWall as IEdge<FlatTriPoint>).GetEdgeFaces().Where(x => grid.Contains(x));

				//At least one of the two faces must be in the maze
				if (faces.Any(point => !inMaze.Contains(point)))
				{
					newMaizePoint = faces.First(point => !inMaze.Contains(point));
					inMaze.Add(newMaizePoint);
					walls[randomWall] = true;

					yield return randomWall;

					// Add all edges that are not passages
					edges = newMaizePoint.GetEdges().Where(edge => !(walls[edge]));
					wallList.AddRange(edges);
				}
				else
				{
					wallList.Remove(randomWall);
				}
			}
		}

		/**
			Generates a maze using a randomized version of Prim's algorithm.

			@returns a IEnumerable of passages 
		*/
		public static IGrid<bool, PointyRhombPoint> GenerateMaze<TCell>(FlatTriGrid<TCell> grid)
		{
			var walls = grid.MakeEdgeGrid<bool>(); //true indicates passable
			var wallList = new PointList<PointyRhombPoint>();

			var newMaizePoint = grid.RandomItem();
			var inMaze = new PointList<FlatTriPoint> {newMaizePoint};
			var edges = newMaizePoint.GetEdges();
			wallList.AddRange(edges);

			while (wallList.Any())
			{
				var randomWall = wallList.RandomItem();
				var faces = (randomWall as IEdge<FlatTriPoint>).GetEdgeFaces().Where(grid.Contains);

				//At least one of the two faces must be in the maze
				if (faces.Any(point => !inMaze.Contains(point)))
				{
					newMaizePoint = faces.First(point => !inMaze.Contains(point));
					inMaze.Add(newMaizePoint);
					walls[randomWall] = true;

					// Add all edges that are not passages
					edges = newMaizePoint.GetEdges().Where(edge => !(walls[edge]));
					wallList.AddRange(edges);
				}
				else
				{
					wallList.Remove(randomWall);
				}
			}

			return walls;
		}
	}
}