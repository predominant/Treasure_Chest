//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2014 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	public class LineGridTest : GLMonoBehaviour
	{
		private readonly Vector2 CellDimensions = new Vector2(30, 30);

		public SpriteCell cellPrefab;
		public GameObject root;
		public Texture2D plane;

		private LineGrid<SpriteCell> grid;
		private IMap3D<LinePoint> map;
		private IMap<LinePoint> map2D;
		private VoronoiMap<LinePoint> voronoiMap;

		public void Start()
		{
			BuildGrid();
		}

		public void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Vector3 worldPosition = GridBuilderUtils.ScreenToWorld(root, Input.mousePosition);
				LinePoint gridPoint = 1;

				for (int i = 0; i < 101; i++)
				{
					gridPoint = voronoiMap[worldPosition];
				}

				if (grid.Contains(gridPoint))
				{
					grid[gridPoint].HighlightOn = !grid[gridPoint].HighlightOn;
				}
			}
		}

		private void BuildGrid()
		{
			grid = LineGrid<SpriteCell>
				.BeginShape()
				.Segment(300)
				.EndShape();

			map2D = new ArchimedeanSpiralMap(CellDimensions, grid)
				.AnchorCellMiddleCenter()
				.WithWindow(ExampleUtils.ScreenRect)
				.AlignMiddleCenter(grid);

			map = map2D
				.To3DXY();

			foreach (var point in grid)
			{
				var cell = Instantiate(cellPrefab);

				Vector3 worldPoint = map[point];

				cell.transform.parent = root.transform;
				cell.transform.localScale = Vector3.one;
				cell.transform.localPosition = worldPoint;

				cell.Color = ExampleUtils.Colors[ColorFunction(point)] + Color.white*0.2f;
				cell.name = point.ToString();

				grid[point] = cell;
			}

			voronoiMap = new VoronoiMap<LinePoint>(grid, map2D);
			ExampleUtils.PaintScreenTexture(plane, voronoiMap, ColorFunction);
		}

		private int ColorFunction(LinePoint point)
		{
			if (point == -1) return 16;

			return point.GetColor(2)*4 + point.GetColor(16)/4;
		}
	}
}