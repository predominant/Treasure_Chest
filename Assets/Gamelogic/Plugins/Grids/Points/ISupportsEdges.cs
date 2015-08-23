//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System.Collections.Generic;

namespace Gamelogic.Grids
{
	/**
		Used to indicated that grids of this point type can support edge grids.

		
		
		@version1_1

		Replaces ISupportsEdgeGrid that is now applied to grids, not 

		@ingroup Interface
	*/
	public interface ISupportsEdges<TVertexPoint>
	{
		IEnumerable<TVertexPoint> GetEdges();
	}
}
