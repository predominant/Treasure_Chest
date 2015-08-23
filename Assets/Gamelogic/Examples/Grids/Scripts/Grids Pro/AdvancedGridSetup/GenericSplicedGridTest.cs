//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2014 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids.Examples
{
/**
	This is an example of a generic spliced grid in combination with a Voronoi map.
	
	The base grid is a pointy hex grid. Each cell is then spliced based on the splice 
	points. The splice points are relative to the cell at (0, 0), and in the same dimensions.
	
	The Voronoi map is then used to map the plane to the grid. 

	For this example, we also paint cells to an image to make it easier to see what happens
	(this is quite slow, but interaction with the grid is still snappy enough).
*/

	public class GenericSplicedGridTest : GLMonoBehaviour
	{
		public SpriteCell cellPrefab;
		public GameObject root;
		public Texture2D plane;
		public Vector2 cellDimensions = new Vector2(60, 60);
		public Vector2[] spliceOffsets;

		private SplicedGrid<SpriteCell, PointyHexPoint> grid;
		private IMap3D<SplicedPoint<PointyHexPoint>> map;

		public void Start()
		{
			BuildGrid();
		}

		public void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Vector3 worldPosition = GridBuilderUtils.ScreenToWorld(root, Input.mousePosition);
				var gridPoint = map[worldPosition];

				if (grid.Contains(gridPoint))
				{
					grid[gridPoint].HighlightOn = !grid[gridPoint].HighlightOn;
				}
			}
		}

		private void BuildGrid()
		{
			//This is the base grid, we will use it to define
			//the shape of the splice grid.
			//The contents is not important.
			var baseGrid = PointyHexGrid<bool>
				.BeginShape()
				.Hexagon(5)
				.EndShape();

			//This is the base map, used for the course 
			//mapping
			var baseMap = new PointyHexMap(cellDimensions)
				.WithWindow(ExampleUtils.ScreenRect)
				.AlignMiddleCenter(baseGrid);

			//Now we make the actual spliced grid.
			//We feed it the base grid, and the number
			//of splices we want.
			grid = new SplicedGrid<SpriteCell, PointyHexPoint>(baseGrid, spliceOffsets.Length);

			//Now we make a spliced map. This is just a one-way map --
			//it only maps grid points to the world (using the base map plus 
			//splice point offsets
			var splicedMap = new SplicedMap<PointyHexPoint>(baseMap, spliceOffsets);

			//Finally, we make the above into a two way map. This map uses a Vonoroi diagram
			//to do the inverse mapping
			map = new VoronoiMap<SplicedPoint<PointyHexPoint>>(grid, splicedMap).To3DXY();

			//Then we instantiate cells as usual, and put them in our grid.
			foreach (var point in grid)
			{
				var cell = Instantiate(cellPrefab);
				Vector3 worldPoint = map[point];

				cell.transform.parent = root.transform;
				cell.transform.localScale = Vector3.one;
				cell.transform.localPosition = worldPoint;

				//slightly lighter than the DefaultColors we will use to paint the background
				cell.Color = ExampleUtils.Colors[ColorFunction(point)] + Color.white*0.1f;
				cell.name = point.ToString();

				grid[point] = cell;
			}

			// To make it easier to see how points are mapped, we 
			ExampleUtils.PaintScreenTexture(plane, map.To2D(), ColorFunction);
		}

		private int ColorFunction(SplicedPoint<PointyHexPoint> point)
		{
			return (point.BasePoint.GetColor3_7()*spliceOffsets.Length + point.I)%11;
		}
	}
}