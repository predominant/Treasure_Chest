//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



namespace Gamelogic.Grids
{
	// Documentation in Op.cs
	public partial class PointyTriOp<TCell>
	{
		#region Shape Methods
		[ShapeMethod]
		public PointyTriShapeInfo<TCell> Rectangle(int width, int height)
		{
			return ShapeFromBase(FlatHexGrid<TCell>.BeginShape().Rectangle(width, height));
		}

		[ShapeMethod]
		public PointyTriShapeInfo<TCell> FatRectangle(int width, int height)
		{
			return ShapeFromBase(FlatHexGrid<TCell>.BeginShape().FatRectangle(width, height));
		}

		[ShapeMethod]
		public PointyTriShapeInfo<TCell> ThinRectangle(int width, int height)
		{
			return ShapeFromBase(FlatHexGrid<TCell>.BeginShape().ThinRectangle(width, height));
		}

		[ShapeMethod]
		public PointyTriShapeInfo<TCell> Hexagon(int side)
		{
			return BeginGroup()
				.ParallelogramXZ(2 * side, 2 * side)
				.Translate(0, 2 * side + (2 * side - 1) / 2)
				.Intersection()
				.ParallelogramXY(2 * side, 2 * side)
			.EndGroup(this);
		}

		[ShapeMethod]
		public PointyTriShapeInfo<TCell> RightTriangle(int side)
		{
			return Shape(side, side, x => IsInsideRightTriangle(x, side));
		}

		/**
			The origin is just to the left of the bottom corner.
		*/
		[ShapeMethod]
		public PointyTriShapeInfo<TCell> LeftTriangle(int side)
		{
			var storageBottomLeft = new FlatHexPoint(-side, 0);

			return Shape(side, side, x => IsInsideLeftTriangle(x, side), storageBottomLeft);
		}

		/**
			@version1_1
		*/
		[ShapeMethod]
		public PointyTriShapeInfo<TCell> ParallelogramXY(int width, int height)
		{
			return Shape(width, width, x => IsInsideParallelogramXY(x, width, height));
		}

		/**
			Top corner is origin.
			@version1_1
		*/
		[ShapeMethod]
		public PointyTriShapeInfo<TCell> ParallelogramXZ(int width, int height)
		{
			var storageBottomLeft = new FlatHexPoint(0, 1-width -height);

			return Shape(width, width + height, x => IsInsideParallelogramXZ(x, width, height), storageBottomLeft);
		}

		[ShapeMethod]
		public PointyTriShapeInfo<TCell> Star(int side)
		{
			return 
				BeginGroup()
					.RightTriangle(3 * side)
					.Translate(side, side)
					.Union()
					.LeftTriangle(3 * side)
				.EndGroup(this);
		}
		#endregion

		#region Helpers
		private static bool IsInsideRightTriangle(PointyTriPoint point, int side)
		{
			int y = 2 * (point.X + point.Y) + point.I;
			return point.Y >= 0 && y < 2 * side - 1 && point.X >= 0;
		}

		private static bool IsInsideLeftTriangle(PointyTriPoint point, int side)
		{
			if (point.Y >= side) return false;
			if (point.X >= 0) return false;
			if (point.Z > 0) return false;

			//if ()

			return true;
		}

		private static bool IsInsideParallelogramXY(PointyTriPoint point, int width, int height)
		{
			return
				point.X >= 0 &&
				point.X < width &&
				point.Y >= 0 &&
				point.Y < height;
		}

		private static bool IsInsideParallelogramXZ(PointyTriPoint point, int width, int height)
		{
			return
				point.X >= 0 &&
				point.X < width &&
				point.Z >= 0 &&
				point.Z < height;
		}
	#endregion
	}
}