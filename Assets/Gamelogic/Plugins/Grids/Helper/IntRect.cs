//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

namespace Gamelogic.Grids
{
	/**
		A rectangle where coordinates are non-negative integers (that is, the corners are ArrayPoints). 

		This class is useful for implementing storage operations for shapes.

		
		
		@version1_0

		@ingroup Helpers
	*/
	public struct IntRect
	{
		public readonly ArrayPoint offset;
		public readonly ArrayPoint dimensions;

		/**
			Gives the point just outside the rectangle.
		*/
		public ArrayPoint RightEnd 
		{
			get
			{
				return offset + dimensions;
			}
		}

		public IntRect(ArrayPoint offset, ArrayPoint dimensions)
		{
			if (dimensions.X < 0 || dimensions.Y < 0)
			{
				this.dimensions = ArrayPoint.Zero;
			}
			else
			{
				this.dimensions = dimensions;
			}

			this.offset = offset;
		}

		public IntRect Translate(ArrayPoint point)
		{
			return new IntRect(offset + point, dimensions);
		}

		public IntRect Subtract(ArrayPoint point)
		{
			return new IntRect(offset - point, dimensions);
		}

		public IntRect Intersection(IntRect otherRect)
		{
			var left = ArrayPoint.Max(offset, otherRect.offset);
			var right = ArrayPoint.Min(RightEnd, otherRect.RightEnd);

			return new IntRect(left, right - left);
		}

		public IntRect Union(IntRect otherRect)
		{
			var left = ArrayPoint.Min(offset, otherRect.offset);
			var right = ArrayPoint.Max(RightEnd, otherRect.RightEnd);

			return new IntRect(left, right - left);
		}

		public IntRect Difference(IntRect otherRect)
		{
			// Not tight
			return this;
		}

		public IntRect IncDimensions(ArrayPoint increase)
		{
			return new IntRect(offset, dimensions + increase);
		}
	}
}