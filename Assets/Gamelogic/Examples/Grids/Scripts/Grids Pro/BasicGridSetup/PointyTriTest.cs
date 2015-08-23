//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	public class PointyTriTest : GLMonoBehaviour
	{
		public SpriteCell cellPrefab;
		public GameObject root;

		private readonly Vector2 TriDimensions = new Vector2(69, 80)*2.6f;
		private PointyTriGrid<SpriteCell> grid;
		private IMap3D<PointyTriPoint> map;

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
				PointyTriPoint triPoint = map[worldPosition];

				if (grid.Contains(triPoint))
				{
					grid[triPoint].HighlightOn = !grid[triPoint].HighlightOn;
				}
			}
		}

		private void BuildGrid()
		{
			const int side = 3;

			grid = PointyTriGrid<SpriteCell>
				.Star(side);

			map = new PointyTriMap(TriDimensions)
				.WithWindow(ExampleUtils.ScreenRect)
				.AlignMiddleCenter(grid)
				.AnchorCellMiddleCenter()
				.To3DXY();

			foreach (PointyTriPoint point in grid)
			{
				SpriteCell cell = Instantiate(cellPrefab);
				Vector2 worldPoint = map[point];

				cell.transform.parent = root.transform;
				cell.transform.localScale = Vector3.one;
				cell.transform.localPosition = worldPoint;

				cell.Color = ExampleUtils.Colors[point.GetColor(5, 1, 1)];
				cell.name = point.ToString();
				cell.SetAngle(360f/PointyTriPoint.SpliceCount*point.I);

				grid[point] = cell;
			}
		}
	}
}