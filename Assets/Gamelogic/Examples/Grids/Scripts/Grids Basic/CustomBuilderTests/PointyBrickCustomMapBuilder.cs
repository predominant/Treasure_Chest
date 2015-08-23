using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	public class PointyBrickCustomMapBuilder : CustomMapBuilder
	{
		public Vector2 cellDimensions;

		public override WindowedMap<TPoint> CreateWindowedMap<TPoint>()
		{
			if (typeof (TPoint) == typeof (PointyHexPoint))
			{
				return (WindowedMap<TPoint>) (object) new PointyBrickMap(cellDimensions).WithWindow(new Rect(0, 0, 0, 0));
			}

			return null;
		}
	}
}