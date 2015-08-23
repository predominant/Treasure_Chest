//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	[AddComponentMenu("Gamelogic/Examples/PolarPointyBrickGridMain")]
	public class PolarPointyBrickGridMain : GLMonoBehaviour
	{
		public MeshTileCell cellPrefab;
		public GameObject gridRoot;

		private IGrid<MeshTileCell, PointyHexPoint> grid;
		private PolarPointyBrickMap map;

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

			grid = PointyHexGrid<MeshTileCell>.HorizontallyWrappedRectangle(width, height);
			map = new PolarPointyBrickMap(Vector2.zero, 50, 350, new VectorPoint(width, height));

			foreach (var point in grid)
			{
				var cell = Instantiate(cellPrefab);
				cell.transform.parent = gridRoot.transform;

				Mesh mesh = cell.GetComponent<MeshFilter>().mesh;

				float innerRadius = map.GetInnerRadius(point) + border/2;
				float outerRadius = map.GetOuterRadius(point) - border/2;
				float startAngle = map.GetStartAngleZ(point);
				float endAngle = map.GetEndAngleZ(point) - border*Mathf.Rad2Deg/outerRadius;
				int quadCount = Mathf.CeilToInt(outerRadius*2*Mathf.PI/(quadSize*width));

				MeshUtils.MakeBandedSector(mesh, startAngle, endAngle, innerRadius, outerRadius, quadCount, v => v);

				cell.Color = ExampleUtils.Colors[point.GetColor(3, -1, 2)];

				Debug.Log(cell.Color);

				cell.HighlightOn = false;
				cell.__CenterOffset = map[point].XYTo3D();

				grid[point] = cell;
			}
		}

		public void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				var worldPosition = GridBuilderUtils.ScreenToWorld(gridRoot, Input.mousePosition);
				var gridPoint = map[worldPosition];

				if (grid.Contains(gridPoint))
				{
					foreach (var point in grid.GetNeighbors(gridPoint))
					{
						grid[point].HighlightOn = !grid[point].HighlightOn;
					}
				}
			}
		}
	}
}