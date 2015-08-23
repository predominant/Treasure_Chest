//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System;

namespace Gamelogic.Grids
{
	/**
		Contains methods for integer arithmetic that works consistently 
		for both positive and negative integers.
		
		@version1_0

		@ingroup Helpers
	*/
	[Obsolete("Use the methods ins GLMathf instead.")]
	public static class Mathi
	{
		/**
			Returns the highest integer equal to the given float.
		 */
		public static int FloorToInt(float x)
		{
			var n = (int)x; //truncate

			if (n > x)
			{
				n = n - 1;
			}

			return n;
		}

		/**
			Mod operator that also works for negative m.
		*/
		public static int Mod(int m, int n)
		{
			if (m >= 0)
			{
				return m % n;
			}

			return (m - 2 * m * n) % n;
		}

		/**
			Floor division that also work for negative m.
		*/
		public static int Div(int m, int n)
		{
			if (m >= 0)
			{
				return m / n;
			}
			
			int t = m / n;

			if (t * n == m)
			{
				return t;
			}

			return t - 1;
		}

		public static float Frac(float x)
		{
			return x - FloorToInt(x); 
		}

		public static int Sine(float p)
		{
			if (p > 0) return 1;
			if (p < 0) return -1;

			return 0;
		}

		public static int Sine(int p)
		{
			if (p > 0) return 1;
			if (p < 0) return -1;

			return 0;
		}
	}
}