using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	public class PointListTest : GLMonoBehaviour
	{
		public SpriteCell cellPrefab;
		public GameObject root;
		public Texture2D plane;

		public Vector2 cellDimensions = new Vector2(30, 30);

		private LineGrid<SpriteCell> grid;
		private IMap3D<LinePoint> voronoiMap;

		public void Start()
		{
			BuildGrid();
		}

		public void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Vector3 worldPosition = GridBuilderUtils.ScreenToWorld(root, Input.mousePosition);

				var gridPoint = voronoiMap[worldPosition];

				if (grid.Contains(gridPoint))
				{
					grid[gridPoint].HighlightOn = !grid[gridPoint].HighlightOn;
				}
			}
		}


		private void BuildGrid()
		{
			var pointList = PoissonDisk.GeneratePoisson(ExampleUtils.ScreenRect, cellDimensions.magnitude, 10);
			grid = LineGrid<SpriteCell>.BeginShape().Segment(pointList.Count).EndShape();
			var map2D = VoronoiMap<LinePoint>.MakeMap(pointList);

			voronoiMap = map2D.To3DXY();

			foreach (var point in grid)
			{
				var cell = Instantiate(cellPrefab);
				Vector3 worldPoint = voronoiMap[point];

				cell.transform.parent = root.transform;
				cell.transform.localScale = Vector3.one;
				cell.transform.localPosition = worldPoint;

				cell.Color = ExampleUtils.Colors[ColorFunction(point)] + Color.white*0.1f;
				cell.name = point.ToString();

				grid[point] = cell;
			}

			ExampleUtils.PaintScreenTexture(plane, voronoiMap.To2D(), ColorFunction);
		}

		private int ColorFunction(LinePoint point)
		{
			if (point == -1) return 16;

			return point.GetColor(2)*4 + point.GetColor(16)/4;
		}
	}
}