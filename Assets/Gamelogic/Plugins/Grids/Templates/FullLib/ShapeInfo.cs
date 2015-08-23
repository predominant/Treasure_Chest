//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

// Auto-generated File

using System;

namespace Gamelogic.Grids
{

	/**
		Provides the implementation for AbstractShapeInfo to be used with RectGrid.

		@link_constructing_grids
		@ingroup Scaffolding
	*/
	public class RectShapeInfo<TCell> : AbstractShapeInfo <RectShapeInfo<TCell>, RectGrid<TCell>, RectPoint, RectPoint, RectOp<TCell>> 
	{
		public RectShapeInfo(ShapeStorageInfo<RectPoint> info):
			base(info)
		{
		}

		/**
			Only call this method from within a RectOp method (usually, in client code, 
			this will be in an extension).

			@param op the operator on which this shape is defined.
			@since 1.1
		*/
		public RectShapeInfo<TCell> EndGroup(RectOp<TCell> op)
		{
			var info = op.combineShapeInfo(op.leftShapeInfo, ShapeStorageStorageInfo);
			return new RectShapeInfo<TCell>(info);
		}

		protected override RectPoint MakePoint(int x, int y)
		{
			return new RectPoint(x, y);
		}

		protected override RectOp<TCell> MakeOp(
			ShapeStorageInfo<RectPoint> shapeInfo, 
			Func<
				ShapeStorageInfo<RectPoint>,
				ShapeStorageInfo<RectPoint>,
				ShapeStorageInfo<RectPoint>> combineInfo)
		{
			return new RectOp<TCell>(shapeInfo,	combineInfo);
		}

		protected override RectShapeInfo<TCell> MakeShapeInfo(
			ShapeStorageInfo<RectPoint> shapeStorageInfo)
		{
			return new RectShapeInfo<TCell>(shapeStorageInfo);
		}

		protected override RectPoint GridPointFromArrayPoint(ArrayPoint point)
		{
			return RectGrid<TCell>.GridPointFromArrayPoint(point);
		}

		protected override ArrayPoint ArrayPointFromGridPoint(RectPoint point)
		{
			return RectGrid<TCell>.ArrayPointFromGridPoint(point);
		}

		protected override RectGrid<TCell> MakeShape(int x, int y, Func<RectPoint, bool> isInside, RectPoint offset)
		{
			return new RectGrid<TCell>(x, y, isInside, offset);
		}
	}

	/**
		Provides the implementation for AbstractShapeInfo to be used with DiamondGrid.

		@link_constructing_grids
		@ingroup Scaffolding
	*/
	public class DiamondShapeInfo<TCell> : AbstractShapeInfo <DiamondShapeInfo<TCell>, DiamondGrid<TCell>, DiamondPoint, DiamondPoint, DiamondOp<TCell>> 
	{
		public DiamondShapeInfo(ShapeStorageInfo<DiamondPoint> info):
			base(info)
		{
		}

		/**
			Only call this method from within a DiamondOp method (usually, in client code, 
			this will be in an extension).

			@param op the operator on which this shape is defined.
			@since 1.1
		*/
		public DiamondShapeInfo<TCell> EndGroup(DiamondOp<TCell> op)
		{
			var info = op.combineShapeInfo(op.leftShapeInfo, ShapeStorageStorageInfo);
			return new DiamondShapeInfo<TCell>(info);
		}

		protected override DiamondPoint MakePoint(int x, int y)
		{
			return new DiamondPoint(x, y);
		}

		protected override DiamondOp<TCell> MakeOp(
			ShapeStorageInfo<DiamondPoint> shapeInfo, 
			Func<
				ShapeStorageInfo<DiamondPoint>,
				ShapeStorageInfo<DiamondPoint>,
				ShapeStorageInfo<DiamondPoint>> combineInfo)
		{
			return new DiamondOp<TCell>(shapeInfo,	combineInfo);
		}

		protected override DiamondShapeInfo<TCell> MakeShapeInfo(
			ShapeStorageInfo<DiamondPoint> shapeStorageInfo)
		{
			return new DiamondShapeInfo<TCell>(shapeStorageInfo);
		}

		protected override DiamondPoint GridPointFromArrayPoint(ArrayPoint point)
		{
			return DiamondGrid<TCell>.GridPointFromArrayPoint(point);
		}

		protected override ArrayPoint ArrayPointFromGridPoint(DiamondPoint point)
		{
			return DiamondGrid<TCell>.ArrayPointFromGridPoint(point);
		}

		protected override DiamondGrid<TCell> MakeShape(int x, int y, Func<DiamondPoint, bool> isInside, DiamondPoint offset)
		{
			return new DiamondGrid<TCell>(x, y, isInside, offset);
		}
	}

	/**
		Provides the implementation for AbstractShapeInfo to be used with PointyHexGrid.

		@link_constructing_grids
		@ingroup Scaffolding
	*/
	public class PointyHexShapeInfo<TCell> : AbstractShapeInfo <PointyHexShapeInfo<TCell>, PointyHexGrid<TCell>, PointyHexPoint, PointyHexPoint, PointyHexOp<TCell>> 
	{
		public PointyHexShapeInfo(ShapeStorageInfo<PointyHexPoint> info):
			base(info)
		{
		}

		/**
			Only call this method from within a PointyHexOp method (usually, in client code, 
			this will be in an extension).

			@param op the operator on which this shape is defined.
			@since 1.1
		*/
		public PointyHexShapeInfo<TCell> EndGroup(PointyHexOp<TCell> op)
		{
			var info = op.combineShapeInfo(op.leftShapeInfo, ShapeStorageStorageInfo);
			return new PointyHexShapeInfo<TCell>(info);
		}

		protected override PointyHexPoint MakePoint(int x, int y)
		{
			return new PointyHexPoint(x, y);
		}

		protected override PointyHexOp<TCell> MakeOp(
			ShapeStorageInfo<PointyHexPoint> shapeInfo, 
			Func<
				ShapeStorageInfo<PointyHexPoint>,
				ShapeStorageInfo<PointyHexPoint>,
				ShapeStorageInfo<PointyHexPoint>> combineInfo)
		{
			return new PointyHexOp<TCell>(shapeInfo,	combineInfo);
		}

		protected override PointyHexShapeInfo<TCell> MakeShapeInfo(
			ShapeStorageInfo<PointyHexPoint> shapeStorageInfo)
		{
			return new PointyHexShapeInfo<TCell>(shapeStorageInfo);
		}

		protected override PointyHexPoint GridPointFromArrayPoint(ArrayPoint point)
		{
			return PointyHexGrid<TCell>.GridPointFromArrayPoint(point);
		}

		protected override ArrayPoint ArrayPointFromGridPoint(PointyHexPoint point)
		{
			return PointyHexGrid<TCell>.ArrayPointFromGridPoint(point);
		}

		protected override PointyHexGrid<TCell> MakeShape(int x, int y, Func<PointyHexPoint, bool> isInside, PointyHexPoint offset)
		{
			return new PointyHexGrid<TCell>(x, y, isInside, offset);
		}
	}

	/**
		Provides the implementation for AbstractShapeInfo to be used with FlatHexGrid.

		@link_constructing_grids
		@ingroup Scaffolding
	*/
	public class FlatHexShapeInfo<TCell> : AbstractShapeInfo <FlatHexShapeInfo<TCell>, FlatHexGrid<TCell>, FlatHexPoint, FlatHexPoint, FlatHexOp<TCell>> 
	{
		public FlatHexShapeInfo(ShapeStorageInfo<FlatHexPoint> info):
			base(info)
		{
		}

		/**
			Only call this method from within a FlatHexOp method (usually, in client code, 
			this will be in an extension).

			@param op the operator on which this shape is defined.
			@since 1.1
		*/
		public FlatHexShapeInfo<TCell> EndGroup(FlatHexOp<TCell> op)
		{
			var info = op.combineShapeInfo(op.leftShapeInfo, ShapeStorageStorageInfo);
			return new FlatHexShapeInfo<TCell>(info);
		}

		protected override FlatHexPoint MakePoint(int x, int y)
		{
			return new FlatHexPoint(x, y);
		}

		protected override FlatHexOp<TCell> MakeOp(
			ShapeStorageInfo<FlatHexPoint> shapeInfo, 
			Func<
				ShapeStorageInfo<FlatHexPoint>,
				ShapeStorageInfo<FlatHexPoint>,
				ShapeStorageInfo<FlatHexPoint>> combineInfo)
		{
			return new FlatHexOp<TCell>(shapeInfo,	combineInfo);
		}

		protected override FlatHexShapeInfo<TCell> MakeShapeInfo(
			ShapeStorageInfo<FlatHexPoint> shapeStorageInfo)
		{
			return new FlatHexShapeInfo<TCell>(shapeStorageInfo);
		}

		protected override FlatHexPoint GridPointFromArrayPoint(ArrayPoint point)
		{
			return FlatHexGrid<TCell>.GridPointFromArrayPoint(point);
		}

		protected override ArrayPoint ArrayPointFromGridPoint(FlatHexPoint point)
		{
			return FlatHexGrid<TCell>.ArrayPointFromGridPoint(point);
		}

		protected override FlatHexGrid<TCell> MakeShape(int x, int y, Func<FlatHexPoint, bool> isInside, FlatHexPoint offset)
		{
			return new FlatHexGrid<TCell>(x, y, isInside, offset);
		}
	}

	/**
		Provides the implementation for AbstractSplicedShapeInfo to be used with PointyTriGrid.

		@link_constructing_grids
		@ingroup Scaffolding
	*/
	public class PointyTriShapeInfo<TCell> : AbstractSplicedShapeInfo <PointyTriShapeInfo<TCell>, PointyTriGrid<TCell>, PointyTriPoint, FlatHexPoint, PointyTriOp<TCell>> 
	{
		public PointyTriShapeInfo(ShapeStorageInfo<PointyTriPoint> info):
			base(info)
		{
		}

		/**
			Only call this method from within a PointyTriOp method (usually, in client code, 
			this will be in an extension).

			@param op the operator on which this shape is defined.
			@since 1.1
		*/
		public PointyTriShapeInfo<TCell> EndGroup(PointyTriOp<TCell> op)
		{
			var info = op.combineShapeInfo(op.leftShapeInfo, ShapeStorageStorageInfo);
			return new PointyTriShapeInfo<TCell>(info);
		}

		protected override FlatHexPoint MakePoint(int x, int y)
		{
			return new FlatHexPoint(x, y);
		}

		protected override PointyTriOp<TCell> MakeOp(
			ShapeStorageInfo<PointyTriPoint> shapeInfo, 
			Func<
				ShapeStorageInfo<PointyTriPoint>,
				ShapeStorageInfo<PointyTriPoint>,
				ShapeStorageInfo<PointyTriPoint>> combineInfo)
		{
			return new PointyTriOp<TCell>(shapeInfo,	combineInfo);
		}

		protected override PointyTriShapeInfo<TCell> MakeShapeInfo(
			ShapeStorageInfo<PointyTriPoint> shapeStorageInfo)
		{
			return new PointyTriShapeInfo<TCell>(shapeStorageInfo);
		}

		protected override FlatHexPoint GridPointFromArrayPoint(ArrayPoint point)
		{
			return PointyTriGrid<TCell>.GridPointFromArrayPoint(point);
		}

		protected override ArrayPoint ArrayPointFromGridPoint(FlatHexPoint point)
		{
			return PointyTriGrid<TCell>.ArrayPointFromGridPoint(point);
		}

		protected override PointyTriGrid<TCell> MakeShape(int x, int y, Func<PointyTriPoint, bool> isInside, FlatHexPoint offset)
		{
			return new PointyTriGrid<TCell>(x, y, isInside, offset);
		}
	}

	/**
		Provides the implementation for AbstractSplicedShapeInfo to be used with FlatTriGrid.

		@link_constructing_grids
		@ingroup Scaffolding
	*/
	public class FlatTriShapeInfo<TCell> : AbstractSplicedShapeInfo <FlatTriShapeInfo<TCell>, FlatTriGrid<TCell>, FlatTriPoint, PointyHexPoint, FlatTriOp<TCell>> 
	{
		public FlatTriShapeInfo(ShapeStorageInfo<FlatTriPoint> info):
			base(info)
		{
		}

		/**
			Only call this method from within a FlatTriOp method (usually, in client code, 
			this will be in an extension).

			@param op the operator on which this shape is defined.
			@since 1.1
		*/
		public FlatTriShapeInfo<TCell> EndGroup(FlatTriOp<TCell> op)
		{
			var info = op.combineShapeInfo(op.leftShapeInfo, ShapeStorageStorageInfo);
			return new FlatTriShapeInfo<TCell>(info);
		}

		protected override PointyHexPoint MakePoint(int x, int y)
		{
			return new PointyHexPoint(x, y);
		}

		protected override FlatTriOp<TCell> MakeOp(
			ShapeStorageInfo<FlatTriPoint> shapeInfo, 
			Func<
				ShapeStorageInfo<FlatTriPoint>,
				ShapeStorageInfo<FlatTriPoint>,
				ShapeStorageInfo<FlatTriPoint>> combineInfo)
		{
			return new FlatTriOp<TCell>(shapeInfo,	combineInfo);
		}

		protected override FlatTriShapeInfo<TCell> MakeShapeInfo(
			ShapeStorageInfo<FlatTriPoint> shapeStorageInfo)
		{
			return new FlatTriShapeInfo<TCell>(shapeStorageInfo);
		}

		protected override PointyHexPoint GridPointFromArrayPoint(ArrayPoint point)
		{
			return FlatTriGrid<TCell>.GridPointFromArrayPoint(point);
		}

		protected override ArrayPoint ArrayPointFromGridPoint(PointyHexPoint point)
		{
			return FlatTriGrid<TCell>.ArrayPointFromGridPoint(point);
		}

		protected override FlatTriGrid<TCell> MakeShape(int x, int y, Func<FlatTriPoint, bool> isInside, PointyHexPoint offset)
		{
			return new FlatTriGrid<TCell>(x, y, isInside, offset);
		}
	}

	/**
		Provides the implementation for AbstractSplicedShapeInfo to be used with PointyRhombGrid.

		@link_constructing_grids
		@ingroup Scaffolding
	*/
	public class PointyRhombShapeInfo<TCell> : AbstractSplicedShapeInfo <PointyRhombShapeInfo<TCell>, PointyRhombGrid<TCell>, PointyRhombPoint, PointyHexPoint, PointyRhombOp<TCell>> 
	{
		public PointyRhombShapeInfo(ShapeStorageInfo<PointyRhombPoint> info):
			base(info)
		{
		}

		/**
			Only call this method from within a PointyRhombOp method (usually, in client code, 
			this will be in an extension).

			@param op the operator on which this shape is defined.
			@since 1.1
		*/
		public PointyRhombShapeInfo<TCell> EndGroup(PointyRhombOp<TCell> op)
		{
			var info = op.combineShapeInfo(op.leftShapeInfo, ShapeStorageStorageInfo);
			return new PointyRhombShapeInfo<TCell>(info);
		}

		protected override PointyHexPoint MakePoint(int x, int y)
		{
			return new PointyHexPoint(x, y);
		}

		protected override PointyRhombOp<TCell> MakeOp(
			ShapeStorageInfo<PointyRhombPoint> shapeInfo, 
			Func<
				ShapeStorageInfo<PointyRhombPoint>,
				ShapeStorageInfo<PointyRhombPoint>,
				ShapeStorageInfo<PointyRhombPoint>> combineInfo)
		{
			return new PointyRhombOp<TCell>(shapeInfo,	combineInfo);
		}

		protected override PointyRhombShapeInfo<TCell> MakeShapeInfo(
			ShapeStorageInfo<PointyRhombPoint> shapeStorageInfo)
		{
			return new PointyRhombShapeInfo<TCell>(shapeStorageInfo);
		}

		protected override PointyHexPoint GridPointFromArrayPoint(ArrayPoint point)
		{
			return PointyRhombGrid<TCell>.GridPointFromArrayPoint(point);
		}

		protected override ArrayPoint ArrayPointFromGridPoint(PointyHexPoint point)
		{
			return PointyRhombGrid<TCell>.ArrayPointFromGridPoint(point);
		}

		protected override PointyRhombGrid<TCell> MakeShape(int x, int y, Func<PointyRhombPoint, bool> isInside, PointyHexPoint offset)
		{
			return new PointyRhombGrid<TCell>(x, y, isInside, offset);
		}
	}

	/**
		Provides the implementation for AbstractSplicedShapeInfo to be used with FlatRhombGrid.

		@link_constructing_grids
		@ingroup Scaffolding
	*/
	public class FlatRhombShapeInfo<TCell> : AbstractSplicedShapeInfo <FlatRhombShapeInfo<TCell>, FlatRhombGrid<TCell>, FlatRhombPoint, FlatHexPoint, FlatRhombOp<TCell>> 
	{
		public FlatRhombShapeInfo(ShapeStorageInfo<FlatRhombPoint> info):
			base(info)
		{
		}

		/**
			Only call this method from within a FlatRhombOp method (usually, in client code, 
			this will be in an extension).

			@param op the operator on which this shape is defined.
			@since 1.1
		*/
		public FlatRhombShapeInfo<TCell> EndGroup(FlatRhombOp<TCell> op)
		{
			var info = op.combineShapeInfo(op.leftShapeInfo, ShapeStorageStorageInfo);
			return new FlatRhombShapeInfo<TCell>(info);
		}

		protected override FlatHexPoint MakePoint(int x, int y)
		{
			return new FlatHexPoint(x, y);
		}

		protected override FlatRhombOp<TCell> MakeOp(
			ShapeStorageInfo<FlatRhombPoint> shapeInfo, 
			Func<
				ShapeStorageInfo<FlatRhombPoint>,
				ShapeStorageInfo<FlatRhombPoint>,
				ShapeStorageInfo<FlatRhombPoint>> combineInfo)
		{
			return new FlatRhombOp<TCell>(shapeInfo,	combineInfo);
		}

		protected override FlatRhombShapeInfo<TCell> MakeShapeInfo(
			ShapeStorageInfo<FlatRhombPoint> shapeStorageInfo)
		{
			return new FlatRhombShapeInfo<TCell>(shapeStorageInfo);
		}

		protected override FlatHexPoint GridPointFromArrayPoint(ArrayPoint point)
		{
			return FlatRhombGrid<TCell>.GridPointFromArrayPoint(point);
		}

		protected override ArrayPoint ArrayPointFromGridPoint(FlatHexPoint point)
		{
			return FlatRhombGrid<TCell>.ArrayPointFromGridPoint(point);
		}

		protected override FlatRhombGrid<TCell> MakeShape(int x, int y, Func<FlatRhombPoint, bool> isInside, FlatHexPoint offset)
		{
			return new FlatRhombGrid<TCell>(x, y, isInside, offset);
		}
	}

	/**
		Provides the implementation for AbstractSplicedShapeInfo to be used with CairoGrid.

		@link_constructing_grids
		@ingroup Scaffolding
	*/
	public class CairoShapeInfo<TCell> : AbstractSplicedShapeInfo <CairoShapeInfo<TCell>, CairoGrid<TCell>, CairoPoint, PointyHexPoint, CairoOp<TCell>> 
	{
		public CairoShapeInfo(ShapeStorageInfo<CairoPoint> info):
			base(info)
		{
		}

		/**
			Only call this method from within a CairoOp method (usually, in client code, 
			this will be in an extension).

			@param op the operator on which this shape is defined.
			@since 1.1
		*/
		public CairoShapeInfo<TCell> EndGroup(CairoOp<TCell> op)
		{
			var info = op.combineShapeInfo(op.leftShapeInfo, ShapeStorageStorageInfo);
			return new CairoShapeInfo<TCell>(info);
		}

		protected override PointyHexPoint MakePoint(int x, int y)
		{
			return new PointyHexPoint(x, y);
		}

		protected override CairoOp<TCell> MakeOp(
			ShapeStorageInfo<CairoPoint> shapeInfo, 
			Func<
				ShapeStorageInfo<CairoPoint>,
				ShapeStorageInfo<CairoPoint>,
				ShapeStorageInfo<CairoPoint>> combineInfo)
		{
			return new CairoOp<TCell>(shapeInfo,	combineInfo);
		}

		protected override CairoShapeInfo<TCell> MakeShapeInfo(
			ShapeStorageInfo<CairoPoint> shapeStorageInfo)
		{
			return new CairoShapeInfo<TCell>(shapeStorageInfo);
		}

		protected override PointyHexPoint GridPointFromArrayPoint(ArrayPoint point)
		{
			return CairoGrid<TCell>.GridPointFromArrayPoint(point);
		}

		protected override ArrayPoint ArrayPointFromGridPoint(PointyHexPoint point)
		{
			return CairoGrid<TCell>.ArrayPointFromGridPoint(point);
		}

		protected override CairoGrid<TCell> MakeShape(int x, int y, Func<CairoPoint, bool> isInside, PointyHexPoint offset)
		{
			return new CairoGrid<TCell>(x, y, isInside, offset);
		}
	}
}
