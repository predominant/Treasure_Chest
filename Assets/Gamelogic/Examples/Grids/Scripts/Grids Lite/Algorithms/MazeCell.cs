//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	[AddComponentMenu("Gamelogic/Cells/MazeCell")]
	public class MazeCell : SpriteCell
	{
		public void SetOrientation(int index, bool open)
		{
			FrameIndex = (index + (open ? 3 : 0));
		}
	}
}