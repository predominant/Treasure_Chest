//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	/**
		This example shows you how to use a brick mapper with a hex grid.
	
		(You can set the shape in the inspector dynamically).
	*/
	public class PointyBrickTest : GLMonoBehaviour
	{
		public SpriteCell cellPrefab;
		public GameObject root;

		private PointyHexGrid<SpriteCell> grid;
		private IMap3D<PointyHexPoint> map;

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
				PointyHexPoint hexPoint = map[worldPosition];

				if (grid.Contains(hexPoint))
				{
					grid[hexPoint].HighlightOn = !grid[hexPoint].HighlightOn;
				}
			}
		}

		private void BuildGrid()
		{
			const int width = 6;
			const int height = 9;

			grid = PointyHexGrid<SpriteCell>.FatRectangle(width, height);

			map = new PointyBrickMap(cellPrefab.Dimensions)
				.AnchorCellMiddleCenter()
				.WithWindow(ExampleUtils.ScreenRect)
				.AlignMiddleCenter(grid)
				.To3DXY()
				;

			foreach (PointyHexPoint point in grid)
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