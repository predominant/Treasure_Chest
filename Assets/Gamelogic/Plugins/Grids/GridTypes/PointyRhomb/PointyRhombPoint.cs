//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using System;
using System.Linq;
using System.Collections.Generic;

namespace Gamelogic.Grids
{
	/**
		A struct that represents a point of a PointyRhombPoint.
			
		@immutable
	
		
		
		@version1_0

		@ingroup Points
	*/
	public partial struct PointyRhombPoint :
		IEdge<PointyHexPoint>,
		IEdge<FlatTriPoint>
	{
		#region Constants
		public const int SpliceCount = 3;

		public static readonly IEnumerable<PointyRhombPoint>[] MainDirections =
		{
			new PointList<PointyRhombPoint>
			{
				new PointyRhombPoint(0, 0, 2),
				new PointyRhombPoint(0, 0, 1),
				new PointyRhombPoint(-1, 0, 2),
				new PointyRhombPoint(1, -1, 1)
			},

			new PointList<PointyRhombPoint>
			{
				new PointyRhombPoint(0, 1, 2),
				new PointyRhombPoint(-1, 1, 1),
				new PointyRhombPoint(0, 0, 2),
				new PointyRhombPoint(0, 0, 1),
			},

			new PointList<PointyRhombPoint>
			{
				new PointyRhombPoint(1, 0, 1),
				new PointyRhombPoint(1, -1, 2),
				new PointyRhombPoint(0, 0, 1),
				new PointyRhombPoint(0, 0, 2),
			}
		};

		public static readonly IList<IEnumerable<PointyHexPoint>> HexEdgeFaceDirections = new List<IEnumerable<PointyHexPoint>>
		{
			new PointList<PointyHexPoint>
			{
				new PointyHexPoint(-1, 0),
				new PointyHexPoint(0, -1)
			},

			new PointList<PointyHexPoint>
			{
				new PointyHexPoint(0, 0),
				new PointyHexPoint(-1, 0)
			},

			new PointList<PointyHexPoint>
			{
				new PointyHexPoint(0, 0),
				new PointyHexPoint(0, -1)
			}
		};

		public static readonly List<IEnumerable<FlatTriPoint>> TriEdgeFaceDirections = new List<IEnumerable<FlatTriPoint>>
		{
			
			new PointList<FlatTriPoint>
			{
				new FlatTriPoint(0, -1, 0),
				new FlatTriPoint(0, -1, 1)
			},
			
			new PointList<FlatTriPoint>
			{
				new FlatTriPoint(0, 0, 0),
				new FlatTriPoint(0, -1, 1)
			},

			new PointList<FlatTriPoint>
			{
				new FlatTriPoint(1, -1, 0),
				new FlatTriPoint(0, -1, 1)
			},
		};
		#endregion

		#region Colorings
		public int GetColor12()
		{
			return basePoint.GetColor2_4() + 4 * I;
		}
		#endregion

		#region Magnitude
		public int DistanceFrom(PointyRhombPoint other)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Vertices and Edges
		[Experimental]
		IEnumerable<PointyHexPoint> IEdge<PointyHexPoint>.GetEdgeFaces()
		{
			var basePointCopy = BasePoint;
			return HexEdgeFaceDirections[I].Select(x => x + basePointCopy);
		}

		[Experimental]
		IEnumerable<FlatTriPoint> IEdge<FlatTriPoint>.GetEdgeFaces()
		{
			var basePointCopy = BasePoint;
			return TriEdgeFaceDirections[I].Select(x => x + basePointCopy);
		}
		#endregion

		
	}
}