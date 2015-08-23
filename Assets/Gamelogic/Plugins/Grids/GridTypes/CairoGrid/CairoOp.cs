//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

namespace Gamelogic.Grids
{
	#region Shape Methods
	public partial class CairoOp<TCell>
	{
		/**
			@version1_7
		*/
		[ShapeMethod]
		public CairoShapeInfo<TCell> Hexagon(int side)
		{
			return ShapeFromBase(PointyHexGrid<TCell>.BeginShape().Hexagon(side));
		}

		/**
			@version1_7
		*/
		[ShapeMethod]
		public CairoShapeInfo<TCell> FatRectangle(int width, int height)
		{
			return ShapeFromBase(PointyHexGrid<TCell>.BeginShape().FatRectangle(width, height));
		}

		/**
			@version1_7
		*/
		[ShapeMethod]
		public CairoShapeInfo<TCell> ThinRectangle(int width, int height)
		{
			return ShapeFromBase(PointyHexGrid<TCell>.BeginShape().ThinRectangle(width, height));
		}

		/**
			@version1_7
		*/
		[ShapeMethod]
		public CairoShapeInfo<TCell> Rectangle(int width, int height)
		{
			return ShapeFromBase(PointyHexGrid<TCell>.BeginShape().Rectangle(width, height));
		}

		/**
			@version1_7
		*/
		[ShapeMethod]
		public CairoShapeInfo<TCell> Parallelogram(int width, int height)
		{
			return ShapeFromBase(PointyHexGrid<TCell>.BeginShape().Parallelogram(width, height));
		}
	}
	#endregion
}