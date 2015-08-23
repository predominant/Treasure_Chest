//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Provides extension methods for transforming Vector2 instances.

		
		
		@version1_0

		@ingroup Helpers
	*/
	public static class Vector2Transforms
	{
		public static Vector2 ReflectAboutX(this Vector2 v)
		{
			return new Vector2(v.x, -v.y);
		}

		public static Vector2 ReflectAboutY(this Vector2 v)
		{
			return new Vector2(-v.x, v.y);
		}

		/**
			\param angle in degrees.
		*/
		public static Vector2 Rotate(this Vector2 v, float angle)
		{
			float alpha = Mathf.Deg2Rad * angle;
			float cosAngle = Mathf.Cos(alpha);
			float sinAngle = Mathf.Sin(alpha);

			float x = v.x * cosAngle - v.y * sinAngle;
			float y = v.x * sinAngle + v.y * cosAngle;

			return new Vector2(x, y);
		}

		public static Vector2 RotateAround(this Vector2 v, float angle, Vector2 axis)
		{
			return (v - axis).Rotate(angle) + axis;
		}

		public static Vector2 Rotate90(this Vector2 v)
		{
			return new Vector2(-v.y, v.x);
		}

		public static Vector2 Rotate180(this Vector2 v)
		{
			return new Vector2(-v.x, -v.y);
		}

		public static Vector2 Rotate270(this Vector2 v)
		{
			return new Vector2(v.y, -v.x);
		}

		public static Vector2 ReflectXY(this Vector2 v)
		{
			return new Vector2(v.y, v.x);
		}

		public static Vector3 XYTo3D(this Vector2 v)
		{
			return XYTo3D(v, 0);
		}

		public static Vector3 XYTo3D(this Vector2 v, float z)
		{
			return new Vector3(v.x, v.y, z);
		}

		public static Vector3 XZTo3D(this Vector2 v)
		{
			return XZTo3D(v, 0);
		}

		public static Vector3 XZTo3D(this Vector2 v, float y)
		{
			return new Vector3(v.x, y, v.y);
		}
	}
}