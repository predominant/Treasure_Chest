using UnityEngine;

namespace Gamelogic.Grids.Examples
{

	public class LayeredGridTest : GLMonoBehaviour
	{
		public Block cellPrefab;

		private LayeredGrid<Block, PointyHexPoint> grid;
		private SimpleLayeredMap<PointyHexPoint> map;

		public void Start()
		{
			map = new SimpleLayeredMap<PointyHexPoint>(new PointyHexMap(new Vector2(69, 80)*5f), 200, 0);

			var shapes = new[]
			{
				PointyHexGrid<Block>.BeginShape().Hexagon(6),
				PointyHexGrid<Block>.BeginShape().Hexagon(5),
				PointyHexGrid<Block>.BeginShape().Hexagon(4),
				PointyHexGrid<Block>.BeginShape().Hexagon(3),
				PointyHexGrid<Block>.BeginShape().Hexagon(2),
				PointyHexGrid<Block>.BeginShape().Hexagon(1)
			};

			grid = LayeredGrid<Block, PointyHexPoint>.Make<
				PointyHexShapeInfo<Block>,
				PointyHexGrid<Block>,
				PointyHexPoint, PointyHexPoint, PointyHexOp<Block>>(shapes);

			foreach (LayeredPoint<PointyHexPoint> point in grid)
			{
				var cell = Instantiate(cellPrefab);

				cell.transform.parent = transform;
				cell.transform.localPosition = map[point];

				var color = ExampleUtils.Colors[(point.Point.GetColor1_3()) + 4];
				cell.GetComponent<Renderer>().material.color = color;

				cell.name = point.ToString();

				grid[point] = cell;
			}
		}

		public void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				var mousePosition = Input.mousePosition;
				var ray = Camera.main.ScreenPointToRay(mousePosition);

				RaycastHit hitInfo;

				if (Physics.Raycast(ray, out hitInfo))
				{
					var worldPoint = hitInfo.point;
					var gridPoint = map[worldPoint];

					if (grid.Contains(gridPoint))
					{
						grid[gridPoint].GetComponent<Renderer>().material.color = ExampleUtils.Colors[7];
					}
				}
			}
		}
	}
}