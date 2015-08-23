//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Extension methods for Vector2.

		@version1_2
	*/
	public static class Vector2Extensions
	{
		/**
			Floors each component and returns the corresponding VectorPoint.

			@version1_7
		*/

		public static VectorPoint FloorToVectorPoint(this Vector2 vec)
		{
			return new VectorPoint(Mathf.FloorToInt(vec.x), Mathf.FloorToInt(vec.y));
		}

		public static Vector2 HadamardMul(this Vector2 thisVector, VectorPoint otherVector)
		{
			return new Vector2(thisVector.x * otherVector.X, thisVector.y * otherVector.Y);
		}

		public static Vector2 HadamardDiv(this Vector2 thisVector, VectorPoint otherVector)
		{
			return new Vector2(thisVector.x / otherVector.X, thisVector.y / otherVector.Y);
		}


		/**
		*/
		public static float PerpDot(this Vector2 thisVector, Vector2 otherVector)
		{
			return thisVector.x*otherVector.y - thisVector.y*otherVector.x;
		}
	}
}
