//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2014 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System.Globalization;
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Represents 1D grid points. These are auto-convertible to integers, making it possible to 
		write, for example, `grid[6]` instead of `grid[new LinePoint(6)]`.
	
		@version1_8
		@immutable
		@ingroup Points
	*/
	public partial struct LinePoint : IGridPoint<LinePoint>, IVectorPoint<LinePoint>
	{
		public static readonly LinePoint Zero = 0;
		
		/**
			Add this to another LinePoint to get the point to the left (negative side) of the other point.
		*/
		public static readonly LinePoint Left = -1;
		
		/**
			Add this to another LinePoint to get the point to the right (positive side) of the other point.
		*/
		public static readonly LinePoint Right = 1;

		private readonly int n;
		public int X
		{
			get { return n; }
		}

		public int Y
		{
			get { return 0; }
		}

		public int SpliceIndex
		{
			get
			{
				return 0;
			}
		}

		public int SpliceCount
		{
			get
			{
				return 1;
			}
		}

		public LinePoint(int n)
		{
			this.n = n;
		}

		public bool Equals(LinePoint other)
		{
			return n == other.n;
		}

		public int DistanceFrom(LinePoint other)
		{
			return Mathf.Abs(n - other.n);
		}

		public static implicit operator LinePoint(int n)
		{
			return new LinePoint(n);
		}

		public static implicit operator int(LinePoint point)
		{
			return point.n;
		}

		public LinePoint Translate(LinePoint vector)
		{
			return n + vector.n;
		}

		public LinePoint Negate()
		{
			return -n;
		}

		public LinePoint Subtract(LinePoint vector)
		{
			return n - vector.n;
		}

		public LinePoint MoveBy(LinePoint splicedVector)
		{
			return Translate(splicedVector);
		}

		public LinePoint MoveBackBy(LinePoint splicedVector)
		{
			return Subtract(splicedVector);
		}

		public LinePoint ScaleDown(int r)
		{
			return GLMathf.Div(n, r);
		}

		public LinePoint ScaleUp(int r)
		{
			return n*r;
		}

		public LinePoint Div(LinePoint other)
		{
			return GLMathf.Div(n, other.n);
		}

		public LinePoint Mod(LinePoint other)
		{
			return GLMathf.Mod(n, other.n);
		}

		public LinePoint Mul(LinePoint other)
		{
			return n*other.n;
		}

		public int Magnitude()
		{
			return Mathf.Abs(n);
		}

		public int GetColor(int colorCount)
		{
			return GLMathf.Mod(n, colorCount);
		}

		override public string ToString()
		{
			return n.ToString(CultureInfo.InvariantCulture);
		}
	}
}