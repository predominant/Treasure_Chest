//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2014 Gamelogic (Pty) Ltd       //
//----------------------------------------------//
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		A one-way map that converts grid points to worls points. One-way maps are useful 
		for maps that can automatically invert the map, such as VoronoiMap.
		
		@version1_8
		@ingroup Interface
	*/
	public interface IGridToWorldMap<TPoint> where TPoint : IGridPoint<TPoint>
	{
		#region Properties
		/**
			Gets a world point given a grid point.
		*/
		Vector2 this[TPoint point]
		{
			get;
		}
		#endregion
	}
}