//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		The default maps that works with PointyRhombGrids.

		@link_working_with_maps

		
		
		@version1_0
		
		@ingroup Maps
	*/
	public class PointyRhombMap : AbstractMap<PointyRhombPoint>
	{
		readonly IMap<PointyHexPoint> baseMap;
		private readonly float rhombWidth;
		private readonly float rhombHeight;

		//TODO: Make this constructor take more intuitive parms
		public PointyRhombMap(Vector2 cellDimensions) :
			base(cellDimensions)
		{
			Vector2 hexDimensions = cellDimensions / 2;
			hexDimensions.y = 2 * hexDimensions.y / 1.5f;

			baseMap = new PointyHexMap(hexDimensions).AnchorCellMiddleCenter();

			rhombWidth = cellDimensions.x / Mathf.Sqrt(3);
			rhombHeight = cellDimensions.x;
		}

		override public Vector2 GetCellDimensions(PointyRhombPoint point)
		{
			return point.I == 1 ? new Vector2(rhombHeight, rhombWidth) : new Vector2(rhombHeight / 2, cellDimensions.y);
		}

		public override Vector2 GridToWorld(PointyRhombPoint point)
		{
			Vector2 basePoint = baseMap[point.BasePoint] * 2;

			switch (point.I)
			{
				case 2:
					basePoint += new Vector2(cellDimensions.x / 2, -cellDimensions.y) / 2;
					break;
				case 0:
					basePoint += new Vector2(-cellDimensions.x / 2, -cellDimensions.y) / 2;
					break;
			}

			basePoint += new Vector2(cellDimensions.x / 4, cellDimensions.y / 2);


			return basePoint;
		}

		public override PointyRhombPoint RawWorldToGrid(Vector2 point)
		{
			float hexSize = cellDimensions.x;

			float y = (point.y - cellDimensions.x / 4 / Mathf.Sqrt(3)) / hexSize * Mathf.Sqrt(3);
			float x = (point.x - cellDimensions.x / 4) / hexSize;

			int ti = Mathf.FloorToInt(-x + y);
			int tj = Mathf.FloorToInt(x + y);
			int tk = Mathf.FloorToInt(-2 * x);

			float hi0 = Mathf.FloorToInt((+ti + tk + 2.0f) / 3);
			float hj0 = Mathf.FloorToInt((+tj - tk + 1.0f) / 3);
			float hk0 = hi0 + hj0;

			float hi = -hi0;
			float hj = hk0;

			int index;

			if (
				GLMathf.Mod(tj, 3) == 0 && GLMathf.Mod(tk, 3) == 1 ||
				GLMathf.Mod(tj, 3) == 1 && GLMathf.Mod(tk, 3) == 2 ||
				GLMathf.Mod(tj, 3) == 2 && GLMathf.Mod(tk, 3) == 0)
			{
				index = 0;
			}
			else if (
				GLMathf.Mod(ti, 3) == 0 && GLMathf.Mod(tk, 3) == 1 ||
				GLMathf.Mod(ti, 3) == 1 && GLMathf.Mod(tk, 3) == 0 ||
				GLMathf.Mod(ti, 3) == 2 && GLMathf.Mod(tk, 3) == 2
				)
			{
				index = 2;
			}
			else
			{
				index = 1;
			}

			return new PointyRhombPoint(Mathf.FloorToInt(hi), Mathf.FloorToInt(hj), index);
		}
	}
}