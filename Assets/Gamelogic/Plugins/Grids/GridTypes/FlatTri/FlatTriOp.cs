//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



namespace Gamelogic.Grids
{
	// Documentation in Op.cs
	public partial class FlatTriOp<TCell>
	{
		#region Shape Methods
		[ShapeMethod]
		public FlatTriShapeInfo<TCell> Rectangle(int width, int height)
		{
			return ShapeFromBase(PointyHexGrid<TCell>.BeginShape().Rectangle(width, height));
		}

		[ShapeMethod]
		public FlatTriShapeInfo<TCell> UpTriangle(int side)
		{
			return Shape(side, side, x => IsInsideUpTriangle(x, side));
		}

		[ShapeMethod]
		public FlatTriShapeInfo<TCell> DownTriangle(int side)
		{
			var storageBottomLeft = new PointyHexPoint(0, -side);
			return Shape(side, side, x => IsInsideDownTriangle(x, side), storageBottomLeft);
		}

		/**
			@version1_1
		*/
		[ShapeMethod]
		public FlatTriShapeInfo<TCell> ParallelogramXY(int width, int height)
		{
			return Shape(width + height, height, x => IsInsideParallelogramXY(x, width, height));
		}

		/**
			Top corner is origin.
			@version1_1
		*/
		[ShapeMethod]
		public FlatTriShapeInfo<TCell> ParallelogramXZ(int width, int height)
		{
			var storageBottomLeft = new PointyHexPoint(0, 1-width - height);

			return Shape(width, height + width, x => IsInsideParallelogramXZ(x, width, height), storageBottomLeft);
		
		}

		[ShapeMethod]
		public FlatTriShapeInfo<TCell> Star(int side)
		{
			return 
				BeginGroup()
					.UpTriangle(3 * side)
					.Translate(side, side)
					.Union()
					.DownTriangle(3 * side)
				.EndGroup(this);
		}

		[ShapeMethod]
		public FlatTriShapeInfo<TCell> Hexagon(int side)
		{
			return
				BeginGroup()
					.ParallelogramXZ(2 * side, 2 * side)
					.Translate(0, 2 * side + (2 * side - 1) / 2)
					.Intersection()
					.ParallelogramXY(2 * side, 2 * side)
				.EndGroup(this);
		}
		#endregion

		#region Helpers
		private static bool IsInsideUpTriangle(FlatTriPoint point, int side)
		{
			int x = 2 * (point.X + point.Y) + point.I;
			return point.X >= 0 && x < 2 * side - 1 && point.Y >= 0;
		}

		private static bool IsInsideDownTriangle(FlatTriPoint point, int side)
		{
			if (point.X >= side) return false;
			if (point.Y >= 0) return false;
			if (point.Z > 0) return false;
			
			//if ()

			return true;
		}

		private static bool IsInsideParallelogramXY(FlatTriPoint point, int width, int height)
		{
			return
				point.X >= 0 &&
				point.X < width &&
				point.Y >= 0 &&
				point.Y < height;
		}

		private static bool IsInsideParallelogramXZ(FlatTriPoint point, int width, int height)
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