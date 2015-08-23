//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//


using System.Collections.Generic;
using System.Linq;

namespace Gamelogic.Grids
{
	public partial struct DiamondPoint : 
		ISupportsVertices<DiamondPoint>,
		ISupportsEdges<RectPoint>,
		IVertex<DiamondPoint>,
		IEdge<RectPoint>
	{
		#region Constants 
		public static readonly IEnumerable<DiamondPoint> VertexDirections = new PointList<DiamondPoint>
		{
			new DiamondPoint(1, 0), //E
			new DiamondPoint(1, 1), //N
			new DiamondPoint(0, 1), //W
			new DiamondPoint(0, 0), //S
		};

		public static readonly IEnumerable<RectPoint> EdgeDirections = new PointList<RectPoint>
		{
			new RectPoint(1, 1), //NE
			new RectPoint(0, 1), //NW
			new RectPoint(0, 0), //SW
			new RectPoint(1, 0), //SE
		};

		public static readonly List<IEnumerable<RectPoint>> EdgeFaceDirections = new List<IEnumerable<RectPoint>>
		{
			new PointList<RectPoint> //SW
			{
				new RectPoint(0, 0),
				new RectPoint(-1, 0),
			},

			new PointList<RectPoint> //NW
			{
				new RectPoint(0, 0),
				new RectPoint(0, 1)
			},	
		};

		public static readonly IEnumerable<DiamondPoint> VertexFaceDirections = new PointList<DiamondPoint>
		{
			new DiamondPoint(0, 0), //NE
			new DiamondPoint(-1, 0), //NW
			new DiamondPoint(-1, -1), //SW
			new DiamondPoint(0, -1), //SE
			
		};
		#endregion
		
		#region Vertices and Edges
		public DiamondPoint PointFromVertexAnchor()
		{
			return this;
		}

		public DiamondPoint GetVertexAnchor()
		{
			return this;
		}

		public RectPoint GetEdgeAnchor()
		{
			return new RectPoint(X - Y, X + Y);
		}

		public IEnumerable<DiamondPoint> GetVertices()
		{
			var vertexAnchor = GetVertexAnchor();

			return from vertexDirection in VertexDirections
				   select vertexAnchor + vertexDirection;
		}

		public IEnumerable<RectPoint> GetEdges()
		{
			RectPoint edgeAnchor = GetEdgeAnchor();

			return from edgeDirection in EdgeDirections
				   select edgeAnchor + edgeDirection;
		}

		private RectPoint GetEdgeFaceAnchor()
		{
			/*
			0 0 -> 0 0 
			0 1 -> 0 0
			1 0 -> 0 -1 
			1 1 -> 1 0
			*/
			return new RectPoint(
				GLMathf.Div(X + Y, 2),
				GLMathf.Div(Y - X, 2));
		}

		public IEnumerable<RectPoint> GetEdgeFaces()
		{
			var edgeAnchor = GetEdgeFaceAnchor();

			int edgeIndex = GLMathf.Mod(X + Y, 2);

			return from faceDirection in EdgeFaceDirections[edgeIndex]
				   select faceDirection + edgeAnchor;
		}

		public IEnumerable<DiamondPoint> GetVertexFaces()
		{
			var thisCopy = this;
			return VertexFaceDirections.Select(p => thisCopy + p);
		}
		#endregion
	}

	public partial class DiamondGrid<TCell>
	{
		#region Vertices and Edges
		/**
			Makes a vertex grid for this grid.
		*/
		public IGrid<TNewCell, DiamondPoint> MakeVertexGrid<TNewCell>()
		{
			var vertices = this.SelectMany(x => x.GetVertices());
			var storage = DiamondGrid<TNewCell>.CalculateStorage(vertices);
			var offset = DiamondGrid<TNewCell>.GridPointFromArrayPoint(storage.offset);

			return new DiamondGrid<TNewCell>(storage.dimensions.X, storage.dimensions.Y, x => IsInsideVertexGrid(x + offset), offset);
		}

		/**
			Makes an edge grid for this grid.
		*/
		public IGrid<TNewCell, RectPoint> MakeEdgeGrid<TNewCell>()
		{
			var edges = this.SelectMany(x => x.GetEdges());
			var storage = RectGrid<TNewCell>.CalculateStorage(edges);
			var offset = RectGrid<TNewCell>.GridPointFromArrayPoint(storage.offset);

			return new RectGrid<TNewCell>(storage.dimensions.X, storage.dimensions.Y, x => IsInsideEdgeGrid(x + offset), offset);
		}
		#endregion
	}
}