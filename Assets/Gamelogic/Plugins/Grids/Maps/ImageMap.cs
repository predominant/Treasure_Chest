//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2014 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Map that maps a grid to an image; useful for texturing a grid with a single image, or 
		point an image with a grid.

		@version1_8
	*/
	[Experimental]
	public class ImageMap<TPoint>
		where TPoint : IGridPoint<TPoint>
	{
		private readonly IMap<TPoint> map;
		private Rect imageRect;
		private Vector2 gridDimensions;
		private Vector2 anchorPoint;

		public ImageMap(Rect imageRect, IGridSpace<TPoint> grid, IMap<TPoint> map)
		{
			this.imageRect = imageRect;
			this.map = map;

			gridDimensions = map.CalcGridDimensions(grid);
			anchorPoint = map.GetAnchorTranslation();
		}

		public Vector2 this[TPoint point]
		{
			get
			{
				var worldPoint = map[point];

				var x = (worldPoint.x - anchorPoint.x)/gridDimensions.x*imageRect.width + imageRect.xMin;
				var y = (worldPoint.y - anchorPoint.y)/gridDimensions.y*imageRect.height + imageRect.yMin;

				return new Vector2(x, y);
			}
		}

		public TPoint this[Vector2 point]
		{
			get
			{
				var x = (point.x - imageRect.xMin)/imageRect.width*gridDimensions.x + anchorPoint.x;
				var y = (point.y - imageRect.yMin)/imageRect.height*gridDimensions.y + anchorPoint.y;

				var worldPoint = new Vector2(x, y);

				return map[worldPoint];
			}
		}

		public Vector2 GetCellDimensions(TPoint point)
		{
			var cellDimensions = map.GetCellDimensions(point);
			var x = cellDimensions.x/gridDimensions.x*imageRect.width;
			var y = cellDimensions.y/gridDimensions.y*imageRect.height;

			return new Vector2(x, y);
		}

		public Vector2 GetCellScale(TPoint point)
		{
			var cellDimensions = GetCellDimensions(point);

			return new Vector2(1f/cellDimensions.x, 1f/cellDimensions.y);
		}
	}
}
