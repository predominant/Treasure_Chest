//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System.Collections;
using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	public class StressTestHex : GLMonoBehaviour
	{
		public TileCell cellPrefab;
		public int cellsPerIteration = 1000;
		public Camera cam;
		public int width = 500;
		public int height = 500;

		private PointyHexGrid<TileCell> grid;
		private IMap3D<PointyHexPoint> map;
		private int totalCellCount;

		public void Start()
		{
			StartCoroutine(BuildGrid());
		}

		public void OnGUI()
		{
			GUILayout.TextField("Hexes: " + totalCellCount);
		}

		public void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Vector3 worldPosition = GridBuilderUtils.ScreenToWorld(Input.mousePosition);
				PointyHexPoint hexPoint = map[worldPosition];

				if (grid.Contains(hexPoint))
				{
					if (grid[hexPoint] != null)
					{
						grid[hexPoint].gameObject.SetActive(!grid[hexPoint].gameObject.activeInHierarchy);
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
			grid = PointyHexGrid<TileCell>.Rectangle(width, height);

			map = new PointyHexMap(new Vector2(69, 80)*3)
				.To3DXY();

			int cellCount = 0;

			foreach (var point in grid)
			{
				var cell = Instantiate(cellPrefab);
				Vector3 worldPoint = map[point];

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