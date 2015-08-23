//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2014 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System.Linq;
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		A map that maps points of a LineGrid in 
		an archimedean spiral.
	
		@version1_8

		@ingroup Maps
	*/
	public class ArchimedeanSpiralMap : AbstractMap<LinePoint>
	{
		private const int Offset = 20;
		private const float Radius = 7; 
		private const float SampleFrequency = 2 / Radius;
		
		private readonly Vector2[] lookUpWorld;
		private readonly LinePoint[] lookUpGrid;
		private readonly LinePoint notInGrid;

		public ArchimedeanSpiralMap(Vector2 cellDimensions, IGridSpace<LinePoint> grid)
			: base(cellDimensions)
		{
			lookUpWorld = grid.Select(p => this[p]).ToArray();
			lookUpGrid = grid.ToArray();
			notInGrid = grid.Last().Translate(LinePoint.Right);
		}

		public override LinePoint RawWorldToGrid(Vector2 worldPoint)
		{
			for (int i = 0; i < lookUpWorld.Length; i++)
			{
				if ((lookUpWorld[i] - worldPoint).magnitude < (cellDimensions/2.5f).magnitude)
				{
					return lookUpGrid[i];
				}
			}

			return notInGrid;
		}

		public override Vector2 GridToWorld(LinePoint gridPoint)
		{
			float t = Offset + gridPoint.X * cellDimensions.x * SampleFrequency;
			float s = 1.2f * Mathf.Sign(t) * Mathf.Sqrt(Mathf.Abs(t));
			float x = Radius * s * Mathf.Cos(s);
			float y = Radius * s * Mathf.Sin(s);
			
			return new Vector2(x, y);
		}
	}
}