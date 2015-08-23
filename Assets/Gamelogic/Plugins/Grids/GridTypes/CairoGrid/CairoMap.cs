//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System.Collections.Generic;
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Maps between world points and CiaroPoints.

		This example shows how to implement a map for a 
		grid with arbitrary polygons, using PolygonMap.
		
		@link_working_with_maps
		
		
		
		@version1_2
		
		@ingroup Maps
	*/
	[Experimental]
	public class CairoMap : PolygonGridMap<CairoPoint, PointyHexPoint>
	{
		private readonly float shapeWidth;
		private readonly float shapeHeight;

		private static readonly IEnumerable<IEnumerable<Vector2>> polies = new List<IEnumerable<Vector2>>
		{
			//0
			new List<Vector2>
			{
				new Vector2(0, 8),
				new Vector2(1.5f, 8),
				new Vector2(1, 7),
				new Vector2(0, 7),
			},

			//1
			new List<Vector2>
			{
				new Vector2(1.5f, 8),
				new Vector2(4, 8),
				new Vector2(4, 6),
				new Vector2(2, 5),
				new Vector2(1, 7),
			},

			//2
			new List<Vector2>
			{
				new Vector2(4, 8),
				new Vector2(6.5f, 8),
				new Vector2(7, 7),
				new Vector2(6, 5),
				new Vector2(4, 6),
			},

			//3
			new List<Vector2>
			{
				new Vector2(6.5f, 8),
				new Vector2(8, 8),
				new Vector2(8, 7),
				new Vector2(7, 7),
			},

			//4
			new List<Vector2>
			{
				new Vector2(0, 7),
				new Vector2(1, 7),
				new Vector2(2, 5),
				new Vector2(0, 4),
			},

			//5
			new List<Vector2>
			{
				new Vector2(7, 7),
				new Vector2(8, 7),
				new Vector2(8, 4),
				new Vector2(6, 5),
			},

			//6
			new List<Vector2>
			{
				new Vector2(2, 5),
				new Vector2(4, 6),
				new Vector2(6, 5),
				new Vector2(5, 3),
				new Vector2(3, 3),
			},

			//7
			new List<Vector2>
			{
				new Vector2(0, 4),
				new Vector2(2, 5),
				new Vector2(3, 3),
				new Vector2(2, 1),
				new Vector2(0, 2),
			},

			//8
			new List<Vector2>
			{
				new Vector2(5, 3),
				new Vector2(6, 5),
				new Vector2(8, 4),
				new Vector2(8, 2),
				new Vector2(6, 1),
			},

			//9
			new List<Vector2>
			{
				new Vector2(3, 3),
				new Vector2(5, 3),
				new Vector2(6, 1),
				new Vector2(4, 0),
				new Vector2(2, 1),
			},

			//10
			new List<Vector2>
			{
				new Vector2(0, 2),
				new Vector2(2, 1),
				new Vector2(1.5f, 0),
				new Vector2(0, 0),
			},

			//11
			new List<Vector2>
			{
				new Vector2(6, 1),
				new Vector2(8, 2),
				new Vector2(8, 0),
				new Vector2(6.5f, 0),
			},

			//12
			new List<Vector2>
			{
				new Vector2(2, 1),
				new Vector2(4, 0),
				new Vector2(1.5f, 0),
			},

			//13
			new List<Vector2>
			{
				new Vector2(4, 0),
				new Vector2(6, 1),
				new Vector2(6.5f, 0),
			},
		};

		private static readonly IEnumerable<CairoPoint> offsets = new PointList<CairoPoint>
		{
			new CairoPoint(-1, 1, 2),
			new CairoPoint(-1, 1, 3),
			new CairoPoint(0, 1, 1),
			new CairoPoint(0, 1, 2),
			new CairoPoint(-1, 1, 0),
			
			new CairoPoint(0, 1, 0),
			new CairoPoint(0, 0, 2),
			new CairoPoint(0, 0, 1),
			new CairoPoint(0, 0, 3),
			new CairoPoint(0, 0, 0),
			
			new CairoPoint(0, -1, 2),
			new CairoPoint(1, -1, 2),
			new CairoPoint(0, -1, 3),
			new CairoPoint(1, -1, 1),
		};

		private static readonly IEnumerable<Vector2> rectOffsets = new List<Vector2>
		{
			new Vector2(0, 0),
			new Vector2(-2.5f, 1.5f),
			new Vector2(0, 3),
			new Vector2(2.5f, 1.5f),
		};
		
		public CairoMap(Vector2 cellDimensions):
			base(
				cellDimensions, 
				2 * cellDimensions, 
				new Vector2(4, 1.5f) / 8,
				Scale(polies, 1f/8), 
				offsets, 
				Scale(rectOffsets, 1f/8), 
				MakeCairoPoint,
				CairoBasePointToNormalisedWorld,
				StraightRectPointToSkewRectPoint)
		{			
			shapeWidth = cellDimensions.x;
			shapeHeight = 3 * cellDimensions.y / 4.0f;
		}

		private static CairoPoint MakeCairoPoint(int x, int y, int index)
		{
			return new CairoPoint(x, y, index);
		}

		private static PointyHexPoint StraightRectPointToSkewRectPoint(VectorPoint straight)
		{
			int y = 2 * straight.Y;
			int x = straight.X - straight.Y;

			return new PointyHexPoint(x, y);
		}

		private static Vector2 CairoBasePointToNormalisedWorld(PointyHexPoint baseGridPoint)
		{
			float x = (baseGridPoint.X + baseGridPoint.Y / 2f); 
			float y = (baseGridPoint.Y / 2f);
	
			return new Vector2(x, y);
		}

		public override Vector2 GetCellDimensions(CairoPoint point)
		{
			switch (point.I)
			{
				case 0:
				case 2:
					return new Vector2(shapeWidth, shapeHeight);

				case 1:
				case 3:
					return new Vector2(shapeHeight, shapeWidth);
			}

			return Vector2.zero;
		}
	}
}