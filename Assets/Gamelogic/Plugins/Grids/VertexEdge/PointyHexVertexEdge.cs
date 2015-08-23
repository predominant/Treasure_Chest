//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using System.Collections.Generic;
using System.Linq;

namespace Gamelogic.Grids
{
	public partial struct PointyHexPoint :
		IVertex<FlatTriPoint>
	{
		public static readonly IEnumerable<PointyRhombPoint> EdgeDirections = new PointList<PointyRhombPoint>
		{
			new PointyRhombPoint(1, 0, 1), //E
			new PointyRhombPoint(0, 1, 2), //NE
			new PointyRhombPoint(0, 1, 0), //NW
		
			new PointyRhombPoint(0, 0, 1), //W
			new PointyRhombPoint(0, 0, 2), //SW 
			new PointyRhombPoint(1, 0, 0), //SE
		};

		public static readonly IEnumerable<FlatTriPoint> VertexDirections = new PointList<FlatTriPoint>
		{
			new FlatTriPoint(1, 1, 0), //NE
			new FlatTriPoint(0, 1, 1), //N
			new FlatTriPoint(0, 1, 0), //NW

			new FlatTriPoint(0, 0, 1), //SW
			new FlatTriPoint(1, 0, 0), //S
			new FlatTriPoint(1, 0, 1), //SW
		};

		public static readonly IEnumerable<FlatTriPoint> VertexFaceDirections = new PointList<FlatTriPoint>
		{
			new FlatTriPoint(0, 0, 0),
			new FlatTriPoint(0, -1, 1),
			new FlatTriPoint(0, -1, 0),
			new FlatTriPoint(-1, -1, 1),
			new FlatTriPoint(-1, 0, 0),
			new FlatTriPoint(-1, 0, 1),
		};

		#region Vertice and Edges
		public FlatTriPoint GetVertexAnchor()
		{
			return new FlatTriPoint(X, Y, 0);
		}

		public FlatTriPoint PointFromVertexAnchor()
		{
			return new FlatTriPoint(X, Y, 0);
		}

		public IEnumerable<FlatTriPoint> GetVertices()
		{
			FlatTriPoint vertexAnchor = GetVertexAnchor();

			return
				from vertexDirection in VertexDirections
				select vertexAnchor.MoveBy(vertexDirection);
		}

		public IEnumerable<PointyRhombPoint> GetEdges()
		{
			PointyRhombPoint edgeAnchor = GetEdgeAnchor();

			return
				from edgeDirection in EdgeDirections
				select edgeAnchor.MoveBy(edgeDirection);
		}

		public FlatTriPoint GetVertexFaceAnchor()
		{
			return GetVertexAnchor();
		}

		public IEnumerable<FlatTriPoint> GetVertexFaces()
		{
			var vertexFaceAnchor = GetVertexFaceAnchor();
			return VertexFaceDirections.Select(p => p + vertexFaceAnchor.BasePoint);
		}

		public PointyRhombPoint GetEdgeAnchor()
		{
			return new PointyRhombPoint(X, Y, 0);
		}
		#endregion
	}

	public partial class PointyHexGrid<TCell>
	{
		#region Vertices and Edges
		public IGrid<TNewCell, FlatTriPoint> MakeVertexGrid<TNewCell>()
		{
			var vertices = this.SelectMany(x => x.GetVertices());
			var storage = FlatTriGrid<TNewCell>.CalculateStorage(vertices);
			var offset = FlatTriGrid<TNewCell>.GridPointFromArrayPoint(storage.offset);

			return new FlatTriGrid<TNewCell>(storage.dimensions.X, storage.dimensions.Y, x => IsInsideVertexGrid(x + offset), offset);
		}

		/**
			Makes an edge grid for this grid.
		*/
		public IGrid<TNewCell, PointyRhombPoint> MakeEdgeGrid<TNewCell>()
		{
			var edges = this.SelectMany(x => x.GetEdges());
			var storage = PointyRhombGrid<TNewCell>.CalculateStorage(edges);
			var offset = PointyRhombGrid<TNewCell>.GridPointFromArrayPoint(storage.offset);

			return new PointyRhombGrid<TNewCell>(storage.dimensions.X + 2, storage.dimensions.Y + 2, x => IsInsideEdgeGrid(x + offset), offset);

		}
		#endregion
	}
}