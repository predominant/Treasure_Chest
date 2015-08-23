//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	public class FlatRhombTest : GLMonoBehaviour
	{
		private readonly Vector2 CellDimensions = new Vector2(350, 400);

		public SpriteCell cellPrefab;
		public GameObject root;

		private FlatRhombGrid<SpriteCell> grid;
		private IMap3D<FlatRhombPoint> map;

		public void Start()
		{
			BuildGrid();
		}

		public void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Vector2 worldPosition = GridBuilderUtils.ScreenToWorld(root, Input.mousePosition);
				FlatRhombPoint gridPoint = map[worldPosition];

				if (grid.Contains(gridPoint))
				{
					grid[gridPoint].HighlightOn = !grid[gridPoint].HighlightOn;
				}
			}
		}

		private void BuildGrid()
		{
			const int width = 4;
			const int height = 3;

			grid = FlatRhombGrid<SpriteCell>
				.BeginShape()
				.Rectangle(width, height)
				.EndShape();

			map = new FlatRhombMap(CellDimensions)
				.AnchorCellMiddleCenter()
				.WithWindow(ExampleUtils.ScreenRect)
				.AlignMiddleCenter(grid)
				.To3DXY()
				;

			foreach (FlatRhombPoint point in grid)
			{
				SpriteCell cell = Instantiate(cellPrefab);
				Vector3 worldPoint = map[point];

				cell.transform.parent = root.transform;
				cell.transform.localPosition = worldPoint;
				cell.transform.localScale = Vector3.one;

				cell.Color = ExampleUtils.Colors[point.GetColor12()];
				cell.name = point.ToString();
				cell.SetAngle(-360f/FlatRhombPoint.SpliceCount*point.I);

				grid[point] = cell;
			}
		}
	}
}