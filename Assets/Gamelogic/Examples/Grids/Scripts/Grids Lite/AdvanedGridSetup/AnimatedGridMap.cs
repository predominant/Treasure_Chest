using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	public class AnimatedGridMap : CustomMapBuilder
	{
		public override WindowedMap<TPoint> CreateWindowedMap<TPoint>()
		{

			if (typeof (TPoint) == typeof (RectPoint))
			{
				var map = new RectMap(new Vector2(200, 200))
					.Animate((x, t) => x.Rotate(30*t), (x, t) => x.Rotate(-30*t))
					.Animate((x, t) => x + new Vector2(75*Mathf.Sin(t/5.0f), 0),
						(x, t) => x - new Vector2(75*Mathf.Sin(t/5.0f), 0))
					.Animate((x, t) => x*(1.5f + 0.5f*Mathf.Sin(t*7)),
						(x, t) => x/(1.5f + 0.5f*Mathf.Sin(t*7)))
					.WithWindow(ExampleUtils.ScreenRect);

				return (WindowedMap<TPoint>) (object) map;
			}

			return null;
		}
	}
}