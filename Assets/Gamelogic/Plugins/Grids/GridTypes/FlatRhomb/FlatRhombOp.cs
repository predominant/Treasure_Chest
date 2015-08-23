//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



namespace Gamelogic.Grids
{
	#region Shape Methods
	// Documentation in Op.cs
	public partial class FlatRhombOp<TCell> : AbstractOp<ShapeStorageInfo<FlatRhombPoint>>
	{
		[ShapeMethod]
		public FlatRhombShapeInfo<TCell> Rectangle(int width, int height)
		{
			return ShapeFromBase(FlatHexGrid<TCell>.BeginShape().Rectangle(width, height));
		}

		[ShapeMethod]
		public FlatRhombShapeInfo<TCell> Hexagon(int side)
		{
			return ShapeFromBase(FlatHexGrid<TCell>.BeginShape().Hexagon(side));
		}

		[ShapeMethod]
		public FlatRhombShapeInfo<TCell> Parallelogram(int width, int height)
		{
			return ShapeFromBase(FlatHexGrid<TCell>.BeginShape().Parallelogram(width, height));
		}

		[ShapeMethod]
		public FlatRhombShapeInfo<TCell> FatRectangle(int width, int height)
		{
			return ShapeFromBase(FlatHexGrid<TCell>.BeginShape().FatRectangle(width, height));
		}

		[ShapeMethod]
		public FlatRhombShapeInfo<TCell> ThinRectangle(int width, int height)
		{
			return ShapeFromBase(FlatHexGrid<TCell>.BeginShape().ThinRectangle(width, height));
		}
	}
	#endregion
}