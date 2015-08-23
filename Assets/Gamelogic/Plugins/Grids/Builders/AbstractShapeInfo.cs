//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//
using System;

namespace Gamelogic.Grids
{
	/**
		The base class of shape info classes
		@version1_0
		@ingroup Scaffolding
	*/
	public abstract class AbstractShapeInfo<TShapeInfo, TGrid, TPoint, TVectorPoint, TShapeOp> : IShapeInfo<TShapeInfo, TGrid, TPoint, TVectorPoint, TShapeOp>
		where TShapeInfo : IShapeInfo<TShapeInfo, TGrid, TPoint, TVectorPoint, TShapeOp>
		where TGrid : IGridSpace<TPoint>
		where TPoint : ISplicedVectorPoint<TPoint, TVectorPoint>, IGridPoint<TPoint>
		where TVectorPoint : IVectorPoint<TVectorPoint>
	{
		#region Fields
		protected ShapeStorageInfo<TPoint> shapeStorageStorageInfo;
		#endregion

		#region Properties
		public ShapeStorageInfo<TPoint> ShapeStorageStorageInfo
		{
			get
			{
				return shapeStorageStorageInfo;
			}
		}
		#endregion

		#region Construction
		protected AbstractShapeInfo(ShapeStorageInfo<TPoint> info)
		{
			shapeStorageStorageInfo = info;
		}
		#endregion

		#region Interface
		public TShapeOp If(bool condition)
		{
			return MakeOp(shapeStorageStorageInfo, (x, y) => condition ? x : y);
		}

		public TShapeOp Intersection()
		{
			return MakeOp(shapeStorageStorageInfo, (x, y) => x.Intersection(y));
		}

		public TShapeOp Union()
		{
			return MakeOp(shapeStorageStorageInfo, (x, y) => x.Union(y));
		}

		public TShapeOp Difference()
		{
			return MakeOp(shapeStorageStorageInfo, (x, y) => x.Difference(y));
		}

		/**
			The resulting shape is the union of the first and immediately following shape, minus
			the intersection of the two shapes.

			@version1_1
		*/
		public TShapeOp SymmetricDifference()
		{
			return MakeOp(shapeStorageStorageInfo, (x, y) => x.SymmetricDifference(y));
		}

		public TShapeInfo Translate(int x, int y)
		{
			return Translate(MakePoint(x, y));
		}

		public TShapeInfo IfTranslate(bool condition, int x, int y)
		{
			return condition ? Translate(MakePoint(x, y)) : MakeShapeInfo(shapeStorageStorageInfo);
		}
		
		//Assumption:
		// ArrayFromGridPoint(p1 + offset) == ArrayFromGridPoint(p1) + ArrayFromGridPoint(offset)
		public TShapeInfo Translate(TVectorPoint offset)
		{
			Func<TPoint, bool> newIsInside = x => shapeStorageStorageInfo.contains(x.Subtract(offset));

			var newStorageRect = shapeStorageStorageInfo.storageRect
				.Translate(ArrayPointFromGridPoint(offset));
													  
			return MakeShapeInfo(new ShapeStorageInfo<TPoint>(newStorageRect, newIsInside));
		}

public TShapeInfo Filter(Func<TPoint, bool> filter)
{
	Func<TPoint, bool> newIsInside = 
		x => shapeStorageStorageInfo.contains(x) && filter(x);

	var newStorageRect = shapeStorageStorageInfo.storageRect;

	return MakeShapeInfo(new ShapeStorageInfo<TPoint>(newStorageRect, newIsInside));
}

		public TGrid EndShape()
		{
			var gridOffset = GridPointFromArrayPoint(shapeStorageStorageInfo.storageRect.offset);

			return MakeShape(
				shapeStorageStorageInfo.storageRect.dimensions.X,
				shapeStorageStorageInfo.storageRect.dimensions.Y,
				x => shapeStorageStorageInfo.contains(x.Translate(gridOffset)),
				gridOffset);
		}
		#endregion
		
		#region Abstract Implementation
		abstract protected ArrayPoint ArrayPointFromGridPoint(TVectorPoint point);
		abstract protected TVectorPoint GridPointFromArrayPoint(ArrayPoint point);
		abstract protected TVectorPoint MakePoint(int x, int y);

		abstract protected TShapeOp MakeOp(
			ShapeStorageInfo<TPoint> shapeInfo,
			Func<
				ShapeStorageInfo<TPoint>,
				ShapeStorageInfo<TPoint>,
				ShapeStorageInfo<TPoint>> combineInfo);

		abstract protected TShapeInfo MakeShapeInfo(
			ShapeStorageInfo<TPoint> shapeStorageInfo);

		abstract protected TGrid MakeShape(int x, int y, Func<TPoint, bool> isInside, TVectorPoint offset);
		#endregion
	}
}