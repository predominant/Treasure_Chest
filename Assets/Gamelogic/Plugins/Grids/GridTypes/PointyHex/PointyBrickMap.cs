//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		A map that can be used with a PointyHexGrid to get a brick-wall pattern. The cells are rectangular.
	
		@link_working_with_maps
		
		
		@version1_0

		@ingroup Maps
	*/
	public class PointyBrickMap : AbstractMap<PointyHexPoint>
	{
		public PointyBrickMap(Vector2 cellDimensions) :
			base(cellDimensions)
		{ }

		override public Vector2 GridToWorld(PointyHexPoint point)
		{
			float sX = (point.X + point.Y / 2.0f) * cellDimensions.x;
			float sY = point.Y * cellDimensions.y;

			return new Vector2(sX, sY);
		}

		override public PointyHexPoint RawWorldToGrid(Vector2 point)
		{
			int y = GLMathf.FloorToInt((point.y + cellDimensions.y / 2) / cellDimensions.y);
			int x = GLMathf.FloorToInt((point.x - y * cellDimensions.x / 2 + cellDimensions.x / 2) / cellDimensions.x);

			return new PointyHexPoint(x, y);
		}
	}
}