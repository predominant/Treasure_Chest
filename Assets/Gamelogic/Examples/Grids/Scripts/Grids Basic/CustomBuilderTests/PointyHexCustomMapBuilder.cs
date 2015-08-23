using UnityEngine;

//TODO - make a more exciting example!

namespace Gamelogic.Grids.Examples
{
	public class PointyHexCustomMapBuilder : CustomMapBuilder
	{
		public Vector2 cellDimensions;
		/**
		@implementers note:
			Check the type; if it is "your" type, return the map.
			Otherwise return null.
	*/

		public override WindowedMap<TPoint> CreateWindowedMap<TPoint>()
		{
			if (typeof (TPoint) == typeof (PointyHexPoint))
			{
				return (WindowedMap<TPoint>) (object) new PointyHexMap(cellDimensions)
					.ReflectAboutY()
					.TranslateY(200)
					.Scale(0.5f)
					.RotateAround(45, Vector2.zero)
					//TODO: why does this not work properly
					.WithWindow(new Rect(0, 0, 0, 0));
			}

			return null;
		}
	}
}