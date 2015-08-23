//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System;

namespace Gamelogic.Grids
{
	/**
		This is the base class of all ShapeInfo classes for spliced grids.
	
		
		
		@version1_0

		@ingroup Scaffolding
	*/
	public abstract class AbstractSplicedShapeInfo<TShapeInfo, TGrid, TPoint, TVectorPoint, TShapeOp> :
		AbstractShapeInfo<TShapeInfo, TGrid, TPoint, TVectorPoint, TShapeOp>

		where TShapeInfo : IShapeInfo<TShapeInfo, TGrid, TPoint, TVectorPoint, TShapeOp>
		where TGrid : IGridSpace<TPoint>
		where TPoint : ISplicedVectorPoint<TPoint, TVectorPoint>, ISplicedPoint<TPoint, TVectorPoint>
		where TVectorPoint : IVectorPoint<TVectorPoint>, IGridPoint<TVectorPoint>
	{
		#region Construction
		/**
		Constructs a new instance wih the given sotrage information.
	*/

		protected AbstractSplicedShapeInfo(ShapeStorageInfo<TPoint> info) :
			base(info)
		{
		}
		#endregion

		#region Index Transforms
		/**
		This increments the index part of the spliced points for this shape.
	*/
		public TShapeInfo IncIndex(int indexOffset)
		{
			Func<TPoint, bool> newIsInside = x => shapeStorageStorageInfo.contains(x.DecIndex(indexOffset));

			return MakeShapeInfo(new ShapeStorageInfo<TPoint>(shapeStorageStorageInfo.storageRect, newIsInside));
		}

		/**
			This inverts the index part of the spliced points for this part.
		*/
		public TShapeInfo InvertIndex()
		{
			Func<TPoint, bool> newIsInside = x => shapeStorageStorageInfo.contains(x.InvertIndex());

			return MakeShapeInfo(new ShapeStorageInfo<TPoint>(shapeStorageStorageInfo.storageRect, newIsInside));
		}
		#endregion
	}
}
