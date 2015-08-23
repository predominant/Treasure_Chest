//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using System;
using System.Linq;

namespace Gamelogic.Grids
{
	/**
		A grid of triangles in the pointy orientation. All triangles have one vertical edge.
	
		
		
		@version1_0

		@ingroup Grids
	*/
	[Serializable]
	public partial class PointyTriGrid<TCell> : AbstractSplicedGrid<TCell, PointyTriPoint, FlatHexPoint>
	{
		#region Implementation

		protected override PointyTriPoint MakePoint(FlatHexPoint basePoint, int index)
		{
			return new PointyTriPoint(basePoint.X, basePoint.Y, index);
		}

		protected override IGrid<TCell[], FlatHexPoint> MakeUnderlyingGrid(int width, int height)
		{
			return new FlatHexGrid<TCell[]>(width, height);
		}
		#endregion

		#region StorageInfo
		public static FlatHexPoint GridPointFromArrayPoint(ArrayPoint point)
		{
			return FlatHexGrid<TCell>.GridPointFromArrayPoint(point);
		}

		public static ArrayPoint ArrayPointFromGridPoint(FlatHexPoint point)
		{
			return FlatHexGrid<TCell>.ArrayPointFromGridPoint(point);
		}
		#endregion

		#region Vertices and Edges
		public IGrid<TNewCell, FlatHexPoint> MakeVertexGrid<TNewCell>()
		{
			var vertices = this.SelectMany(x => x.GetVertices());
			var storage = FlatHexGrid<TNewCell>.CalculateStorage(vertices);
			var offset = FlatHexGrid<TNewCell>.GridPointFromArrayPoint(storage.offset);

			return new FlatHexGrid<TNewCell>(storage.dimensions.X, storage.dimensions.Y, x => IsInsideVertexGrid(x + offset), offset);
		}

		/**
			Makes an edge grid for this grid.
		*/
		public IGrid<TNewCell, FlatRhombPoint> MakeEdgeGrid<TNewCell>()
		{
			var edgeOffset = GridOrigin;

			var edges = this.SelectMany(x => x.GetEdges());
			var storage = FlatRhombGrid<TNewCell>.CalculateStorage(edges);
			var offset = new FlatHexPoint(-2, 0);
			return new FlatRhombGrid<TNewCell>(storage.dimensions.X + 4, storage.dimensions.Y + 4, x => IsInsideEdgeGrid(x + offset), edgeOffset.GetEdgeAnchor().BasePoint + offset);
		}
		#endregion
	}
}