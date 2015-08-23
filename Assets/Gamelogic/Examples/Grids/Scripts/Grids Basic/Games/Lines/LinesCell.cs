//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

namespace Gamelogic.Grids.Examples
{
	public class LinesCell : SpriteCell
	{
		public bool IsEmpty { get; private set; }

		public int Type { get; private set; }

		public void SetState(bool isEmpty, int type)
		{
			IsEmpty = isEmpty;
			Type = isEmpty ? -1 : type;

			Color = (isEmpty ? UnityEngine.Color.white : ExampleUtils.Colors[type]);
		}
	}
}