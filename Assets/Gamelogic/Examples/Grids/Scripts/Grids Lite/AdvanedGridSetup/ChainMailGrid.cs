using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	/**
		Extensions for RectOp that defines three new shapes.
	*/
	public static class RectOpExtensions
	{
		public static RectShapeInfo<TCell> Ring<TCell>(this RectOp<TCell> op)
		{
			return op
				.BeginGroup()	//If you do not use begin group and EndGroup
								// the shapes will behave unexpectedly when combined
								// with other shapes.
				.Rectangle(5, 5)
				.Translate(-1, -1)
				.Difference()
				.Rectangle(3,3)
				.EndGroup(op);
		}

		public static RectShapeInfo<TCell> Chain<TCell>(this RectOp<TCell> op)
		{
			return op
				.BeginGroup()
				.Ring()
				.Translate(3, -3)
				.Union()
				.Ring()
				.Translate(3, +3)
				.Union()
				.Ring()
				.Translate(3, -3)
				.Union()
				.Ring()
				.Translate(3, +3)
				.Union()
				.Ring()
				.Translate(3, -3)
				.Union()
				.Ring()
				.EndGroup(op);
		}

		public static RectShapeInfo<TCell> ChainMail<TCell>(this RectOp<TCell> op)
		{
			return op
				.BeginGroup()
				.Chain()
				.Translate(0, 6)
				.Union()
				.Chain()
				.Translate(0, 6)
				.Union()
				.Chain()
				.Translate(0, 6)
				.Union()
				.Chain()
				.Translate(0, 6)
				.Union()
				.Chain()
				.EndGroup(op);
		}
	}

	public class ChainMailGrid : CustomGridBuilder
	{
		public override IGrid<TCell, TPoint> MakeGrid<TCell, TPoint>()
		{
			if (typeof (TPoint) == typeof (RectPoint))
			{
				var grid = RectGrid<TCell>
					.BeginShape()
					.ChainMail() //You can now chain the newly defined method to BeginShape
					.EndShape();

				return (IGrid<TCell, TPoint>) grid;
			}

			Debug.LogError("<color=blue><b>" + GetType() + "</b></color> does not support grids for points of type " +
			               typeof (TPoint));

			return null;
		}
	}
}