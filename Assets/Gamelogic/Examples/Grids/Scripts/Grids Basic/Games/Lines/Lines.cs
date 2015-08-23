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
	public class Lines : GridBehaviour<PointyHexPoint>
	{
		#region Inspector

		public int lineLength = 4;
		public int colorCount = 3;
		public int cellsPerTurn = 5;

		#endregion

		#region Private Fields

		private PointyHexGrid<LinesCell> grid;
		private bool isHoldingCell;
		private LinesCell cellThatIsBeingHeld;

		#endregion

		#region Callbacks

		public void OnGUI()
		{
			if (GUILayout.Button("Reset"))
			{
				Reset();
			}
		}

		public override void InitGrid()
		{
			grid = (PointyHexGrid<LinesCell>) Grid.CastValues<LinesCell, PointyHexPoint>();
			Reset();
		}

		public void Reset()
		{
			foreach (var cell in grid.Values)
			{
				cell.SetState(true, -1);
			}

			AddNewCells();
		}

		public void OnClick(PointyHexPoint point)
		{
			LinesCell clickedCell = grid[point];

			if (isHoldingCell)
			{
				if (clickedCell.IsEmpty)
				{
					MoveCell(clickedCell);

					if (!ClearLinesAroundPoint(point))
					{
						AddNewCells();
					} //otherwise, give the player a "free" turn.

				}
				else if (clickedCell == cellThatIsBeingHeld)
				{
					DropCell();
				}
			}
			else
			{
				if (!clickedCell.IsEmpty)
				{
					PickUpCell(clickedCell);
				}
			}
		}

		#endregion

		#region Implementation

		private void PickUpCell(LinesCell clickedCell)
		{
			cellThatIsBeingHeld = clickedCell;
			isHoldingCell = true;
			clickedCell.HighlightOn = true;
		}

		private void DropCell()
		{
			cellThatIsBeingHeld.HighlightOn = false;
			cellThatIsBeingHeld = null;
			isHoldingCell = false;
		}

		private void MoveCell(LinesCell clickedCell)
		{
			cellThatIsBeingHeld.HighlightOn = false;
			SwapCells(cellThatIsBeingHeld, clickedCell);
			cellThatIsBeingHeld = null;
			isHoldingCell = false;
		}

		private void AddNewCells()
		{
			var emptyCells = GetEmptyCells();
			IEnumerable<PointyHexPoint> cellsToPlace = emptyCells.SampleRandom(cellsPerTurn);

			foreach (PointyHexPoint point in cellsToPlace)
			{
				SetCellToRandom(grid[point]);
				ClearLinesAroundPoint(point);
			}

			emptyCells = GetEmptyCells();

			if (!emptyCells.Any())
			{
				Debug.Log("Game Ends!");
			}
		}

		private IEnumerable<PointyHexPoint> GetEmptyCells()
		{
			return grid.WhereCell(cell => cell.IsEmpty);
		}

		private void SetCellToRandom(LinesCell cell)
		{
			int newColor = (Random.Range(0, colorCount));
			cell.SetState(false, newColor);
		}

		private static void SwapCells(LinesCell cell1, LinesCell cell2)
		{
			int tempColor = cell1.Type;
			bool tempIsEmpty = cell1.IsEmpty;

			cell1.SetState(cell2.IsEmpty, cell2.Type);
			cell2.SetState(tempIsEmpty, tempColor);
		}

		private bool ClearLinesAroundPoint(PointyHexPoint point)
		{
			bool wasLinesRemoved = false;
			IEnumerable<IEnumerable<PointyHexPoint>> lines =
				from line in Algorithms.GetConnectedLines(grid, point, (p1, p2) => grid[p1].Color == grid[p2].Color)
				where line.Count() >= lineLength
				select line;

			foreach (IEnumerable<PointyHexPoint> line in lines)
			{
				foreach (PointyHexPoint linePoint in line)
				{
					grid[linePoint].SetState(true, -1);
				}

				wasLinesRemoved = true;
			}

			return wasLinesRemoved;
		}

		#endregion
	}
}