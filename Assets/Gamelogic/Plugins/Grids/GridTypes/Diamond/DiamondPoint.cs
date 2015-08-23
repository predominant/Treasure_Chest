//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System;
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Class that represents a points of a DiamondGrid.
	
		
		
		@version1_0

		@ingroup Points
	*/
	[Serializable]
	[Immutable]
	public partial struct DiamondPoint :
		IGridPoint<DiamondPoint>,
		IVectorPoint<DiamondPoint>
	{
		#region Constants
		public static readonly DiamondPoint NorthEast = new DiamondPoint(1, 0);
		public static readonly DiamondPoint NorthWest = new DiamondPoint(0, 1);
		public static readonly DiamondPoint SouthWest = new DiamondPoint(-1, 0);
		public static readonly DiamondPoint SouthEast = new DiamondPoint(0, -1);

		public static readonly DiamondPoint East = NorthEast + SouthEast;
		public static readonly DiamondPoint North = NorthEast + NorthWest;
		public static readonly DiamondPoint West = NorthWest + SouthWest;
		public static readonly DiamondPoint South = SouthEast + NorthEast;

		public static readonly PointList<DiamondPoint> MainDirections = new PointList<DiamondPoint>
		{
			NorthEast,
			NorthWest,
			SouthWest,
			SouthEast
		};

		public static readonly PointList<DiamondPoint> DiagonalDirections = new PointList<DiamondPoint>
		{
			East,
			North,
			West,
			South
		};

		public static readonly PointList<DiamondPoint> MainAndDiagonalDirections = new PointList<DiamondPoint>
		{
			East,
			NorthEast,
			North,
			NorthWest,
			West,
			SouthWest,
			South,
			SouthEast
		};
		#endregion

		#region Transformations
		public DiamondPoint ReflectAboutY()
		{
			return new DiamondPoint(Y, X);
		}

		public DiamondPoint ReflectAboutX()
		{
			return new DiamondPoint(-Y, -X);
		}

		public DiamondPoint Rotate270()
		{
			return new DiamondPoint(Y, -X);
		}

		public DiamondPoint Rotate180()
		{
			return new DiamondPoint(-X, -Y);
		}

		public DiamondPoint Rotate90()
		{
			return new DiamondPoint(-Y, X);
		}
		#endregion

		#region Magnitude
		public int Magnitude()
		{
			return Mathf.Abs(X) + Mathf.Abs(Y);
		}
		#endregion

		#region Colourings
		public int GetColor1_1()
		{
			return 0;
		}

		public int GetColor1_2()
		{
			return GLMathf.Mod(X + Y, 2);
		}

		public int GetColor4()
		{
			return GLMathf.Mod(X + 3 * Y, 8);
		}
		#endregion
	}
}