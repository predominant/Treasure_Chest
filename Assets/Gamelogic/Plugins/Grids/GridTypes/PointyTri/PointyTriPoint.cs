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
		A struct that represents a point of a PointyTriGrid.
		
		@immutable
		@version1_0
		@ingroup Points
	*/
	public partial struct PointyTriPoint :
		ISupportsVertices<FlatHexPoint>,
		ISupportsEdges<FlatRhombPoint>,
		IVertex<FlatHexPoint>
	{
		#region Constants
		public const int SpliceCount = 2;

		public static readonly IEnumerable<PointyTriPoint>[] MainDirections =
		{
			new PointList<PointyTriPoint>
			{
				new PointyTriPoint(0, 0, 1),
				new PointyTriPoint(0, 1, 1),
				new PointyTriPoint(-1, 0, 1),
			},

			new PointList<PointyTriPoint>
			{
				new PointyTriPoint(1, 0, 1),
				new PointyTriPoint(0, 1, 1),
				new PointyTriPoint(0, 0, 1),
			}
		};

		//I want it private, but the other part of the class needs it
		public readonly static IEnumerable<FlatHexPoint>[] VertexDirections =
		{
			new PointList<FlatHexPoint>
			{
				new FlatHexPoint(1, 0), //E
				new FlatHexPoint(0, 1), //NW
				new FlatHexPoint(0, 0), //SW
			},

			new PointList<FlatHexPoint>
			{
				new FlatHexPoint(1, 1), //NE
				new FlatHexPoint(0, 1), //W
				new FlatHexPoint(1, 0), //SE
			},
		};

		public readonly static PointList<FlatRhombPoint>[] EdgeDirections =
		{
			new PointList<FlatRhombPoint>
			{
				new FlatRhombPoint(0, 1, 0), //NE
				new FlatRhombPoint(-1, 1, 2), //NW
				new FlatRhombPoint(0, 0, 1), //S
			},

			new PointList<FlatRhombPoint>
			{
				new FlatRhombPoint(0, 1, 0), //SW
				new FlatRhombPoint(0, 1, 2), //SE
				new FlatRhombPoint(0, 1, 1), //N
			}
		};

		public readonly static PointList<FlatHexPoint>[] VertexFaceDirections =
		{
			new PointList<FlatHexPoint>
			{
				new FlatHexPoint(0, -1),
				new FlatHexPoint(-1, 0),
				new FlatHexPoint(-1, -1),
			},
			
			new PointList<FlatHexPoint>
			{
				new FlatHexPoint(0, 0),
				new FlatHexPoint(0, -1),
				new FlatHexPoint(-1, 0),
			},
		};

		#endregion

		#region Properties
		/**
			This is a redundant coordinate that is useful for certain algorithms and calculation.
			The coordinates of a triangle satsify this identity: X + Y + Z + I = 0.
		*/
		public int Z
		{
			get
			{
				return (BasePoint.Z - I);
			}
		}

		public PointyHexPoint AsHexPoint
		{
			get
			{
				var x = 3*X + I + 1;
				var y = 3*Y + I + 1;

				return new PointyHexPoint(x, y);
			}
		}
		#endregion

		#region Magnitude
		public int DistanceFrom(PointyTriPoint other)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Colorings
		public int GetColor2()
		{
			return I;
		}

		public int GetColor4()
		{
			return basePoint.GetColor2_4() + 4 * I;
		}
		#endregion

		#region Vertices and Edges
		public FlatHexPoint PointFromVertexAnchor()
		{
			return new FlatHexPoint(X, Y);
		}

		public FlatHexPoint GetVertexAnchor()
		{
			return new FlatHexPoint(X, Y);
		}

		public IEnumerable<FlatHexPoint> GetVertices()
		{
			var vertexAnchor = GetVertexAnchor();

			return
				from vertexDirection in VertexDirections[I]
				select vertexAnchor.MoveBy(vertexDirection);
		}

		public IEnumerable<FlatRhombPoint> GetEdges()
		{
			var edgeAnchor = GetEdgeAnchor();

			return
				from edgeDirection in EdgeDirections[I]
				select edgeAnchor.MoveBy(edgeDirection);
		}

		public FlatRhombPoint GetEdgeAnchor()
		{
			return new FlatRhombPoint(X, Y, 0);
		}

		public FlatHexPoint GetVertexFaceAnchor()
		{
			return GetVertexAnchor();
		}

		public IEnumerable<FlatHexPoint> GetVertexFaces()
		{
			var vertexFaceAnchor = GetVertexFaceAnchor();
			return VertexFaceDirections[I].Select(point => point + vertexFaceAnchor);
		}
		#endregion
	}
}