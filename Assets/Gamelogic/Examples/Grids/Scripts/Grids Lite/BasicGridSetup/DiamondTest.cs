//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	/**
		This example shows how to use a diamond grid.
	
		It also shows how to use transforms such as rotation 
		and scale on the map.
	*/
	public class DiamondTest : GLMonoBehaviour
	{
		public SpriteCell cellPrefab;
		public GameObject root;

		private DiamondGrid<SpriteCell> grid;
		private IMap3D<DiamondPoint> map;

		public void Start()
		{
			BuildGrid();
		}

		private void BuildGrid()
		{
			grid = DiamondGrid<SpriteCell>.ThinRectangle(5, 5);

			map = new DiamondMap(cellPrefab.Dimensions)
				.WithWindow(ExampleUtils.ScreenRect)
				.AlignMiddleCenter(grid)
				.To3DXY();

			foreach (DiamondPoint point in grid)
			{
				var cell = Instantiate(cellPrefab);
				Vector3 worldPoint = map[point];

				cell.transform.parent = root.transform;
				cell.transform.localScale = Vector3.one;
				cell.transform.localPosition = worldPoint;

				cell.Color = ExampleUtils.Colors[point.GetColor4()];
				cell.name = point.ToString();

				grid[point] = cell;
			}
		}

		public void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Vector2 worldPosition = GridBuilderUtils.ScreenToWorld(root, Input.mousePosition);

				DiamondPoint point = map[worldPosition];

				if (grid.Contains(point))
				{
					//Toggle the highlight
					grid[point].HighlightOn = !grid[point].HighlightOn;
				}
			}
		}
	}
}