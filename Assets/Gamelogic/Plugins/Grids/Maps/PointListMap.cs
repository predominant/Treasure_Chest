//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2014 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		A one-way map map that maps a sequence of LinePoints to an arbitrary list of world points.

		@version1_8
	*/
	public class PointListMap : IGridToWorldMap<LinePoint>
	{
		private readonly Vector2[] pointList;

		public PointListMap(IEnumerable<Vector2> pointList)
		{
			this.pointList = pointList.ToArray();
		}
		public Vector2 this[LinePoint point]
		{
			get
			{
				return pointList[point];
			}
		}
	}

}