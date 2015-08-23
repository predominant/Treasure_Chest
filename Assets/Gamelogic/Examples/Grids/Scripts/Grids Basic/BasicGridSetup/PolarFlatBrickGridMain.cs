//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	[AddComponentMenu("Gamelogic/Examples/PolarFlatBrickGridMain")]
	public class PolarFlatBrickGridMain : GLMonoBehaviour
	{
		public MeshTileCell cellPrefab;
		public GameObject gridRoot;

		private IGrid<MeshTileCell, FlatHexPoint> grid;
		private PolarFlatBrickMap map;

		public void Start()
		{
			BuildGrid();
		}

		private void BuildGrid()
		{
			const int width = 6;
			const int height = 3;
			const float border = 0;
			const int quadCount = 15;

			grid = FlatHexGrid<MeshTileCell>.HorizontallyWrappedRectangle(width, height);
			map = new PolarFlatBrickMap(Vector2.zero, 50, 300, new VectorPoint(width, height));

			foreach (var point in grid)
			{
				var cell = Instantiate(cellPrefab);
				cell.transform.parent = gridRoot.transform;

				Mesh mesh = cell.GetComponent<MeshFilter>().mesh;

				float innerRadius = map.GetInnerRadius(point) + border/2;
				float outerRadius = map.GetOuterRadius(point) - border/2;
				float startAngle = map.GetStartAngleZ(point);
				float endAngle = map.GetEndAngleZ(point) - border*Mathf.Rad2Deg/outerRadius;

				MeshUtils.MakeBandedSector(mesh, startAngle, endAngle, innerRadius, outerRadius, quadCount, v => v);

				cell.Color = ExampleUtils.Colors[point.GetColor1_3()];
				cell.HighlightOn = false;
				cell.__CenterOffset = map[point].XYTo3D();

				grid[point] = cell;
			}
		}

		// Update is called once per frame
		public void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				var worldPosition = GridBuilderUtils.ScreenToWorld(gridRoot, Input.mousePosition);

				var gridPoint = map[worldPosition];

				if (grid.Contains(gridPoint))
				{
					grid[gridPoint].HighlightOn = !grid[gridPoint].HighlightOn;
				}
			}
		}
	}
}