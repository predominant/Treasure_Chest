//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System.Collections;
using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	public class StressTestRect : GLMonoBehaviour
	{
		public TileCell cellPrefab;
		public int cellsPerIteration = 1000;
		public Camera cam;
		public int width = 500;
		public int height = 500;

		private RectGrid<TileCell> grid;
		private IMap3D<RectPoint> map;
		private int totalCellCount;

		public void Start()
		{
			StartCoroutine(BuildGrid());
		}

		public void OnGUI()
		{
			GUILayout.TextField("Rects: " + totalCellCount);
		}

		public void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				var worldPosition = GridBuilderUtils.ScreenToWorld(Input.mousePosition);
				var gridPoint = map[worldPosition];

				if (grid.Contains(gridPoint))
				{
					if (grid[gridPoint] != null)
					{
						grid[gridPoint].gameObject.SetActive(!grid[gridPoint].gameObject.activeInHierarchy);
					}
				}
			}

			if (Input.GetKey(KeyCode.UpArrow))
			{
				cam.transform.position = cam.transform.position + Vector3.up*10f;
			}

			if (Input.GetKey(KeyCode.DownArrow))
			{
				cam.transform.position = cam.transform.position + Vector3.down*10f;
			}

			if (Input.GetKey(KeyCode.LeftArrow))
			{
				cam.transform.position = cam.transform.position + Vector3.left*10f;
			}

			if (Input.GetKey(KeyCode.RightArrow))
			{
				cam.transform.position = cam.transform.position + Vector3.right*10f;
			}
		}

		public IEnumerator BuildGrid()
		{
			totalCellCount = 0;
			grid = RectGrid<TileCell>.Rectangle(width, height);

			map = new RectMap(new Vector2(80, 80)*3)
				.To3DXY();

			int cellCount = 0;

			foreach (var point in grid)
			{
				var cell = Instantiate(cellPrefab);
				var worldPoint = map[point];

				cell.transform.localPosition = worldPoint;

				cellCount++;
				totalCellCount++;

				grid[point] = cell;

				if (cellCount >= cellsPerIteration)
				{
					cellCount = 0;
					yield return null;
				}
			}
		}
	}
}