//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System.Collections.Generic;

namespace Gamelogic.Grids
{
	/**
		Use this class in constructors of HashSets and Dictionaries that 
		take point types (such as PointyHexPoint) as keys.
	*/
	public class PointComparer<TPoint> : IEqualityComparer<TPoint> 
		where TPoint : IGridPoint<TPoint>
	{
		public bool Equals(TPoint p1, TPoint p2)
		{
			return p1.Equals(p2);
		}

		public int GetHashCode(TPoint p)
		{
			return p.GetHashCode();
		}
	}
}