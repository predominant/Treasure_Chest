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
	public class LightsOutHex : GridBehaviour<PointyHexPoint>, IResetable
	{
		private const int Symmetry2 = 0;
		private const int Symmetry3 = 1;
		private const int Symmetry6 = 2;

		private Color offColor;
		private Color onColor;

		public override void InitGrid()
		{
			if (GridBuilder.Colors.Length >= 2)
			{
				onColor = GridBuilder.Colors[0];
				offColor = GridBuilder.Colors[1];
			}
			else
			{
				onColor = Color.white;
				offColor = Color.black;
			}

			Reset();
		}

		public void OnGUI()
		{
			if (GUILayout.Button("Reset"))
			{
				Reset();
			}
		}

		public void OnClick(PointyHexPoint point)
		{
			ToggleCellAt(point);
		}

		public void Reset()
		{
			foreach (var point in Grid)
			{
				((SpriteCell) Grid[point]).HighlightOn = false;
				Grid[point].Color = offColor;
			}

			InitGame();
		}

		private void InitGame()
		{
			int pattern = Random.Range(0, 4);

			switch (pattern)
			{
				case 0:
					InitPattern1();
					break;
				case 1:
					InitPattern2();
					break;
				case 2:
					InitPattern1();
					InitPattern2();
					break;
				case 3:
					InitPattern3();
					break;
			}

			if (Grid.All(p => !((SpriteCell) Grid[p]).HighlightOn))
			{
				Reset();
			}
		}

		private void InitPattern2()
		{
			int start = Random.Range(0, 3);
			int end = Random.Range(start, 3);
			int symmetry = Random.Range(0, 3);

			switch (symmetry)
			{
				case Symmetry6:
					for (int i = start; i <= end; i++)
					{
						ToggleCellAt((PointyHexPoint.East + PointyHexPoint.NorthEast)*i);
						ToggleCellAt((PointyHexPoint.West + PointyHexPoint.SouthWest)*i);
						ToggleCellAt((PointyHexPoint.NorthEast + PointyHexPoint.NorthWest)*i);
						ToggleCellAt((PointyHexPoint.SouthWest + PointyHexPoint.SouthEast)*i);
						ToggleCellAt((PointyHexPoint.NorthWest + PointyHexPoint.West)*i);
						ToggleCellAt((PointyHexPoint.SouthEast + PointyHexPoint.East)*i);
					}
					break;
				case Symmetry3:
					for (int i = start; i <= end; i++)
					{
						ToggleCellAt((PointyHexPoint.East + PointyHexPoint.NorthEast)*i);
						ToggleCellAt((PointyHexPoint.SouthWest + PointyHexPoint.SouthEast)*i);
						ToggleCellAt((PointyHexPoint.NorthWest + PointyHexPoint.West)*i);

					}
					break;
				case Symmetry2:
					for (int i = start; i <= end; i++)
					{
						ToggleCellAt((PointyHexPoint.East + PointyHexPoint.NorthEast)*i);
						ToggleCellAt((PointyHexPoint.West + PointyHexPoint.SouthWest)*i);
					}
					break;
			}
		}

		private void InitPattern1()
		{
			int start = Random.Range(0, 5);
			int end = Random.Range(start, 5);
			int symmetry = Random.Range(0, 3);

			switch (symmetry)
			{
				case Symmetry6:
					for (int i = start; i <= end; i++)
					{
						ToggleCellAt(PointyHexPoint.East*i);
						ToggleCellAt(PointyHexPoint.West*i);
						ToggleCellAt(PointyHexPoint.NorthEast*i);
						ToggleCellAt(PointyHexPoint.SouthWest*i);
						ToggleCellAt(PointyHexPoint.NorthWest*i);
						ToggleCellAt(PointyHexPoint.SouthEast*i);
					}
					break;
				case Symmetry3:
					for (int i = start; i <= end; i++)
					{
						ToggleCellAt(PointyHexPoint.East*i);
						ToggleCellAt(PointyHexPoint.SouthWest*i);
						ToggleCellAt(PointyHexPoint.NorthWest*i);

					}
					break;
				case Symmetry2:
					for (int i = start; i <= end; i++)
					{
						ToggleCellAt(PointyHexPoint.East*i);
						ToggleCellAt(PointyHexPoint.West*i);
					}
					break;
			}
		}

		private void InitPattern3()
		{
			var randomPoints = Grid.SampleRandom(2);
			var pattern = new HashSet<PointyHexPoint>();
			int symmetry = Random.Range(0, 3);

			foreach (
				var pointyHexPoints in
					randomPoints.Select(point1 => Grid.Where(p => (p - point1).Magnitude() <= 3).SampleRandom(2))
						.Select(randomGroup => randomGroup as IList<PointyHexPoint> ?? randomGroup.ToList()))
			{
				pattern.AddRange(pointyHexPoints);

				switch (symmetry)
				{
					case Symmetry6:

						pattern.AddRange(pointyHexPoints.Select(p => p.Rotate60()));
						pattern.AddRange(pointyHexPoints.Select(p => p.Rotate120()));
						pattern.AddRange(pointyHexPoints.Select(p => p.Rotate180()));
						pattern.AddRange(pointyHexPoints.Select(p => p.Rotate240()));
						pattern.AddRange(pointyHexPoints.Select(p => p.Rotate300()));

						break;

					case Symmetry3:
						pattern.AddRange(pointyHexPoints.Select(p => p.Rotate120()));
						pattern.AddRange(pointyHexPoints.Select(p => p.Rotate240()));

						break;

					case Symmetry2:
						pattern.AddRange(pointyHexPoints.Select(p => p.Rotate180()));
						break;
				}
			}

			foreach (var point in pattern)
			{
				ToggleCellAt(point);
			}
		}

		private void ToggleCellAt(PointyHexPoint centerPoint)
		{
			foreach (var point in Grid.GetNeighbors(centerPoint))
			{
				var cell = (SpriteCell) Grid[point];

				cell.HighlightOn = !cell.HighlightOn;
				cell.Color = cell.HighlightOn ? onColor : offColor;
			}
		}
	}
}