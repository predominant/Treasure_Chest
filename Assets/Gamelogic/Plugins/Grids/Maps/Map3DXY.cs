//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		The default maps for putting a grid in 3D space in the XY-plane.
	
		
		
		@version1_0

		@ingroup Interface
	*/
	public class Map3DXY<TPoint> : IMap3D<TPoint>
		where TPoint : IGridPoint<TPoint>
	{
		private readonly IMap<TPoint> map2D;
		private readonly float z;

		public Vector3 this[TPoint point]
		{
			get
			{
				var point2D = map2D[point];

				return new Vector3(point2D.x, point2D.y, z);
			}
		}

		public TPoint this[Vector3 point]
		{
			get
			{
				var point2D = new Vector2(point.x, point.y);

				return map2D[point2D];
			}
		}

		public Map3DXY(IMap<TPoint> map2D, float z)
		{
			this.z = z;
			this.map2D = map2D;
		}

		public IMap<TPoint> To2D()
		{
			return map2D;
		}
	}
}