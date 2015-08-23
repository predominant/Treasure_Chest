//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		The default map between world coordinates and FlatTri coordinates.
	
		@link_working_with_maps
		
		
		@version1_0

		@ingroup Maps
	*/
	public class FlatTriMap : AbstractMap<FlatTriPoint>
	{
		private readonly PointyHexMap baseMap;

		public FlatTriMap(Vector2 cellDimensions) :
			base(cellDimensions)
		{
			var hexDimensions =
				new Vector2(cellDimensions.x, 4f/3f*cellDimensions.y);

			baseMap = new PointyHexMap(hexDimensions);
		}

		public override Vector2 GridToWorld(FlatTriPoint point)
		{
			Vector2 basePoint = baseMap[point.BasePoint];

			if (point.I == 1)
			{
				basePoint += new Vector2(cellDimensions.x / 2, 0);
			}

			return basePoint;
		}

		public override FlatTriPoint RawWorldToGrid(Vector2 point)
		{
			float hexSize = cellDimensions.y * 2;

			float y = point.x / hexSize * Mathf.Sqrt(3);
			float x = (point.y - cellDimensions.y / 2) / hexSize;

			int ti = Mathf.FloorToInt(-x + y);
			int tj = Mathf.FloorToInt(x + y) + 1;
			int tk = Mathf.FloorToInt(-2 * x);

			return new FlatTriPoint(ti, -tk, GLMathf.Mod(tj + tk + ti, 2));
		}
	}
}