//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using System.Collections.Generic;
using System.Linq;

namespace Gamelogic.Grids
{
	public partial class RectGrid<TCell>
	{
		#region Vertices and Edges
		public IGrid<TNewCell, RectPoint> MakeVertexGrid<TNewCell>()
		{
			var vertices = this.SelectMany(x => x.GetVertices());
			var storage = RectGrid<TNewCell>.CalculateStorage(vertices);
			var offset = RectGrid<TNewCell>.GridPointFromArrayPoint(storage.offset);

			return new RectGrid<TNewCell>(storage.dimensions.X, storage.dimensions.Y, x => IsInsideVertexGrid(x + offset), offset);
		}

		/**
			Makes an edge grid for this grid.
		*/
		public IGrid<TNewCell, DiamondPoint> MakeEdgeGrid<TNewCell>()
		{
			var edges = this.SelectMany(x => x.GetEdges());
			var storage = DiamondGrid<TNewCell>.CalculateStorage(edges);
			var offset = DiamondGrid<TNewCell>.GridPointFromArrayPoint(storage.offset);

			return new DiamondGrid<TNewCell>(storage.dimensions.X, storage.dimensions.Y, x => IsInsideEdgeGrid(x + offset), offset);
		}
		#endregion
	}

	public partial struct RectPoint:
		ISupportsVertices<RectPoint>,
		ISupportsEdges<DiamondPoint>,
		IVertex<RectPoint>,
		IEdge<DiamondPoint>
	{
		#region Constants
		public static readonly IEnumerable<RectPoint> VertexDirections = new PointList<RectPoint>
		{
			new RectPoint(1, 1), //NE
			new RectPoint(0, 1), //NW
			new RectPoint(0, 0), //SW
			new RectPoint(1, 0) //SE
		};

		public static readonly IEnumerable<DiamondPoint> EdgeDirections = new PointList<DiamondPoint>
		{
			new DiamondPoint(1, 0), //E
			new DiamondPoint(1, 1), //N
			new DiamondPoint(0, 1), //W
			new DiamondPoint(0, 0) //S
		};

		public static readonly IList<IEnumerable<DiamondPoint>> EdgeFaceDirections = new List<IEnumerable<DiamondPoint>>
		{
			new PointList<DiamondPoint> 
//SW
			{
				new DiamondPoint(0, 0),
				new DiamondPoint(-1, 0),
			},

			new PointList<DiamondPoint> 
//NW
			{
				new DiamondPoint(0, 0),
				new DiamondPoint(0, 1)
			},	
		};

		public static readonly IEnumerable<RectPoint> VertexFaceDirections = new PointList<RectPoint>
		{
			new RectPoint(0, 0), //NE
			new RectPoint(-1, 0), //NW
			new RectPoint(-1, -1), //SW
			new RectPoint(0, -1), //SE
		};
		#endregion

		#region Vertices and Edges
		public RectPoint GetVertexAnchor()
		{
			return this;
		}

		public DiamondPoint GetEdgeAnchor()
		{
			return new DiamondPoint(X - Y, X + Y);
		}

		private DiamondPoint GetEdgeFaceAnchor()
		{
			/*
			0 0 -> 0 0 
			0 1 -> 0 0
			1 0 -> 0 -1 
			1 1 -> 1 0
			*/
			return new DiamondPoint(
				GLMathf.Div(X + Y, 2),
				GLMathf.Div(Y - X, 2));
		}

		public IEnumerable<DiamondPoint> GetEdges()
		{
			DiamondPoint edgeAnchor = GetEdgeAnchor();

			return
				from edgeDirection in EdgeDirections
				select edgeDirection + edgeAnchor;
		}

		public IEnumerable<RectPoint> GetVertices()
		{
			var vertexAnchor = GetVertexAnchor();

			return
				from vertexDirection in VertexDirections
				select vertexAnchor + vertexDirection;
		}

		public IEnumerable<DiamondPoint> GetEdgeFaces()
		{
			var edgeAnchor = GetEdgeFaceAnchor();

			int edgeIndex = GLMathf.Mod(X + Y, 2);

			return from faceDirection in EdgeFaceDirections[edgeIndex]
				   select faceDirection + edgeAnchor;

		}

		public IEnumerable<RectPoint> GetVertexFaces()
		{
			var thisCopy = this;
			return VertexFaceDirections.Select(point => thisCopy + point);
		}
		#endregion
	}
}