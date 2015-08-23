//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System.Collections.Generic;

namespace Gamelogic.Grids
{
	/**
		Used to indicate hat grids of this point type can support vertex grids.

		
		
		@version1_1

		Replaces ISupportsVertexGrid that is now aplied to grids and not points.

		@ingroup Interface
	*/
	public interface ISupportsVertices<TVertexPoint>
	{
		/**
			Returns the vertices of the point in the dual grid.
		*/
		IEnumerable<TVertexPoint> GetVertices();
	}
}
