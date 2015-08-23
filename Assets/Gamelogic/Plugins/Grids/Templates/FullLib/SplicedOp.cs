//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

// Auto-generated File

using System;

namespace Gamelogic.Grids
{


	public partial class FlatTriOp<TCell> : AbstractOp<ShapeStorageInfo<FlatTriPoint>>
	{
		/**
			This creates a shape described by a shape of the base point shape op.
			Typicaly usage:
			\code
			[ShapeMethod]
			public FlatTriShapeInfo<TCell> SomeShape(...)
			{
				return ShapeFromBase(PointyHexGrid<TCell>.BeginShape().SomeShape(...));
			}
			\endcode
		*/
		public FlatTriShapeInfo<TCell>ShapeFromBase(PointyHexShapeInfo<TCell> baseShapeInfo)
		{
			ShapeStorageInfo<PointyHexPoint> storageInfo = baseShapeInfo.ShapeStorageStorageInfo;
			var storageRect = storageInfo.storageRect;
			Func<FlatTriPoint, bool> isInside = x => storageInfo.contains(x.BasePoint);
			var shapeInfo = MakeShapeStorageInfo<FlatTriPoint>(storageRect, isInside);
			return new FlatTriShapeInfo<TCell>(shapeInfo); 
		}

		[ShapeMethod]
		public FlatTriShapeInfo<TCell> SingleGroup()
		{
			var rawInfow = MakeShapeStorageInfo<FlatTriPoint>(
				1, 
				1,
				x => x.BasePoint == PointyHexPoint.Zero);

			return new FlatTriShapeInfo<TCell>(rawInfow);
		}
	}

	public partial class PointyTriOp<TCell> : AbstractOp<ShapeStorageInfo<PointyTriPoint>>
	{
		/**
			This creates a shape described by a shape of the base point shape op.
			Typicaly usage:
			\code
			[ShapeMethod]
			public PointyTriShapeInfo<TCell> SomeShape(...)
			{
				return ShapeFromBase(FlatHexGrid<TCell>.BeginShape().SomeShape(...));
			}
			\endcode
		*/
		public PointyTriShapeInfo<TCell>ShapeFromBase(FlatHexShapeInfo<TCell> baseShapeInfo)
		{
			ShapeStorageInfo<FlatHexPoint> storageInfo = baseShapeInfo.ShapeStorageStorageInfo;
			var storageRect = storageInfo.storageRect;
			Func<PointyTriPoint, bool> isInside = x => storageInfo.contains(x.BasePoint);
			var shapeInfo = MakeShapeStorageInfo<PointyTriPoint>(storageRect, isInside);
			return new PointyTriShapeInfo<TCell>(shapeInfo); 
		}

		[ShapeMethod]
		public PointyTriShapeInfo<TCell> SingleGroup()
		{
			var rawInfow = MakeShapeStorageInfo<PointyTriPoint>(
				1, 
				1,
				x => x.BasePoint == FlatHexPoint.Zero);

			return new PointyTriShapeInfo<TCell>(rawInfow);
		}
	}

	public partial class FlatRhombOp<TCell> : AbstractOp<ShapeStorageInfo<FlatRhombPoint>>
	{
		/**
			This creates a shape described by a shape of the base point shape op.
			Typicaly usage:
			\code
			[ShapeMethod]
			public FlatRhombShapeInfo<TCell> SomeShape(...)
			{
				return ShapeFromBase(FlatHexGrid<TCell>.BeginShape().SomeShape(...));
			}
			\endcode
		*/
		public FlatRhombShapeInfo<TCell>ShapeFromBase(FlatHexShapeInfo<TCell> baseShapeInfo)
		{
			ShapeStorageInfo<FlatHexPoint> storageInfo = baseShapeInfo.ShapeStorageStorageInfo;
			var storageRect = storageInfo.storageRect;
			Func<FlatRhombPoint, bool> isInside = x => storageInfo.contains(x.BasePoint);
			var shapeInfo = MakeShapeStorageInfo<FlatRhombPoint>(storageRect, isInside);
			return new FlatRhombShapeInfo<TCell>(shapeInfo); 
		}

		[ShapeMethod]
		public FlatRhombShapeInfo<TCell> SingleGroup()
		{
			var rawInfow = MakeShapeStorageInfo<FlatRhombPoint>(
				1, 
				1,
				x => x.BasePoint == FlatHexPoint.Zero);

			return new FlatRhombShapeInfo<TCell>(rawInfow);
		}
	}

	public partial class PointyRhombOp<TCell> : AbstractOp<ShapeStorageInfo<PointyRhombPoint>>
	{
		/**
			This creates a shape described by a shape of the base point shape op.
			Typicaly usage:
			\code
			[ShapeMethod]
			public PointyRhombShapeInfo<TCell> SomeShape(...)
			{
				return ShapeFromBase(PointyHexGrid<TCell>.BeginShape().SomeShape(...));
			}
			\endcode
		*/
		public PointyRhombShapeInfo<TCell>ShapeFromBase(PointyHexShapeInfo<TCell> baseShapeInfo)
		{
			ShapeStorageInfo<PointyHexPoint> storageInfo = baseShapeInfo.ShapeStorageStorageInfo;
			var storageRect = storageInfo.storageRect;
			Func<PointyRhombPoint, bool> isInside = x => storageInfo.contains(x.BasePoint);
			var shapeInfo = MakeShapeStorageInfo<PointyRhombPoint>(storageRect, isInside);
			return new PointyRhombShapeInfo<TCell>(shapeInfo); 
		}

		[ShapeMethod]
		public PointyRhombShapeInfo<TCell> SingleGroup()
		{
			var rawInfow = MakeShapeStorageInfo<PointyRhombPoint>(
				1, 
				1,
				x => x.BasePoint == PointyHexPoint.Zero);

			return new PointyRhombShapeInfo<TCell>(rawInfow);
		}
	}

	public partial class CairoOp<TCell> : AbstractOp<ShapeStorageInfo<CairoPoint>>
	{
		/**
			This creates a shape described by a shape of the base point shape op.
			Typicaly usage:
			\code
			[ShapeMethod]
			public CairoShapeInfo<TCell> SomeShape(...)
			{
				return ShapeFromBase(PointyHexGrid<TCell>.BeginShape().SomeShape(...));
			}
			\endcode
		*/
		public CairoShapeInfo<TCell>ShapeFromBase(PointyHexShapeInfo<TCell> baseShapeInfo)
		{
			ShapeStorageInfo<PointyHexPoint> storageInfo = baseShapeInfo.ShapeStorageStorageInfo;
			var storageRect = storageInfo.storageRect;
			Func<CairoPoint, bool> isInside = x => storageInfo.contains(x.BasePoint);
			var shapeInfo = MakeShapeStorageInfo<CairoPoint>(storageRect, isInside);
			return new CairoShapeInfo<TCell>(shapeInfo); 
		}

		[ShapeMethod]
		public CairoShapeInfo<TCell> SingleGroup()
		{
			var rawInfow = MakeShapeStorageInfo<CairoPoint>(
				1, 
				1,
				x => x.BasePoint == PointyHexPoint.Zero);

			return new CairoShapeInfo<TCell>(rawInfow);
		}
	}
}
