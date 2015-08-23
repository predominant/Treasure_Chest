//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	/**
		Shows how to set up a hex grid.
	
		You can change the shape dynamically in the inspector.
	*/
	public class FlatHexTest : GLMonoBehaviour
	{
		private readonly Vector2 HexDimensions = new Vector2(239, 206);

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
				Vector3 worldPosition = GridBuilderUtils.ScreenToWorld(root, Input.mousePosition);
				FlatHexPoint hexPoint = map[worldPosition];

				if (grid.Contains(hexPoint))
				{
					grid[hexPoint].HighlightOn = !grid[hexPoint].HighlightOn;
				}
			}
		}

		private void BuildGrid()
		{
			root.transform.DestroyChildren();

			grid = FlatHexGrid<SpriteCell>.Hexagon(4);

			map = new FlatHexMap(HexDimensions)
				.AnchorCellMiddleCenter()
				.WithWindow(ExampleUtils.ScreenRect)
				.AlignMiddleCenter(grid)
				.To3DXY()
				;

			foreach (FlatHexPoint point in grid)
			{
				SpriteCell cell = Instantiate(cellPrefab);
				Vector3 worldPoint = map[point];

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