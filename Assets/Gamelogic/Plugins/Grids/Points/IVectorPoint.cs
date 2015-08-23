//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

namespace Gamelogic.Grids
{
	/**
		A VectorPoint is a point that is also an algebraic vector. 

		
		
		@version1_0

		@ingroup Interface
	*/
	public interface IVectorPoint<TPoint> : ISplicedVectorPoint<TPoint, TPoint>
		where TPoint : IVectorPoint<TPoint>
	{
		int X { get; }
		int Y { get; }

		/**
			Scales this vector by the given amount.

				v.ScaleUp(1)
				v.ScaleUp(n) ==  v.ScaleUp(n - 1).Translate(v)
		*/
		TPoint ScaleDown(int r);
		TPoint ScaleUp(int r);

		/**
			Integer divides a point by another point component by component. 
			Remainders are always positive.
				(-5, 5) Div (2, 2) == (-3, 2)

			@version1_7
		*/
		TPoint Div(TPoint other);

		/**
			Integer divides a point component by component and returns the remainder. 
			Remainders are always positive.
				(-5, 5) Mod (2, 2) == (1, 1)

			@version1_7
		*/
		TPoint Mod(TPoint other);

		/**
			Multiplies two points component by component. 
				(-5, 5) Mul (2, 2) == (-10, 10)

			@version1_7
		*/
		TPoint Mul(TPoint other);

		int Magnitude();
	}
}
