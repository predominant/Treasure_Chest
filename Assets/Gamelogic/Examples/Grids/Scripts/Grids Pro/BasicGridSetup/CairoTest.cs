//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids.Examples
{
/**
	A biasic example using the Cairo grid.
*/

	public class CairoTest : GLMonoBehaviour
	{
		private readonly Vector2 CellDimensions = new Vector2(263, 263);

		public SpriteCell cellPrefab;
		public GameObject root;

		private CairoGrid<SpriteCell> grid;
		private IMap3D<CairoPoint> map;

		public void Start()
		{
			BuildGrid();
		}

		public void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Vector2 worldPosition = GridBuilderUtils.ScreenToWorld(root, Input.mousePosition);
				CairoPoint gridPoint = map[worldPosition];

				if (grid.Contains(gridPoint))
				{
					grid[gridPoint].HighlightOn = !grid[gridPoint].HighlightOn;
				}
			}
		}

		private void BuildGrid()
		{
			const int width = 2;
			const int height = 2;

			grid = CairoGrid<SpriteCell>.Default(width, height);

			map = new CairoMap(CellDimensions)
				.AnchorCellMiddleCenter()
				.WithWindow(ExampleUtils.ScreenRect)
				.AlignMiddleCenter(grid)
				.To3DXY();

			foreach (CairoPoint point in grid)
			{
				SpriteCell cell = Instantiate(cellPrefab);
				Vector3 worldPoint = map[point];

				cell.transform.parent = root.transform;
				cell.transform.localPosition = worldPoint;
				cell.transform.localScale = Vector3.one;

				cell.Color = ExampleUtils.Colors[ColorMap(point.GetColor12())];
				cell.name = PointToString(point);
				cell.SetAngle(360f/CairoPoint.SpliceCount*point.I);

				grid[point] = cell;
			}
		}

		private int ColorMap(int oldColor)
		{
			return ((oldColor%4)*4 + oldColor/4);
		}

		private string PointToString(CairoPoint point)
		{
			return "" + point.X + ", " + point.Y + " | " + point.I;
		}
	}
}