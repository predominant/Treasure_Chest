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
	[AddComponentMenu("Gamelogic/Examples/Diffusion (Rect, Editor)")]
	public class DiffusionRect : GridBehaviour<RectPoint>
	{
		public Gradient temperatureGradient = new Gradient
		{
			alphaKeys = new GradientAlphaKey[0],
			colorKeys = new[]
			{
				new GradientColorKey(ExampleUtils.Colors[0], 4),
 				new GradientColorKey(ExampleUtils.Colors[2], 7),
			}
		};

		private IGrid<float, RectPoint> gas;

		public void Start()
		{
			gas = Grid.CloneStructure<float>();

			foreach (var point in gas)
			{
				gas[point] = 0;
			}
		}

		private float CalculateAverage(RectPoint point, IEnumerable<RectPoint> neighbors)
		{
			float sum = neighbors
				.Select(x => gas[x])
				.Aggregate((p, q) => p + q) + gas[point];

			return sum/(neighbors.Count() + 1);
		}

		public void Update()
		{
			ProcessInput();
			
			Algorithms.AggregateNeighborhood(gas, CalculateAverage);

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

		private void UpdateCell(RectPoint point)
		{
			var newColor = temperatureGradient.Evaluate(gas[point]);
			Grid[point].Color = newColor;
		}
	}
}