//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System.Linq;
using UnityEngine;

namespace Gamelogic.Grids.Examples
	{
	/**
		This example shows the Lights Out game on a Cairo grid.
	*/
	public class LightsOutCairo : GridBehaviour<CairoPoint>, IResetable
	{
		private Color offColor;
		private Color onColor;

		public void OnGUI()
		{
			if (GUILayout.Button("Reset"))
			{
				Reset();
			}
		}

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

		public void Reset()
		{
			foreach (var point in Grid)
			{
				((SpriteCell) Grid[point]).HighlightOn = false;
				Grid[point].Color = offColor;
			}

			InitGame();
		}

		public void InitGame()
		{
			//Initialize with random pattern
			Grid.SampleRandom(9).ToList().ForEach(ToggleCellAt);
		}

		public void OnClick(CairoPoint point)
		{
			ToggleCellAt(point);
		}

		private void ToggleCellAt(CairoPoint centerPoint)
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