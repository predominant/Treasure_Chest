//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamelogic.Grids.Examples
{
/**
	This example gives an example of building a new 
	algorithm on top of the grid components. 
*/

	[AddComponentMenu("Gamelogic/Examples/Diffusion (Editor)")]
	public class DiffusionHex : GridBehaviour<PointyHexPoint>
	{
		private Color offColor = ExampleUtils.Colors[4];
		private Color onColor = ExampleUtils.Colors[7];

		private IGrid<float, PointyHexPoint> gas;

		public void Start()
		{
			gas = Grid.CloneStructure<float>();

			foreach (var point in gas)
			{
				gas[point] = 0;
			}
		}

		private float CalculateAverage(PointyHexPoint point, IEnumerable<PointyHexPoint> neighbors)
		{
			float sum = neighbors
				.Select(x => gas[x])
				.Aggregate((p, q) => p + q) + gas[point];

			return sum/(neighbors.Count() + 1);
		}

		public void Update()
		{
			ProcessInput();
			Algorithms.AggregateNeighborhood(gas, CalculateAverage); //This adds the 

			foreach (var point in gas)
			{
				UpdateCell(point);
			}
		}

		private void ProcessInput()
		{
			if (Input.GetMouseButton(0))
			{
				var gridPoint = MousePosition;

				if (Grid.Contains(gridPoint))
				{
					gas[gridPoint] = 2;
				}
			}

			if (Input.GetMouseButton(1))
			{
				var gridPoint = MousePosition;

				if (Grid.Contains(gridPoint))
				{
					gas[gridPoint] = 0;
				}
			}
		}

		private void UpdateCell(PointyHexPoint point)
		{
			Color newColor = ExampleUtils.Blend(gas[point], offColor, onColor);
			Grid[point].Color = newColor;
		}
	}
}