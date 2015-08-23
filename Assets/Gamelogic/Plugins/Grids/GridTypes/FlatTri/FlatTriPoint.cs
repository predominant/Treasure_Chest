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
		A struct that represents a point of a FlatTriGrid.
		
		@immutable	
		
		@version1_0

		@ingroup Points
	*/
	public partial struct FlatTriPoint :
		ISupportsVertices<PointyHexPoint>,
		ISupportsEdges<PointyRhombPoint>,
		IVertex<PointyHexPoint>
	{
		#region Constants
		public const int SpliceCount = 2;



		public static readonly IEnumerable<FlatTriPoint>[] MainDirections =
		{
			new PointList<FlatTriPoint>
			{
				new FlatTriPoint(0, 0, 1),
				new FlatTriPoint(-1, 0, 1),
				new FlatTriPoint(0, -1, 1),
			},

			new PointList<FlatTriPoint>
			{
				new FlatTriPoint(1, 0, 1),
				new FlatTriPoint(0, 1, 1),
				new FlatTriPoint(0, 0, 1),
			},
		};

		

		//I want it private, but the other part of the class needs it
		public readonly static IEnumerable<PointyHexPoint>[] VertexDirections =
		{
			new PointList<PointyHexPoint>
			{
				new PointyHexPoint(0, 1), //N
				new PointyHexPoint(0, 0), //SW
				new PointyHexPoint(1, 0), //SE
			},

			new PointList<PointyHexPoint>
			{
				new PointyHexPoint(1, 1), //NE
				new PointyHexPoint(0, 1), //NW
				new PointyHexPoint(1, 0), //S
			},
		};

		public readonly static PointList<PointyRhombPoint>[] EdgeDirections =
		{
			new PointList<PointyRhombPoint>
			{
				new PointyRhombPoint(0, 1, 0), //NE
				new PointyRhombPoint(-1, 1, 2), //NW
				new PointyRhombPoint(0, 0, 1), //S
			},

			new PointList<PointyRhombPoint>
			{
				new PointyRhombPoint(0, 1, 0), //SW
				new PointyRhombPoint(0, 1, 2), //SE
				new PointyRhombPoint(0, 1, 1), //N
			}
		};

		public readonly static PointList<PointyHexPoint>[] VertexFaceDirections =
		{
			new PointList<PointyHexPoint>
			{
				new PointyHexPoint(0, -1),
				new PointyHexPoint(-1, 0),
				new PointyHexPoint(-1, -1),
			},
			
			new PointList<PointyHexPoint>
			{
				new PointyHexPoint(0, 0),
				new PointyHexPoint(0, -1),
				new PointyHexPoint(-1, 0),
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
		#endregion

		#region Magnitude
		public int DistanceFrom(FlatTriPoint other)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Colorings
		public int GetColor4()
		{
			return basePoint.GetColor2_4() + 4 * I;
		}

		public int GetColor2()
		{
			return I;
		}
		#endregion

		#region Vertices and Edges
		public PointyHexPoint PointFromVertexAnchor()
		{
			return new PointyHexPoint(X, Y);
		}

		public PointyHexPoint GetVertexAnchor()
		{
			return new PointyHexPoint(X, Y);
		}

		public IEnumerable<PointyHexPoint> GetVertices()
		{
			var vertexAnchor = GetVertexAnchor();

			return
				from vertexDirection in VertexDirections[I]
				select vertexAnchor.MoveBy(vertexDirection);
		}

		public IEnumerable<PointyRhombPoint> GetEdges()
		{
			var edgeAnchor = GetEdgeAnchor();

			return
				from edgeDirection in EdgeDirections[I]
				select edgeAnchor.MoveBy(edgeDirection);

		}

		public PointyRhombPoint GetEdgeAnchor()
		{
			return new PointyRhombPoint(X, Y, 0);
		}

		public PointyHexPoint GetVertexFaceAnchor()
		{
			return GetVertexAnchor();
		}

		public IEnumerable<PointyHexPoint> GetVertexFaces()
		{
			var vertexFaceAnchor = GetVertexFaceAnchor();
			return VertexFaceDirections[I].Select(x => x + vertexFaceAnchor);
		}
		#endregion
	}
}