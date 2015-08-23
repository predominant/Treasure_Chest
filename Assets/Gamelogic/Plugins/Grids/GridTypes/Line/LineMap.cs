//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2014 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		The default map for a LineGrid, that maps points in a straight horizontal line.

		@version1_8

		@ingroup Maps
	*/
	public class LineMap : AbstractMap<LinePoint>
	{
		public LineMap(Vector2 cellDimensions) : base(cellDimensions)
		{
		}

		public LineMap(Vector2 cellDimensions, Vector2 anchorTranslation) : base(cellDimensions, anchorTranslation)
		{
		}

		public override LinePoint RawWorldToGrid(Vector2 worldPoint)
		{
			var point = (worldPoint + cellDimensions/2);

			var m = GLMathf.FloorToInt(point.x/cellDimensions.x);

			return m;
		}

		public override Vector2 GridToWorld(LinePoint gridPoint)
		{
			var x = cellDimensions.x*gridPoint - cellDimensions.x/2;
			return new Vector2(x, 0);
		}
	}
}