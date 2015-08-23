//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Maps between RectPoints grid points and Vector2 world points. 
	
		@link_working_with_maps
		
		
		@version1_0

		@ingroup Maps
	*/
	public class RectMap : AbstractMap<RectPoint>
	{
		public RectMap(Vector2 cellDimensions) :
			base(cellDimensions)
		{ }

		public override Vector2 GridToWorld(RectPoint point)
		{
			return new Vector2(point.X * cellDimensions.x, point.Y * cellDimensions.y);
		}

		public override RectPoint RawWorldToGrid(Vector2 point)
		{
			return new RectPoint(
				GLMathf.FloorToInt((point.x + cellDimensions.x / 2) / cellDimensions.x),
				GLMathf.FloorToInt((point.y + cellDimensions.y / 2) / cellDimensions.y));
		}
	}
}