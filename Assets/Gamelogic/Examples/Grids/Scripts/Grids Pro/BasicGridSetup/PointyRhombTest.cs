//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	public class PointyRhombTest : GLMonoBehaviour
	{
		private readonly Vector2 CellDimensions = new Vector2(400, 350);

		public SpriteCell cellPrefab;
		public GameObject root;

		private PointyRhombGrid<SpriteCell> grid;
		private IMap3D<PointyRhombPoint> map;

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

				PointyRhombPoint gridPoint = map[worldPosition];

				if (grid.Contains(gridPoint))
				{
					grid[gridPoint].HighlightOn = !grid[gridPoint].HighlightOn;
				}
			}
		}

		private void BuildGrid()
		{
			const int width = 5;
			const int height = 5;

			grid = PointyRhombGrid<SpriteCell>
				.BeginShape()
				.Rectangle(width, height)
				.EndShape();

			map = new PointyRhombMap(CellDimensions)
				.WithWindow(ExampleUtils.ScreenRect)
				.AlignMiddleCenter(grid)
				.To3DXY();


			foreach (PointyRhombPoint point in grid)
			{
				SpriteCell cell = Instantiate(cellPrefab);
				Vector3 worldPoint = map[point];
				cell.transform.parent = root.transform;
				cell.transform.localScale = Vector3.one;
				cell.transform.localPosition = worldPoint;
				cell.Color = ExampleUtils.Colors[point.GetColor12()];
				cell.name = point.ToString();
				cell.SetAngle(-360f/PointyRhombPoint.SpliceCount*point.I);

				grid[point] = cell;
			}
		}
	}
}