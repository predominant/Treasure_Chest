//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



namespace Gamelogic.Grids
{
	// Documentation in Op.cs
	public partial class FlatHexOp<TCell>
	{
		#region Shape Methods
		/**
			Bottom left corner is the origin.
		*/
		[ShapeMethod]
		public FlatHexShapeInfo<TCell> Rectangle(int width, int height)
		{
			var storageWidth = width;
			var storageHeight = height + GLMathf.Div(width - 1, 2);
			var storageBottomLeft = new FlatHexPoint(0, -GLMathf.Div(width - 1, 2));
			
			return Shape(storageWidth, storageHeight, x => IsInsideRectangle(x, width, height), storageBottomLeft);
		}

		/**
			Bottom left corner is the origin.
		*/
		[ShapeMethod]
		public FlatHexShapeInfo<TCell> FatRectangle(int width, int height)
		{
			var storageWidth = width;
			var storageHeight = height + GLMathf.Div(width, 2);
			var storageBottomLeft = new FlatHexPoint(0, -GLMathf.Div(width, 2));

			return Shape(storageWidth, storageHeight, x => IsInsideFatRectangle(x, width, height), storageBottomLeft);
		}

		/**
			Bottom left corner is the origin.
		*/
		[ShapeMethod]
		public FlatHexShapeInfo<TCell> ThinRectangle(int width, int height)
		{
			var storageWidth = width;
			var storageHeight = height + GLMathf.Div(width - 1, 2);
			var storageBottomLeft = new FlatHexPoint(0, -GLMathf.Div(width - 1, 2));

			return Shape(storageWidth, storageHeight, x => IsInsideThinRectangle(x, width, height), storageBottomLeft);
		}

		/**
			Center is the origin.
		*/
		[ShapeMethod]
		public FlatHexShapeInfo<TCell> Hexagon(int side)
		{
			var storageSize = 2 * side - 1;
			var storageBottomLeft = new FlatHexPoint(1 - side, 1 - side);

			return Shape(storageSize, storageSize, x => IsInsideHex(x, side), storageBottomLeft);
		}

		/**
			Bottom corner is the origin.
		*/
		[ShapeMethod]
		public FlatHexShapeInfo<TCell> Hexagon(FlatHexPoint centre, int side)
		{
			return Hexagon(side).Translate(centre);
		}

		/**
			Bottom corner is the origin.
		*/
		[ShapeMethod]
		public FlatHexShapeInfo<TCell> LeftTriangle(int side)
		{
			var storageBottomLeft = new FlatHexPoint(1 - side, 0);
			return Shape(side, side, x => IsInsideLeftTriangle(x, side), storageBottomLeft);
		}

		/**
			Bottom corner is the origin.
		*/
		[ShapeMethod]
		public FlatHexShapeInfo<TCell> RightTriangle(int side)
		{
			return Shape(side, side, x => IsInsideRightTriangle(x, side));
		}

		/**
			Bottom corner is the origin.
		*/
		[ShapeMethod]
		public FlatHexShapeInfo<TCell> Parallelogram(int width, int height)
		{
			return Shape(width, height, x => IsInsideXYParallelogram(x, width, height));
		}

		/**
			Bottom corner is the origin.
		*/
		[ShapeMethod]
		public FlatHexShapeInfo<TCell> Diamond(int side)
		{
			return
				BeginGroup()
					.LeftTriangle(side - 1)
					.Translate(new FlatHexPoint(-1, 1))
					.Union()
					.RightTriangle(side)
				.EndGroup(this);
		}
		#endregion

		#region Helpers
		private static bool IsInsideRectangle(FlatHexPoint point, int width, int height)
		{
			int startY = -(GLMathf.Div(point.X, 2));

			return
				point.Y >= startY &&
				point.Y < startY + height &&
				point.X >= 0 &&
				point.X < width;
		}

		private bool IsInsideThinRectangle(FlatHexPoint point, int width, int height)
		{
			int startY = -(GLMathf.Div(point.X, 2));

			return
				point.Y >= startY &&
				point.Y + GLMathf.Mod(point.X, 2) < startY + height &&
				point.X >= 0 &&
				point.X < width;
		}

		private static bool IsInsideFatRectangle(FlatHexPoint point, int width, int height)
		{
			int startY = -(GLMathf.Div(point.X, 2));

			return
				point.Y >= startY - GLMathf.Mod(point.X, 2) &&
				point.Y < startY + height &&
				point.X >= 0 &&
				point.X < width;
		}

		private static bool IsInsideHex(FlatHexPoint point, int side)
		{
			return point.Magnitude() < side;
		}

		private static bool IsInsideXYParallelogram(FlatHexPoint point, int width, int height)
		{
			return
				point.X >= 0 &&
				point.X < width &&
				point.Y >= 0 &&
				point.Y < height;
		}

		private static bool IsInsideRightTriangle(FlatHexPoint point, int side)
		{
			return (point.Y >= 0) && (point.X >= 0) && (point.Y + point.X <= side - 1);
		}

		private static bool IsInsideLeftTriangle(FlatHexPoint point, int side)
		{
			return (point.Y <= side - 1) && (point.X <= 0) && (point.Y + point.X >= 0);
		}
		#endregion
	}
}