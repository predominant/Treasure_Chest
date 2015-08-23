using System;

namespace Gamelogic.Grids
{
	/**
		Provides the implementation for AbstractShapeInfo to be used with LineGrid.

		@link_constructing_grids

		@version1_8
		@ingroup BuilderInterface
	*/
	public class LineShapeInfo<TCell>
		: AbstractShapeInfo <LineShapeInfo<TCell>, LineGrid<TCell>, LinePoint, LinePoint, LineOp<TCell>> 
	{
		public LineShapeInfo(ShapeStorageInfo<LinePoint> info):
			base(info)
		{
		}

		/**
			Only call this method from within a RectOp method (usually, in client code, 
			this will be in an extension).

			@param op the operator on which this shape is defined.
			@version1_1
		*/
		public LineShapeInfo<TCell> EndGroup(LineOp<TCell> op)
		{
			var info = op.combineShapeInfo(op.leftShapeInfo, ShapeStorageStorageInfo);
			return new LineShapeInfo<TCell>(info);
		}

		protected override LinePoint MakePoint(int x, int y)
		{
			return new LinePoint(x);
		}

		protected override LineOp<TCell> MakeOp(
			ShapeStorageInfo<LinePoint> shapeInfo, 
			Func<
				ShapeStorageInfo<LinePoint>,
				ShapeStorageInfo<LinePoint>,
				ShapeStorageInfo<LinePoint>> combineInfo)
		{
			return new LineOp<TCell>(shapeInfo,	combineInfo);
		}

		protected override LineShapeInfo<TCell> MakeShapeInfo(
			ShapeStorageInfo<LinePoint> shapeStorageInfo)
		{
			return new LineShapeInfo<TCell>(shapeStorageInfo);
		}

		protected override LinePoint GridPointFromArrayPoint(ArrayPoint point)
		{
			return point.X;
		}

		protected override ArrayPoint ArrayPointFromGridPoint(LinePoint point)
		{
			return new ArrayPoint(point, 0);
		}

		protected override LineGrid<TCell> MakeShape(int x, int y, Func<LinePoint, bool> isInside, LinePoint offset)
		{
			return new LineGrid<TCell>(x, isInside, (p => p.Translate(offset)), (p => p.Subtract(offset)));
		}
	}
}

