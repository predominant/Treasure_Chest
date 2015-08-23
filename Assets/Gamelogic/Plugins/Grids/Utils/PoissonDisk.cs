using UnityEngine;
using System.Collections.Generic;

namespace Gamelogic.Grids
{
	/**
		Used to generate Poisson disk sample points.

		@version1_8
	*/
	public class PoissonDisk
	{
		//RandomQueue works like a queue, except that it
		//pops a random element from the queue instead of
		//the element at the head of the queue
		private class RandomQueue<T>
		{
			private readonly List<T> list;

			public RandomQueue()
			{
				list = new List<T>();
			}

			public void Push(T item)
			{
				list.Add(item);
			}

			public T Pop()
			{
				int n = Random.Range(0, list.Count);
				T item = list[n];
				list.RemoveAt(n);

				return item;
			}

			public bool IsEmpty()
			{
				return list.Count <= 0;
			}
		}

		public static List<Vector2> GeneratePoisson(Rect rect, float minDist, int newPointsCount)
		{
			//Create the grid
			float cellSize = minDist/Mathf.Sqrt(2);

			var gridWidth = Mathf.CeilToInt(rect.width/cellSize);
			var gridHeight = Mathf.CeilToInt(rect.height/cellSize);
			var grid = RectGrid<Vector2?>.Rectangle(gridWidth, gridHeight); 

			var map = new RectMap(Vector2.one)
				.AnchorCellBottomLeft()
				.WithWindow(rect)
				.Stretch(grid);

			var processList = new RandomQueue<Vector2>();
			var samplePoints = new List<Vector2>();

			//generate the first point randomly
			//and updates 
			var firstPoint = new Vector2(Random.value * rect.width, Random.value * rect.height) + new Vector2(rect.xMin, rect.yMin);

			//update containers
			processList.Push(firstPoint);
			samplePoints.Add(firstPoint);
			grid[map[firstPoint]] = firstPoint;

			//generate other points from points in queue.
			while (!processList.IsEmpty())
			{
				var point = processList.Pop();

				for (int i = 0; i < newPointsCount; i++)
				{
					var newPoint = GenerateRandomPointAround(point, minDist);
					//check that the point is in the image region
					//and no points exists in the point's neighbourhood

					if (rect.Contains(newPoint) && !IsInNeighbourhood(grid, map, newPoint, minDist))
					{
						if (grid.Contains(map[newPoint])) //TODO: why is this necessary?
						{
							//update containers
							processList.Push(newPoint);
							samplePoints.Add(newPoint);

							grid[map[newPoint]] = newPoint;
						}
						/*
					else
					{
						Debug.Log(newPoint);
						Debug.Log(map[newPoint]);
						Debug.Break();
					}
					*/
					}
				}
			}
			return samplePoints;
		}

		//non-uniform, favours points closer to the inner ring, leads to denser packings
		private static Vector2 GenerateRandomPointAround(Vector2 point, float mindist)
		{
			float r1 = Random.value; //random point between 0 and 1
			float r2 = Random.value;
			//random radius between mindist and 2 * mindist
			float radius = mindist*(r1 + 1);
			//random angle
			float angle = 2*Mathf.PI*r2;
			//the new point is generated around the point (x, y)
			float newX = point.x + radius*Mathf.Cos(angle);
			float newY = point.y + radius*Mathf.Sin(angle);

			return new Vector2(newX, newY);
		}

		private static bool IsInNeighbourhood(RectGrid<Vector2?> grid, IMap<RectPoint> map, Vector2 point, float mindist)
		{
			var gridPoint = map[point];

			//get the neighbourhood if the point in the grid
			//cellsAroundPoint = squareAroundPoint(grid, gridPoint, 5);
			foreach (var neighbor in grid.GetNeighborHood(gridPoint, 2))
			{
				var cell = grid[neighbor];

				if (cell != null)
				{
					var difference = cell.Value - point;

					if (difference.magnitude < mindist)
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}