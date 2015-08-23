// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) Gamelogic (Pty) Ltd            //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Represents a cell whose color can be set. 

		@version1_8
		@ingroup UnityEditorSupport
	*/
	public interface IColorableCell : ICell
	{
		/**
			The main color of the tile. 

			This is used to set the color of tiles dynamically.
		*/
		Color Color { get; set; }
	}
}