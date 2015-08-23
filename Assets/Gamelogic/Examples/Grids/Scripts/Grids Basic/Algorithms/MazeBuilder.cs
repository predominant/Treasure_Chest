//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	/**
		This example gives an example of building a new 
		algorithm on top of the grid components. 
	*/
	[AddComponentMenu("Gamelogic/Examples/MazeBuilder")]
	public class MazeBuilder : GridBehaviour<PointyHexPoint>
	{
		private readonly Color offColor = ExampleUtils.Colors[4];
		private readonly Color onColor = ExampleUtils.Colors[6];

		//private TileGridBuilder<PointyHexPoint> TileGridBuilder;
		private PointyHexGrid<bool> logicalGrid;
		private List<PointyHexPoint> smallGrid;
		private PointList<PointyHexPoint> dirtyCells;
		private PointyHexPoint startNode;
		private PointyHexPoint endNode;
		private int iterationCount;
		private int size;

		public void Awake()
		{
			dirtyCells = new PointList<PointyHexPoint>();
		}

		public void Start()
		{
			iterationCount = 0;
			size = GridBuilder.Size;
			BuildGrid();
			InitPath();
		}

		private void BuildGrid()
		{
			//if(TileGridBuilder.Shape != Hexagon)

			startNode = new PointyHexPoint(1 - size, size - 1);
			endNode = new PointyHexPoint(size - 1, 1 - size);

			smallGrid = PointyHexGrid<int>
				.Hexagon(size - 1)
				.ToList();

			logicalGrid = (PointyHexGrid<bool>) Grid.CloneStructure<bool>();

			foreach (var point in logicalGrid)
			{
				logicalGrid[point] = false;
			}
		}

		private void InitPath()
		{
			for (int i = 1 - size; i <= size - 1; i++)
			{
				ToggleCellAt(new PointyHexPoint(i, -i));
			}
		}

		public void Update()
		{
			if (iterationCount < 400) //stop after a while
			{
				iterationCount++;

				for (int i = 0; i < 400; i++) //do a few iterations per update
				{
					ToggleRandomCell();
				}
			}

			foreach (var point in dirtyCells) //only update visual cells each update
			{
				UpdateCell(point);
			}

			dirtyCells.Clear();
		}

		public void ToggleRandomCell()
		{
			int count = smallGrid.Count();
			int index = Random.Range(0, count);

			PointyHexPoint randomPoint = smallGrid[index];

			if (randomPoint == endNode || randomPoint == startNode)
			{
				return;
			}

			if ((!logicalGrid[randomPoint] || !(Random.value < 0.5f)) &&
			    (logicalGrid[randomPoint] || !(Random.value < 20f/iterationCount + 0.02f)))
			{
				return;
			}

			List<PointyHexPoint> neighborHood = logicalGrid.GetAllNeighbors(randomPoint).ToList();
			neighborHood.Add(randomPoint);

			var closedCells =
				from point in neighborHood
				where !logicalGrid[point]
				select point;

			var pointyHexPoints = closedCells as PointyHexPoint[] ?? closedCells.ToArray();

			if (pointyHexPoints.Count() >= 7) return;

			bool closedCellsAreConnected;

			if (pointyHexPoints.Any())
			{
				closedCellsAreConnected = Algorithms.IsConnected(
					logicalGrid,
					pointyHexPoints,
					(x, y) => (logicalGrid[x] == logicalGrid[y]));
			}
			else
			{
				closedCellsAreConnected = true;
			}

			if (closedCellsAreConnected)
			{
				ToggleCellAt(randomPoint);
			}
		}

		private void UpdateCell(PointyHexPoint point)
		{
			Grid[point].Color = logicalGrid[point] ? onColor : offColor;
		}

		private void ToggleCellAt(PointyHexPoint point)
		{
			dirtyCells.Add(point);
			logicalGrid[point] = !logicalGrid[point];
		}
	}
}