//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

namespace Gamelogic.Grids
{
	/**
		A partial vector point is a point that can be translated by "adding" a vector point.

		Partial vectors can be seen as a pair, one of which is a vector (of type TVectorPoint).
		All the operations actually operate on the vector of this pair.

		Partial vector points are used in SplicedGrids, where the second of the pair is an index 
		that denotes the sub-cell. For example, for a tri point, the vector is a hex point, and
		the index denotes whether the point refers to the up or down triangle.

		\param TPoint The type that implements this interface.
		\param TVectorPoint The type used to translate TPoints.

		
		
		@version1_0

		@ingroup Interface
	*/
	public interface ISplicedVectorPoint<TPoint, TVectorPoint>
		where TPoint : ISplicedVectorPoint<TPoint, TVectorPoint>
		where TVectorPoint : IVectorPoint<TVectorPoint>
	{
		/**
			Translate this point by the given vector.
		*/
		TPoint Translate(TVectorPoint vector);

		/**
			Returns a new point with the vector component negated.
		*/
		TPoint Negate();

		/**
			Translates this point by the negation of the given vector.
		*/
		TPoint Subtract(TVectorPoint vector);

		/**
			If a spliced vectors u and v has base vector B and index I, 
			then this operation is the same as

			new SplicedVector(u.B.Translate(v.B), (u.I + v.I) % SpliceCount))
		*/
		TPoint MoveBy(TPoint splicedVector);

		/**
			If a spliced vectors u and v has base vector B and index I, 
			then this operation is the same as

			new SplicedVector(u.B.Subtract(v.B), (SpliceCount + u.I - v.I) % SpliceCount))
		*/
		TPoint MoveBackBy(TPoint splicedVector);
	}
}