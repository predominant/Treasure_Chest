//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	/**
		This example shows you how to use a brick map with a hex grid.
	*/
	public class FlatBrickTest : GLMonoBehaviour
	{
		public SpriteCell cellPrefab;
		public GameObject root;

		private FlatHexGrid<SpriteCell> grid;
		private IMap3D<FlatHexPoint> map;

		public void Start()
		{
			BuildGrid();
		}

		public void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Vector3 mousePosition = Input.mousePosition;
				Vector2 worldPosition = GridBuilderUtils.ScreenToWorld(root, mousePosition);
				FlatHexPoint hexPoint = map[worldPosition];

				if (grid.Contains(hexPoint))
				{
					grid[hexPoint].HighlightOn = !grid[hexPoint].HighlightOn;
				}
			}
		}

		private void BuildGrid()
		{
			const int width = 7;
			const int height = 9;

			grid = FlatHexGrid<SpriteCell>.FatRectangle(width, height);
			
			map = new FlatBrickMap(cellPrefab.Dimensions)
				.AnchorCellMiddleCenter()
				.WithWindow(ExampleUtils.ScreenRect)
				.AlignMiddleCenter(grid)
				.To3DXY()
				;

			foreach (FlatHexPoint point in grid)
			{
				SpriteCell cell = Instantiate(cellPrefab);
				Vector2 worldPoint = map[point];

				cell.transform.parent = root.transform;
				cell.transform.localScale = Vector3.one;
				cell.transform.localPosition = worldPoint;

				cell.Color = ExampleUtils.Colors[point.GetColor3_7()];
				cell.name = point.ToString();

				grid[point] = cell;
			}
		}
	}
}