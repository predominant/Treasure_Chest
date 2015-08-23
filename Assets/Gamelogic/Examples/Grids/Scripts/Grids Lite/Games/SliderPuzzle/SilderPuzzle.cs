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
	This example shows how the Grids library work with normal Unity Planes.
*/

	public class SilderPuzzle : GridBehaviour<RectPoint>
	{
		public Texture2D puzzleImage;

		private RectGrid<TileCell> gridCopy;
		private RectPoint emptyCell;

		private List<Material> materialBag;

		public override void InitGrid()
		{
			if (materialBag == null)
			{
				materialBag = new List<Material>();
			}

			SetupGrid();
			InitPuzzle();
		}

		public void OnDisable()
		{
			foreach (var material in materialBag)
			{
				Destroy(material);
			}

			materialBag.Clear();
		}

		public void Update()
		{
			if (IsGameFinished())
			{
				Debug.Log("Game finished: you solved the puzzle!");
			}
		}

		private bool IsGameFinished()
		{
			return Grid.All(point => gridCopy[point] == Grid[point]);
		}

		public void OnClick(RectPoint point)
		{
			if (Grid.GetNeighbors(point).Contains(emptyCell))
			{
				SwapWithEmpty(point);
			}
		}

		private void SwapWithEmpty(RectPoint gridPosition)
		{
			var tmpObject = Grid[emptyCell];
			Grid[emptyCell] = Grid[gridPosition];
			Grid[gridPosition] = tmpObject;

			var tmpPosition = Grid[emptyCell].transform.localPosition;
			Grid[emptyCell].transform.localPosition = Grid[gridPosition].transform.localPosition;
			Grid[gridPosition].transform.localPosition = tmpPosition;

			emptyCell = gridPosition;
		}

		private void SetupGrid()
		{
			gridCopy = (RectGrid<TileCell>) Grid.Clone();

			var map = new RectMap(Vector2.one)
				.WithWindow(new Rect(0, 0, 1, 1))
				.Stretch(Grid);

			var textureScaleVector = new Vector2(1f/GridBuilder.Dimensions.X, 1f/GridBuilder.Dimensions.Y);

			foreach (var point in Grid)
			{
				var cellObject = (UVCell) Grid[point];

				cellObject.SetTexture(puzzleImage);
				cellObject.SetUVs(map[point], textureScaleVector);

				materialBag.Add(cellObject.Material);
			}

			emptyCell = RectPoint.Zero;
			Grid[emptyCell].GetComponent<MeshRenderer>().enabled = false;

		}

		private void InitPuzzle()
		{
			var memory = new Queue<RectPoint>();

			memory.Enqueue(emptyCell);
			memory.Enqueue(emptyCell);

			for (var i = 0; i < 2*Grid.Count(); i++)
			{
				var oldPath = memory.Dequeue();

				var randomNeighbor = Grid.GetNeighbors(emptyCell)
					.Where(point => (point != oldPath))
					.SampleRandom(1)
					.First();

				memory.Enqueue(randomNeighbor);
				SwapWithEmpty(randomNeighbor);
			}
		}
	}
}