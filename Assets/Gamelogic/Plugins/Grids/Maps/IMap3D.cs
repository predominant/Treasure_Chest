//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		An IMap maps 3D world coordinates to Grid coordinates and vice versa. 

		Many grids provide 2D maps, which can be converted to standard 3D maps 
		using commands such as To3DXY.
	
		You can also provide your own maps, either as implementations of IMap, or IMap3D.
		
		@version1_0

		@ingroup Interface
	*/
	public interface IMap3D<TPoint>
		where TPoint : IGridPoint<TPoint>
	{
		/**
			Gets a world point given a grid point.
		*/
		Vector3 this[TPoint point]
		{
			get;
		}

		/**
			Gets a grid point given a world point.
		*/
		TPoint this[Vector3 point]
		{
			get;
		}

		IMap<TPoint> To2D();
	}
}