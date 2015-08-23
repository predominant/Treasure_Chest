//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

// Auto-generated File

using System.Linq;

namespace Gamelogic.Grids
{
	public partial class RectGrid<TCell> :
		ISupportsVertexGrid<RectPoint>, 
		ISupportsEdgeGrid<DiamondPoint>
	{
		/**
			A test function that returns true if the point for which the given 
			vertexPoint is a vertex, is inside this grid.
		*/
		private bool IsInsideVertexGrid(RectPoint vertexPoint)
		{
			var faces = (vertexPoint as IVertex<RectPoint>).GetVertexFaces();
			return faces.Any(Contains);
		}

		/**
			A test function that returns true if the point for which the given 
			vertexPoint is a vertex, is inside this grid.
		*/
		private bool IsInsideEdgeGrid(DiamondPoint edgePoint)
		{		
			var faces = (edgePoint as IEdge<RectPoint>).GetEdgeFaces();
			return faces.Any(Contains);
		}
	}
	
	public partial class DiamondGrid<TCell> :
		ISupportsVertexGrid<DiamondPoint>, 
		ISupportsEdgeGrid<RectPoint>
	{
		/**
			A test function that returns true if the point for which the given 
			vertexPoint is a vertex, is inside this grid.
		*/
		private bool IsInsideVertexGrid(DiamondPoint vertexPoint)
		{
			var faces = (vertexPoint as IVertex<DiamondPoint>).GetVertexFaces();
			return faces.Any(Contains);
		}

		/**
			A test function that returns true if the point for which the given 
			vertexPoint is a vertex, is inside this grid.
		*/
		private bool IsInsideEdgeGrid(RectPoint edgePoint)
		{		
			var faces = (edgePoint as IEdge<DiamondPoint>).GetEdgeFaces();
			return faces.Any(Contains);
		}
	}
	
	public partial class PointyHexGrid<TCell> :
		ISupportsVertexGrid<FlatTriPoint>, 
		ISupportsEdgeGrid<PointyRhombPoint>
	{
		/**
			A test function that returns true if the point for which the given 
			vertexPoint is a vertex, is inside this grid.
		*/
		private bool IsInsideVertexGrid(FlatTriPoint vertexPoint)
		{
			var faces = (vertexPoint as IVertex<PointyHexPoint>).GetVertexFaces();
			return faces.Any(Contains);
		}

		/**
			A test function that returns true if the point for which the given 
			vertexPoint is a vertex, is inside this grid.
		*/
		private bool IsInsideEdgeGrid(PointyRhombPoint edgePoint)
		{		
			var faces = (edgePoint as IEdge<PointyHexPoint>).GetEdgeFaces();
			return faces.Any(Contains);
		}
	}
	
	public partial class FlatHexGrid<TCell> :
		ISupportsVertexGrid<PointyTriPoint>, 
		ISupportsEdgeGrid<FlatRhombPoint>
	{
		/**
			A test function that returns true if the point for which the given 
			vertexPoint is a vertex, is inside this grid.
		*/
		private bool IsInsideVertexGrid(PointyTriPoint vertexPoint)
		{
			var faces = (vertexPoint as IVertex<FlatHexPoint>).GetVertexFaces();
			return faces.Any(Contains);
		}

		/**
			A test function that returns true if the point for which the given 
			vertexPoint is a vertex, is inside this grid.
		*/
		private bool IsInsideEdgeGrid(FlatRhombPoint edgePoint)
		{		
			var faces = (edgePoint as IEdge<FlatHexPoint>).GetEdgeFaces();
			return faces.Any(Contains);
		}
	}
	
	public partial class FlatTriGrid<TCell> :
		ISupportsVertexGrid<PointyHexPoint>, 
		ISupportsEdgeGrid<PointyRhombPoint>
	{
		/**
			A test function that returns true if the point for which the given 
			vertexPoint is a vertex, is inside this grid.
		*/
		private bool IsInsideVertexGrid(PointyHexPoint vertexPoint)
		{
			var faces = (vertexPoint as IVertex<FlatTriPoint>).GetVertexFaces();
			return faces.Any(Contains);
		}

		/**
			A test function that returns true if the point for which the given 
			vertexPoint is a vertex, is inside this grid.
		*/
		private bool IsInsideEdgeGrid(PointyRhombPoint edgePoint)
		{		
			var faces = (edgePoint as IEdge<FlatTriPoint>).GetEdgeFaces();
			return faces.Any(Contains);
		}
	}
	
	public partial class PointyTriGrid<TCell> :
		ISupportsVertexGrid<FlatHexPoint>, 
		ISupportsEdgeGrid<FlatRhombPoint>
	{
		/**
			A test function that returns true if the point for which the given 
			vertexPoint is a vertex, is inside this grid.
		*/
		private bool IsInsideVertexGrid(FlatHexPoint vertexPoint)
		{
			var faces = (vertexPoint as IVertex<PointyTriPoint>).GetVertexFaces();
			return faces.Any(Contains);
		}

		/**
			A test function that returns true if the point for which the given 
			vertexPoint is a vertex, is inside this grid.
		*/
		private bool IsInsideEdgeGrid(FlatRhombPoint edgePoint)
		{		
			var faces = (edgePoint as IEdge<PointyTriPoint>).GetEdgeFaces();
			return faces.Any(Contains);
		}
	}
	
}
