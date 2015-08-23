//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		The default map between world coordinates and PointyTri coordinates.

		@link_working_with_maps	
		
		
		@version1_0

		@ingroup Maps
	*/
	public class PointyTriMap : AbstractMap<PointyTriPoint>
	{
		private readonly FlatHexMap baseMap;

		public PointyTriMap(Vector2 cellDimensions) :
			base(cellDimensions)
		{
			var hexDimensions =
				new Vector2(4f / 3f * cellDimensions.x, cellDimensions.y);

			baseMap = new FlatHexMap(hexDimensions);
		}

		public override Vector2 GridToWorld(PointyTriPoint point)
		{
			Vector2 basePoint = baseMap[point.BasePoint];

			if (point.I == 1)
			{
				basePoint += new Vector2(0, cellDimensions.y / 2);
			}

			return basePoint;
		}

		public override PointyTriPoint RawWorldToGrid(Vector2 point)
		{
			float hexSize = cellDimensions.x * 2;

			float x = point.y / hexSize * Mathf.Sqrt(3);
			float y = (point.x - cellDimensions.x / 2) / hexSize /*- cellDimensions.y/2*/;

			int ti = Mathf.FloorToInt(x - y);
			int tj = Mathf.FloorToInt(x + y) + 1;
			int tk = Mathf.FloorToInt(-2 * y);

			return new PointyTriPoint(-tk, ti, GLMathf.Mod(tj + tk + ti, 2));
		}
	}
}