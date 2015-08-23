using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	public class PolarRectGridMain : GLMonoBehaviour
	{
		public MeshTileCell cellPrefab;
		public GameObject gridRoot;

		private IGrid<MeshTileCell, RectPoint> grid;
		private PolarRectMap map;

		public void Start()
		{
			BuildGrid();
		}

		private void BuildGrid()
		{
			const int width = 6;
			const int height = 5;
			const float border = 0f;
			const float quadSize = 15f;

			grid = RectGrid<MeshTileCell>.HorizontallyWrappedParallelogram(width, height);
			map = new PolarRectMap(Vector2.zero, 50, 350, new VectorPoint(width, height));

			foreach (var point in grid)
			{
				var cell = Instantiate(cellPrefab);
				cell.transform.parent = gridRoot.transform;

				float innerRadius = map.GetInnerRadius(point) + border/2;
				float outerRadius = map.GetOuterRadius(point) - border/2;
				float startAngle = map.GetStartAngleZ(point);
				float endAngle = map.GetEndAngleZ(point) - border*Mathf.Rad2Deg/outerRadius;
				int quadCount = Mathf.CeilToInt(outerRadius*2*Mathf.PI/(quadSize*width));

				Mesh mesh = cell.GetComponent<MeshFilter>().mesh;
				MeshUtils.MakeBandedSector(mesh, startAngle, endAngle, innerRadius, outerRadius, quadCount, v => v);

				cell.Color = ExampleUtils.Colors[point.GetColor(6, 3, 1)];
				cell.HighlightOn = false;
				cell.__CenterOffset = map[point].XYTo3D();

				grid[point] = cell;
			}
		}

		// Update is called once per frame
		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				var worldPosition = GridBuilderUtils.ScreenToWorld(gridRoot, Input.mousePosition);
				var gridPoint = map[worldPosition];

				grid[gridPoint].HighlightOn = !grid[gridPoint].HighlightOn;
			}
		}
	}
}