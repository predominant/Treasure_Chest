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
		This class is used for accessing 2D arrays.
	
		It is mainly used as a convenient wrapper for 
		returning results of coordinate caluclations. 
	
		
		
		@version1_0

		@immutable
		@ingroup Helpers

	*/
	[Serializable]
	[Immutable]
	public struct ArrayPoint : IEquatable<ArrayPoint>
	{
		private readonly int x;
		private readonly int y;

		public static readonly ArrayPoint Zero = new ArrayPoint(0, 0);
		public static readonly ArrayPoint One = new ArrayPoint(1, 1);

		/**
			Returns the x-coordinate of this coordinate pair.
		*/
		public int X
		{
			get
			{
				return x;
			}
		}

		/**
			Returns the y-coordinate of this coordinate pair.
		*/
		public int Y
		{
			get
			{
				return y;
			}
		}

		/**
			Constructs a new ArrayPoint with the given coordinates.
		*/
		public ArrayPoint(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		/**
			Converts this ArrayPoint into a string. 
		
			The result is the string "(x, y)".
		*/
		public override string ToString()
		{
			return string.Format("[{0}, {1}]", X, Y);
		}

		public bool Equals(ArrayPoint other)
		{
			return x == other.x && y == other.y;
		}

		public override bool Equals(object other)
		{
			if (other.GetType() != typeof(ArrayPoint))
			{
				return false;
			}

			var point = (ArrayPoint)other;

			return Equals(point);
		}

		public static bool operator ==(ArrayPoint p1, ArrayPoint p2)
		{
			return p1.Equals(p2);
		}

		public static bool operator !=(ArrayPoint p1, ArrayPoint p2)
		{
			return !p1.Equals(p2);
		}

		public static ArrayPoint operator +(ArrayPoint p1, ArrayPoint p2)
		{
			return Add(p1, p2);
		}

		public static ArrayPoint operator -(ArrayPoint p1, ArrayPoint p2)
		{
			return Subtract(p1, p2);
		}

		public static ArrayPoint Add(ArrayPoint p1, ArrayPoint p2)
		{
			return new ArrayPoint(p1.x + p2.x, p1.y + p2.y);
		}

		public static ArrayPoint Subtract(ArrayPoint p1, ArrayPoint p2)
		{
			return new ArrayPoint(p1.x - p2.x, p1.y - p2.y);
		}

		public static ArrayPoint Min(ArrayPoint p1, ArrayPoint p2)
		{
			return new ArrayPoint(Mathf.Min(p1.x, p2.x), Mathf.Min(p1.y, p2.y));
		}

		public static ArrayPoint Max(ArrayPoint p1, ArrayPoint p2)
		{
			return new ArrayPoint(Mathf.Max(p1.x, p2.x), Mathf.Max(p1.y, p2.y));
		}

		override public int GetHashCode()
		{
			return x.GetHashCode() ^ y.GetHashCode();
		}
	}
}