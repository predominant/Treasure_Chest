//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//
using System.Collections.Generic;

namespace Gamelogic.Grids
{
	/**
		This point type is the type of vertex anchor point of the type parmater.
		The vertex anchor is the point from which all the vertices are calculated.

		For example, if TriPoint implements ISupportsVertexGrid<HexPoint>, then
		HexPoint will implement IVertex<TriPoint>.

		
		
		@version1_1 

		(This class replaces IVertexAnchor).

		@ingroup Interface
	*/
	public interface IVertex<TPoint>
	{
		//TPoint PointFromVertexAnchor();

		/**
			Get the coordinates of the faces that corresponds to this point treated as a vertex.
		*/
		IEnumerable<TPoint> GetVertexFaces();
	}

	
}