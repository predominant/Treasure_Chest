//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



namespace Gamelogic.Grids
{
	// Documentation in Op.cs
	public partial class PointyHexOp<TCell>
	{
		#region Shape methods
		/**
			The bottom left corner is the origin.
		*/
		[ShapeMethod]
		public PointyHexShapeInfo<TCell> Rectangle(int width, int height)
		{
			int storageWidth = width + GLMathf.Div(height - 1, 2);
			int storageHeight = height;

			var storageBottomLeft = new PointyHexPoint(-GLMathf.Div(height - 1, 2), 0);

			return Shape(storageWidth, storageHeight, x => IsInsideRectangle(x, width, height), storageBottomLeft);
		}

		/**
			The center is the origin.
		*/
		[ShapeMethod]
		public PointyHexShapeInfo<TCell> Hexagon(int side)
		{
			var storageSize = 2*side - 1;
			var storageBottomLeft = new PointyHexPoint(1 - side, 1 - side);

			return Shape(storageSize, storageSize, x => IsInsideHex(x, side), storageBottomLeft);
		}

		[ShapeMethod]
		public PointyHexShapeInfo<TCell> Hexagon(PointyHexPoint centre, int side)
		{
			return Hexagon(side).Translate(centre);
		}

		/**
			The bottom left corner is the origin.
		*/
		[ShapeMethod]
		public PointyHexShapeInfo<TCell> Parallelogram(int width, int height)
		{
			return Shape(width, height, x => IsInsideXYParallelogram(x, width, height));
		}

		/**
			Origin is bottom left corner.
		*/
		[ShapeMethod]
		public PointyHexShapeInfo<TCell> UpTriangle(int side)
		{
			return Shape(side, side, x => IsInsideUpTriangle(x, side));
		}

		/**
			Origin is top right corner.

			(This definition changed in 1.7).
		*/
		[ShapeMethod]
		public PointyHexShapeInfo<TCell> DownTriangle(int side)
		{
			var storageBottomLeft = new PointyHexPoint(0, 1 - side);

			return Shape(side, side, x => IsInsideDownTriangle(x, side), storageBottomLeft);
		}

		/**
			Left corner is the origin.

			(This definition changed in 1.7).
		*/
		[ShapeMethod]
		public PointyHexShapeInfo<TCell> Diamond(int side)
		{
			return
				BeginGroup()
					.DownTriangle(side - 1)
					.Translate(PointyHexPoint.SouthEast)
					.Union()
					.UpTriangle(side)
				.EndGroup(this);
		}

		/**
			The bottom-left corner is the origin. 
		*/
		[ShapeMethod]
		public PointyHexShapeInfo<TCell> ThinRectangle(int width, int height)
		{
			int storageWidth = width + GLMathf.Div(height - 1, 2);
			int storageHeight = height;
			var storageBottomLeft = new PointyHexPoint(-GLMathf.Div(height - 1, 2), 0);

			return Shape(storageWidth, storageHeight, x => IsInsideThinRectangle(x, width, height), storageBottomLeft);
		}

		/**
			The bottom-left corner is the origin. 
		*/
		[ShapeMethod]
		public PointyHexShapeInfo<TCell> FatRectangle(int width, int height)
		{
			int storageWidth = width + GLMathf.Div(height, 2);
			int storageHeight = height;
			var storageBottomLeft = new PointyHexPoint(-GLMathf.Div(height, 2), 0);

			return Shape(storageWidth, storageHeight, x => IsInsideFatRectangle(x, width, height), storageBottomLeft);
		}
		#endregion

		#region Helpers
		private static bool IsInsideRectangle(PointyHexPoint point, int width, int height)
		{
			int startX = -(GLMathf.Div(point.Y, 2));

			return
				point.Y >= 0 &&
				point.Y < height &&
				point.X >= startX &&
				point.X < startX + width;
		}

		private static bool IsInsideHex(PointyHexPoint point, int side)
		{
			return point.Magnitude() < side;
		}

		private static bool IsInsideXYParallelogram(PointyHexPoint point, int width, int height)
		{
			return
				(point.X >= 0) &&
				(point.X < width) &&
				(point.Y >= 0) &&
				(point.Y < height);
		}

		private static bool IsInsideUpTriangle(PointyHexPoint point, int side)
		{
			return (point.X >= 0) && (point.Y >= 0) && (point.X + point.Y <= side - 1);
		}

		private static bool IsInsideDownTriangle(PointyHexPoint point, int side)
		{
			return (point.X <= side - 1) && (point.Y <= 0) && (point.X + point.Y >= 0);
		}

		private static bool IsInsideThinRectangle(PointyHexPoint point, int width, int height)
		{
			int startX = -(GLMathf.Div(point.Y, 2));

			return
				point.X >= startX &&
				point.X + GLMathf.Mod(point.Y, 2) < startX + width &&
				point.Y >= 0 &&
				point.Y < height;
		}

		private static bool IsInsideFatRectangle(PointyHexPoint point, int width, int height)
		{
			int startX = -(GLMathf.Div(point.Y, 2));

			return
				point.X >= startX - GLMathf.Mod(point.Y, 2) &&
				point.X < startX + width &&
				point.Y >= 0 &&
				point.Y < height;
		}
		#endregion
	}
}