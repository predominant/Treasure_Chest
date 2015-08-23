//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



namespace Gamelogic.Grids
{
	// Documentation in Op.cs
	public partial class PointyRhombOp<TCell> : AbstractOp<ShapeStorageInfo<PointyRhombPoint>>
	{
		#region Shape Methods
		[ShapeMethod]
		public PointyRhombShapeInfo<TCell> Rectangle(int width, int height)
		{
			return ShapeFromBase(PointyHexGrid<TCell>.BeginShape().Rectangle(width, height));
		}

		[ShapeMethod]
		public PointyRhombShapeInfo<TCell> Hexagon(int side)
		{
			return ShapeFromBase(PointyHexGrid<TCell>.BeginShape().Hexagon(side));
		}

		[ShapeMethod]
		public PointyRhombShapeInfo<TCell> Parallelogram(int width, int height)
		{
			return ShapeFromBase(PointyHexGrid<TCell>.BeginShape().Parallelogram(width, height));
		}

		[ShapeMethod]
		public PointyRhombShapeInfo<TCell> FatRectangle(int width, int height)
		{
			return ShapeFromBase(PointyHexGrid<TCell>.BeginShape().FatRectangle(width, height));
		}

		[ShapeMethod]
		public PointyRhombShapeInfo<TCell> ThinRectangle(int width, int height)
		{
			return ShapeFromBase(PointyHexGrid<TCell>.BeginShape().ThinRectangle(width, height));
		}

		[ShapeMethod]
		public PointyRhombShapeInfo<TCell> UpTriangle(int side)
		{
			return ShapeFromBase(PointyHexGrid<TCell>.BeginShape().UpTriangle(side));
		}

		[ShapeMethod]
		public PointyRhombShapeInfo<TCell> DownTriangle(int side)
		{
			return ShapeFromBase(PointyHexGrid<TCell>.BeginShape().DownTriangle(side));
		}

		[ShapeMethod]
		public PointyRhombShapeInfo<TCell> Diamond(int side)
		{
			return ShapeFromBase(PointyHexGrid<TCell>.BeginShape().Diamond(side));
		}
		#endregion
	}
}