//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	public class RectTest : GLMonoBehaviour
	{
		// This is the prefab that will be used for each cell in the grid. 
		// Normally, you would use your own version that has information
		// related to your game.
		public SpriteCell cellPrefab;

		// All cells will be parented to this object.
		public GameObject root;

		// The grid data structure that contains all cell.
		private RectGrid<SpriteCell> grid;

		// The map (that converts between world and grid coordinates).
		private IMap3D<RectPoint> map;

		public void Start()
		{
			BuildGrid();
		}

		private void BuildGrid()
		{
			// Creates a grid in a rectangular shape.
			grid = RectGrid<SpriteCell>.Rectangle(7, 7);

			// Creates a map...
			map = new RectMap(cellPrefab.Dimensions) // The cell dimensions usually correspond to the visual 
				// part of the sprite in pixels. Here we use the actual 
				// sprite size, which causes a border between cells.
				// 

				.WithWindow(ExampleUtils.ScreenRect) // ...that is centered in the rectangle provided
				.AlignMiddleCenter(grid) // by this and the previous line.
				.To3DXY(); // This makes the 2D map returned by the last function into a 3D map
			// This map assumes the grid is in the XY plane.
			// To3DXZ assumes the grid is in the XZ plane (you have to make sure 
			//your tiles are similarly aligned / rotated).


			foreach (RectPoint point in grid) //Iterates over all points (coordinates) contained in the grid
			{
				SpriteCell cell = Instantiate(cellPrefab); // Instantiate a cell from the given prefab.
				//This generic version of Instantiate is defined in GLMonoBehaviour
				//If you don't want to use GLMonoBehvaiour, you have to cast the result of
				//Instantiate

				Vector3 worldPoint = map[point]; //Calculate the world point of the current grid point

				cell.transform.parent = root.transform; //Parent the cell to the root
				cell.transform.localScale = Vector3.one; //Readjust the scale - the re-parenting above may have changed it.
				cell.transform.localPosition = worldPoint; //Set the localPosition of the cell.

				cell.Color = ExampleUtils.Colors[point.GetColor4()%4*4]; //Sets the color of the cell
				//See http://gamelogic.co.za/2013/12/18/what-are-grid-colorings/ for more information on colorings.

				cell.name = point.ToString(); // Makes it easier to identify cells in the editor.
				grid[point] = cell; // Finally, put the cell in the grid.
			}
		}

		public void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				// If you use a different GUI system, you will probably need a 
				// custom version of this function.
				// This assumes your camera is orthographic. For perspective cameras,
				// you must use a ray casting method instead.
				Vector3 worldPosition = GridBuilderUtils.ScreenToWorld(root, Input.mousePosition);

				// Calculates the grid point that corresponds to the given world coordinate.
				RectPoint point = map[worldPosition];

				// The point may in fact lie outside the grid as we defined it when we built it.
				// So we first check whether the grid contains the point...
				if (grid.Contains(point))
				{
					//... and toggle the highlight of the cell
					grid[point].HighlightOn = !grid[point].HighlightOn;
				}
			}
		}
	}
}