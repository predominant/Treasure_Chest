//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	public class FlatTriTest : GLMonoBehaviour
	{
		private readonly Vector2 TriDimensions = new Vector2(80, 69)*2.6f;

		public SpriteCell cellPrefab;
		public GameObject root;

		private FlatTriGrid<SpriteCell> grid;
		private IMap3D<FlatTriPoint> map;

		public void Start()
		{
			BuildGrid();
		}

		public void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Vector2 worldPosition = GridBuilderUtils.ScreenToWorld(root, Input.mousePosition);
				FlatTriPoint triPoint = map[worldPosition];

				if (grid.Contains(triPoint))
				{
					grid[triPoint].HighlightOn = !grid[triPoint].HighlightOn;
				}
			}
		}

		private void BuildGrid()
		{
			const int side = 5;
			grid = FlatTriGrid<SpriteCell>
				.BeginShape()
				.Hexagon(side)
				.EndShape();

			map = new FlatTriMap(TriDimensions)
				.WithWindow(ExampleUtils.ScreenRect)
				.AlignMiddleCenter(grid)
				.AnchorCellMiddleCenter()
				.To3DXY();

			foreach (FlatTriPoint point in grid)
			{
				SpriteCell cell = Instantiate(cellPrefab);
				Vector3 worldPoint = map[point];

				cell.transform.parent = root.transform;
				cell.transform.localScale = Vector3.one;
				cell.transform.localPosition = worldPoint;

				cell.Color = ExampleUtils.Colors[point.GetColor2()*2];
				cell.name = point.ToString();
				cell.SetAngle(360/FlatTriPoint.SpliceCount*point.I);

				grid[point] = cell;
			}
		}
	}
}