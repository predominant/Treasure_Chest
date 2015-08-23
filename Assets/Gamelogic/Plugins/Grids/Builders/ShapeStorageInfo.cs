//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System;

namespace Gamelogic.Grids
{
	/**
		This class handles how the cells of a grid is represented in memeory.

		This class is used by shape info classes.
	
		@version1_0

		@ingroup Scaffolding
	*/
	[Immutable]
	public class ShapeStorageInfo<TPoint>
	{
		#region Fields
		public readonly IntRect storageRect;
		public readonly Func<TPoint, bool> contains;
		#endregion

		#region Constructors
		public ShapeStorageInfo(int width, int height, Func<TPoint, bool> contains) :
			this(new IntRect(ArrayPoint.Zero, new ArrayPoint(width, height)), contains)
		{}

		public ShapeStorageInfo(IntRect storageRect, Func<TPoint, bool> contains)
		{
			this.storageRect = storageRect;
			this.contains = contains;
		}
		#endregion

		#region Set Operators
		public ShapeStorageInfo<TPoint> Intersection(ShapeStorageInfo<TPoint> other)
		{
			Func<TPoint, bool> newIsInside = x => contains(x) && other.contains(x);
			var newStorageRect = storageRect.Intersection(other.storageRect);

			return new ShapeStorageInfo<TPoint>(newStorageRect, newIsInside);
		}

		public ShapeStorageInfo<TPoint> Union(ShapeStorageInfo<TPoint> other)
		{
			Func<TPoint, bool> newIsInside = x => contains(x) || other.contains(x);
			var newStorageRect = storageRect.Union(other.storageRect);

			return new ShapeStorageInfo<TPoint>(newStorageRect, newIsInside);
		}

		public ShapeStorageInfo<TPoint> Difference(ShapeStorageInfo<TPoint> other)
		{
			Func<TPoint, bool> newIsInside = x => contains(x) && !other.contains(x);
			var newStorageRect = storageRect.Difference(other.storageRect);

			return new ShapeStorageInfo<TPoint>(newStorageRect, newIsInside);
		}

		public ShapeStorageInfo<TPoint> SymmetricDifference(ShapeStorageInfo<TPoint> other)
		{
			return Union(other);
		}
		#endregion
	}
}