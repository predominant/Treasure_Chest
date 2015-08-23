//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

// Auto-generated File

using System.Linq;
using System.Collections.Generic;

namespace Gamelogic.Grids
{
	public static partial class Algorithms
	{
		/**
			Rotates a shape 120 degrees around the vertice shared by the three given points.

			The three points must form a close triangle (they must share a vertex).
		*/
		public static IEnumerable<PointyHexPoint> Rotate120About(
			IEnumerable<PointyHexPoint> shape,
			PointyHexPoint p1,
			PointyHexPoint p2,
			PointyHexPoint p3)
		{
			/*
				If t = (p1 + p2 + p3)/3, then the result is p => (p - t).Rotate120() + t.

				This can be rewritten p => p.Rotate120() - t.Rotate120() + t
				= p.Rotate120() (T - T.Rotate120())/3,
				where T = p1 + p2 + p3.

				This is what this method calculates. This is done so that all coordinates in
				intermediatary calculations stay integers.
			*/

			var translation = p1.Translate(p2.Translate(p3));
			var correction = translation.Subtract(translation.Rotate120()).ScaleDown(3);

			return TransformShape(shape, point => point.Rotate120().Translate(correction)).ToList();
		}

		/**
		Rotates a shape 240 degrees around the vertice shared by the three given points.

		The three points must form a close triangle (they must share a vertex).
	*/
		public static IEnumerable<PointyHexPoint> Rotate240About(
			IEnumerable<PointyHexPoint> shape,
			PointyHexPoint p1,
			PointyHexPoint p2,
			PointyHexPoint p3)
		{
			var translation = p1.Translate(p2.Translate(p3));
			var correction = translation.Subtract(translation.Rotate240()).ScaleDown(3);

			return TransformShape<PointyHexPoint>(shape, point => point.Rotate240().Translate(correction)).ToList();
		}

		/**
			Rotates a shape 180 degrees around the edge shared by the two given points.

			The two points must be neighbors.
		*/
		public static IEnumerable<PointyHexPoint> Rotate180About(
			IEnumerable<PointyHexPoint> shape,
			PointyHexPoint p1,
			PointyHexPoint p2)
		{
			var translation = p1.Translate(p2);
			var correction = translation.Subtract(translation.Rotate180()).ScaleDown(2);

			return TransformShape<PointyHexPoint>(shape, point => point.Rotate180().Translate(correction)).ToList();
		}	
		/**
			Rotates a shape 120 degrees around the vertice shared by the three given points.

			The three points must form a close triangle (they must share a vertex).
		*/
		public static IEnumerable<FlatHexPoint> Rotate120About(
			IEnumerable<FlatHexPoint> shape,
			FlatHexPoint p1,
			FlatHexPoint p2,
			FlatHexPoint p3)
		{
			/*
				If t = (p1 + p2 + p3)/3, then the result is p => (p - t).Rotate120() + t.

				This can be rewritten p => p.Rotate120() - t.Rotate120() + t
				= p.Rotate120() (T - T.Rotate120())/3,
				where T = p1 + p2 + p3.

				This is what this method calculates. This is done so that all coordinates in
				intermediatary calculations stay integers.
			*/

			var translation = p1.Translate(p2.Translate(p3));
			var correction = translation.Subtract(translation.Rotate120()).ScaleDown(3);

			return TransformShape(shape, point => point.Rotate120().Translate(correction)).ToList();
		}

		/**
		Rotates a shape 240 degrees around the vertice shared by the three given points.

		The three points must form a close triangle (they must share a vertex).
	*/
		public static IEnumerable<FlatHexPoint> Rotate240About(
			IEnumerable<FlatHexPoint> shape,
			FlatHexPoint p1,
			FlatHexPoint p2,
			FlatHexPoint p3)
		{
			var translation = p1.Translate(p2.Translate(p3));
			var correction = translation.Subtract(translation.Rotate240()).ScaleDown(3);

			return TransformShape<FlatHexPoint>(shape, point => point.Rotate240().Translate(correction)).ToList();
		}

		/**
			Rotates a shape 180 degrees around the edge shared by the two given points.

			The two points must be neighbors.
		*/
		public static IEnumerable<FlatHexPoint> Rotate180About(
			IEnumerable<FlatHexPoint> shape,
			FlatHexPoint p1,
			FlatHexPoint p2)
		{
			var translation = p1.Translate(p2);
			var correction = translation.Subtract(translation.Rotate180()).ScaleDown(2);

			return TransformShape<FlatHexPoint>(shape, point => point.Rotate180().Translate(correction)).ToList();
		}	
	}
}

