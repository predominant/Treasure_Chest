//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Maps between grid points and world points.

		@link_working_with_maps
		
		
		@version1_0

		@ingroup Maps
	*/
	public class DiamondMap : AbstractMap<DiamondPoint>
	{
		public DiamondMap(Vector2 cellDimensions) :
			base(cellDimensions)
		{}

		public override Vector2 GridToWorld(DiamondPoint point)
		{
			float x = (point.X - point.Y) * cellDimensions.x / 2;
			float y = (point.X + point.Y) * cellDimensions.y / 2;

			return new Vector2(x, y);
		}

		public override DiamondPoint RawWorldToGrid(Vector2 point)
		{
			int x =
				 GLMathf.FloorToInt((point.x + 0 * cellDimensions.x / 2) / cellDimensions.x +
				 /*GLMathf.FloorToInt(*/(point.y + cellDimensions.y / 2) / cellDimensions.y);

			int y =
				 GLMathf.FloorToInt((point.y + cellDimensions.y / 2) / cellDimensions.y -
				 (point.x + 0*cellDimensions.x / 2) / cellDimensions.x);

			return new DiamondPoint(x, y);
		}
	}
}