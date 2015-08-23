//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System.Linq;
using System.Collections.Generic;

namespace Gamelogic.Grids.Examples
{
	public class PipesCell : SpriteCell
	{
		private List<int> edgeData;

		public List<int> EdgeData
		{
			get
			{
				return edgeData;
			}

			set
			{
				edgeData = value;
			}
		}

		public void RotateCW()
		{
			var newEdgeData = edgeData.ButFirst().ToList();

			newEdgeData.Add(edgeData.First());
			edgeData = newEdgeData;
			AddAngle(-60);
		}

		public void RotateCCW()
		{
			List<int> newEdgeData = edgeData.ButLast().ToList();

			newEdgeData.Insert(0, edgeData.Last());
			edgeData = newEdgeData;
			AddAngle(60);
		}
	}
}