//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//


using System.Collections.Generic;
using System.Linq;

namespace Gamelogic.Grids
{
	public partial struct FlatHexPoint :
		IVertex<PointyTriPoint>
	{
		public static readonly IEnumerable<FlatRhombPoint> EdgeDirections = new PointList<FlatRhombPoint>
		{
			new FlatRhombPoint(1, 0, 1), //NE
			new FlatRhombPoint(0, 1, 2), //N
			new FlatRhombPoint(0, 1, 0), //NW

			new FlatRhombPoint(0, 0, 1), //SW
			new FlatRhombPoint(0, 0, 2), //S
			new FlatRhombPoint(1, 0, 0), //SE
		};

		public static readonly IEnumerable<PointyTriPoint> VertexDirections = new PointList<PointyTriPoint>
		{
			new PointyTriPoint(1, 0, 1), //E
			new PointyTriPoint(1, 1, 0), //NE
			new PointyTriPoint(0, 1, 1), //NW

			new PointyTriPoint(0, 1, 0), //W
			new PointyTriPoint(0, 0, 1), //SW
			new PointyTriPoint(1, 0, 0), //SE
		};

		public static readonly IEnumerable<PointyTriPoint> VertexFaceDirections = new PointList<PointyTriPoint>
		{
			new PointyTriPoint(0, 0, 0),
			new PointyTriPoint(0, -1, 1),
			new PointyTriPoint(0, -1, 0),
			new PointyTriPoint(-1, -1, 1),
			new PointyTriPoint(-1, 0, 0),
			new PointyTriPoint(-1, 0, 1),
		};

		#region Vertices and Edges
		public PointyTriPoint PointFromVertexAnchor()
		{
			return new PointyTriPoint(X, Y, 0);
		}

		public PointyTriPoint GetVertexAnchor()
		{
			return new PointyTriPoint(X, Y, 0);
		}

		public IEnumerable<PointyTriPoint> GetVertices()
		{
			var vertexAnchor = GetVertexAnchor();

			return
				from vertexDirections in VertexDirections
				select vertexAnchor.MoveBy(vertexDirections);
		}

		public IEnumerable<FlatRhombPoint> GetEdges()
		{
			var edgeAnchor = GetEdgeAnchor();

			return from edgeDirection in EdgeDirections
				   select edgeAnchor.MoveBy(edgeDirection);
		}

		public FlatRhombPoint GetEdgeAnchor()
		{
			return new FlatRhombPoint(X, Y, 0);
		}

		public PointyTriPoint GetVertexFaceAnchor()
		{
			return GetVertexAnchor();
		}

		public IEnumerable<PointyTriPoint> GetVertexFaces()
		{
			var vertexFaceAnchor = GetVertexFaceAnchor();
			return VertexFaceDirections.Select(point => point + vertexFaceAnchor.BasePoint);
		}
		#endregion
	}

	public partial class FlatHexGrid<TCell>
	{
		#region Vertices and Edges
		public IGrid<TNewCell, PointyTriPoint> MakeVertexGrid<TNewCell>()
		{
			var vertices = this.SelectMany(x => x.GetVertices());
			var storage = PointyTriGrid<TNewCell>.CalculateStorage(vertices);
			var offset = PointyTriGrid<TNewCell>.GridPointFromArrayPoint(storage.offset);
			
			return new PointyTriGrid<TNewCell>(storage.dimensions.X, storage.dimensions.Y, x => IsInsideVertexGrid(x + offset), offset);
		}

		/**
			Makes an edge grid for this grid.
		*/
		public IGrid<TNewCell, FlatRhombPoint> MakeEdgeGrid<TNewCell>()
		{
			var edges = this.SelectMany(x => x.GetEdges());
			var storage = FlatRhombGrid<TNewCell>.CalculateStorage(edges);
			var offset = FlatRhombGrid<TNewCell>.GridPointFromArrayPoint(storage.offset);

			var oddPoint = new FlatHexPoint(
				GLMathf.Mod(offset.X, 2),
				GLMathf.Mod(offset.Y, 2));

			var evenPoint = offset.Subtract(oddPoint);

			return new FlatRhombGrid<TNewCell>(storage.dimensions.X + 2, storage.dimensions.Y + 2, IsInsideEdgeGrid, evenPoint);
		}
		#endregion
	}
}