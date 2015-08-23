//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids
{
	// Documentation in Op.cs
	public partial class RectOp<TCell> : AbstractOp<ShapeStorageInfo<RectPoint>>
	{
		/**
			XXXXX
			XXXXX
			XXX
		*/
		[ShapeMethod]
		public RectShapeInfo<TCell> FixedWidth(int width, int cellCount)
		{
			var rawInfo = MakeShapeStorageInfo<RectPoint>(
				width,
				Mathf.CeilToInt(cellCount / (float)width),
				x => IsInsideFixedWidth(x, width, cellCount));

			return new RectShapeInfo<TCell>(rawInfo);
		}

		[ShapeMethod]
		public RectShapeInfo<TCell> FixedHeight(int height, int cellCount)
		{
			var rawInfo = MakeShapeStorageInfo<RectPoint>(
				Mathf.CeilToInt(cellCount / (float)height),
				height,
				x => IsInsideFixedHeight(x, height, cellCount));

			return new RectShapeInfo<TCell>(rawInfo);
		}

		/**
			Generates a grid in a rectangular shape.

			\param width The width of the grid.
			\param height The height of the grid.
		*/
		[ShapeMethod]
		public RectShapeInfo<TCell> Rectangle(int width, int height)
		{
			var rawInfow = MakeShapeStorageInfo<RectPoint>(
				width,
				height,
				x => IsInsideRect(x, width, height));

			return new RectShapeInfo<TCell>(rawInfow);
		}

		/**
			A rectangle centered at the given center, and with the given side length.

			@version1_9
		*/
		[ShapeMethod]
		public RectShapeInfo<TCell> Circle(int radius)
		{
			int storageHeight = 2*radius + 1;
			int storageWidth = 2*radius + 1;
			var storageBottomLeft = new RectPoint(1 - radius, 1 - radius);

			return Shape(storageWidth, storageHeight, p => IsInsideCircle(p, radius), storageBottomLeft);
		}
		
		/**
			@version1_9
		**/
		private static bool IsInsideCircle(RectPoint point, int radius)
		{
			return Mathf.Max(point.X, point.Y) < radius;
		}

		/**
			A synonym for Rectangle. Provided to support wrapped grids uniformly.

			@version1_7
		*/
		[ShapeMethod]
		public RectShapeInfo<TCell> Parallelogram(int width, int height)
		{
			return Rectangle(width, height);
		}

		[ShapeMethod]
		public RectShapeInfo<TCell> CheckerBoard(int width, int height)
		{
			return CheckerBoard(width, height, true);
		}

		[ShapeMethod]
		public RectShapeInfo<TCell> CheckerBoard(int width, int height, bool includesOrigin)
		{
			var rawInfo = MakeShapeStorageInfo<RectPoint>(
				width, height,
				x => IsInsideCheckerBoard(x, width, height, includesOrigin));

			return new RectShapeInfo<TCell>(rawInfo);
		}

		public static bool IsInsideFixedWidth(RectPoint point, int width, int cellCount)
		{
			return point.X >= 0 && point.X < width && point.Y * width + point.X < cellCount;
		}

		public static bool IsInsideFixedHeight(RectPoint point, int height, int cellCount)
		{
			return point.Y >= 0 && point.Y < height && point.X * height + point.Y < cellCount;
		}

		public static bool IsInsideRect(RectPoint point, int width, int height)
		{
			return point.X >= 0 && point.X < width && point.Y >= 0 && point.Y < height;
		}

		public static bool IsInsideCheckerBoard(RectPoint point, int width, int height, bool includesOrigin)
		{
			return
				IsInsideRect(point, width, height) &&
				(GLMathf.Mod(point.X + point.Y, 2) == (includesOrigin ? 0 : 1));
		}
	}
}
