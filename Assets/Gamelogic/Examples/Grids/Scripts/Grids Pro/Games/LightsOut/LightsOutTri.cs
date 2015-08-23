//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System.Linq;
using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	public class LightsOutTri : GridBehaviour<FlatTriPoint>
	{
		public Color offColor = Color.black;
		public Color onColor = Color.white;

		public void OnGUI()
		{
			if (GUILayout.Button("Reset"))
			{
				Reset();
			}
		}

		public override void InitGrid()
		{
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

		public void OnClick(FlatTriPoint point)
		{
			ToggleCellAt(point);
		}

		private void ToggleCellAt(FlatTriPoint centerPoint)
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