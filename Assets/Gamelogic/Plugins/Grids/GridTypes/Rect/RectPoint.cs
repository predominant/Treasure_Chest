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
		The point to be used with rectangular grids.
 
		@immutable
	
		
		
		@version1_0

		@ingroup Points
	*/
	[Serializable]
	[Immutable]
	public partial struct RectPoint :
		IGridPoint<RectPoint>,
		IVectorPoint<RectPoint>
	{
		#region Constants
		public static readonly RectPoint North = new RectPoint(0, 1);
		public static readonly RectPoint East = new RectPoint(1, 0);
		public static readonly RectPoint South = new RectPoint(0, -1);
		public static readonly RectPoint West = new RectPoint(-1, 0);

		public static readonly RectPoint NorthEast = North + East;
		public static readonly RectPoint NorthWest = North + West;
		public static readonly RectPoint SouthWest = South + West;
		public static readonly RectPoint SouthEast = South + East;

		public static readonly PointList<RectPoint> MainDirections = new PointList<RectPoint>
		{
			East,
			North,
			West,
			South
		};


		public static readonly PointList<RectPoint> DiagonalDirections = new PointList<RectPoint>
		{
			NorthEast,
			NorthWest,
			SouthWest,
			SouthEast
		};

		public static readonly PointList<RectPoint> MainAndDiagonalDirections = new PointList<RectPoint>
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

		#region Magnitude
		public int Magnitude()
		{
			return Mathf.Abs(X) + Mathf.Abs(Y);
		}
		#endregion

		#region Transforms
		public object Rotate180()
		{
			return new RectPoint(-X, -Y);
		}

		public object Rotate270()
		{
			return new RectPoint(Y, -X);
		}

		public object ReflectAboutX()
		{
			return new RectPoint(X, -Y);
		}

		public object ReflectAboutY()
		{
			return new RectPoint(-X, Y);
		}

		public RectPoint Rotate90()
		{
			return new RectPoint(-Y, X);
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
