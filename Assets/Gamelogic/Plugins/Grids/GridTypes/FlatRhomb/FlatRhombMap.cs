//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		The default map to be used with FlatRhombGrid.
		
		@link_working_with_maps
		
		
		@version1_0

		@ingroup Maps
	*/
	public class FlatRhombMap : AbstractMap<FlatRhombPoint>
	{
		readonly IMap<FlatHexPoint> baseMap;
		private readonly float rhombWidth;
		private readonly float rhombHeight;

		//TODO: Make this constructor take more intuitive parms
		public FlatRhombMap(Vector2 cellDimensions) :
			base(cellDimensions)
		{
			Vector2 hexDimensions = cellDimensions / 2;
			hexDimensions.x = 2 * hexDimensions.x / 1.5f;

			baseMap = new FlatHexMap(hexDimensions).AnchorCellMiddleCenter();
			rhombWidth = cellDimensions.y / Mathf.Sqrt(3);
			rhombHeight = cellDimensions.y;
		}

		override public Vector2 GetCellDimensions(FlatRhombPoint point)
		{
			return point.I == 2 ? new Vector2(rhombWidth, rhombHeight) : new Vector2(cellDimensions.x, cellDimensions.y / 2);
		}

		public override Vector2 GridToWorld(FlatRhombPoint point)
		{
			Vector2 basePoint = baseMap[point.BasePoint] * 2;

			switch (point.I)
			{
				case 0:
					basePoint += new Vector2(-cellDimensions.x, -cellDimensions.y / 2) / 2;
					break;
				case 1:
					basePoint += new Vector2(-cellDimensions.x, cellDimensions.y / 2) / 2;
					break;
			}

			basePoint += new Vector2(cellDimensions.x / 2, cellDimensions.y / 4);

			return basePoint;
		}

		//TODO: Fix
		public override FlatRhombPoint RawWorldToGrid(Vector2 point)
		{
			float hexSize = cellDimensions.y;

			//basePoint += new Vector2(cellDimensions.x/2, cellDimensions.y / 4);

			float y = (point.y - cellDimensions.y / 4) / hexSize;
			float x = (point.x - cellDimensions.y / 4 / Mathf.Sqrt(3)) / hexSize * Mathf.Sqrt(3);

			int ti = Mathf.FloorToInt(x - y);
			int tj = Mathf.FloorToInt(x + y);
			int tk = Mathf.FloorToInt(-2 * y);

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
				index = 1;
			}
			else
			{
				index = 2;
			}

			return new FlatRhombPoint(Mathf.FloorToInt(hj), Mathf.FloorToInt(hi), index);
		}
	}
}