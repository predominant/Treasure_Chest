//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		A WindowedMap maps grid points relative to a window (a rectangle in world space)
		and provides methods that makes alignment with respect to this window simple.
	
		@copyright Gamelogic
		
		@version1_0

		@ingroup Interface
	*/

	[Immutable]
	public class WindowedMap<TPoint> : CompoundMap<TPoint>
		where TPoint : IGridPoint<TPoint>
	{
		private Rect window;

		public WindowedMap(IMap<TPoint> leftMap, Rect window) :
			base(leftMap, x => x, x => x)
		{
			this.window = window;
		}

		public IMap<TPoint> AlignLeft(IGridSpace<TPoint> grid)
		{
			var anchorPosition = leftMap.CalcBottomLeft(grid);

			return TranslateX(window.x - anchorPosition.x + leftMap.GetAnchorTranslation().x);
		}

		public IMap<TPoint> AlignRight(IGridSpace<TPoint> grid)
		{
			var gridSize = leftMap.CalcGridDimensions(grid);
			var anchorPosition = leftMap.CalcBottomLeft(grid);

			return TranslateX(window.x + window.width - gridSize.x - anchorPosition.x + leftMap.GetAnchorTranslation().x);
		}

		public IMap<TPoint> AlignTop(IGridSpace<TPoint> grid)
		{
			var gridSize = leftMap.CalcGridDimensions(grid);
			var anchorPosition = leftMap.CalcBottomLeft(grid);

			return TranslateY(window.y + window.height - gridSize.y - anchorPosition.y + leftMap.GetAnchorTranslation().y);
		}

		public IMap<TPoint> AlignBottom(IGridSpace<TPoint> grid)
		{
			var anchorPosition = leftMap.CalcBottomLeft(grid);

			return TranslateY(window.y - anchorPosition.y + leftMap.GetAnchorTranslation().y);
		}

		public IMap<TPoint> AlignMiddle(IGridSpace<TPoint> grid)
		{
			var gridSize = leftMap.CalcGridDimensions(grid);
			var anchorPosition = leftMap.CalcBottomLeft(grid);
			var offsetY = window.y + (window.height - gridSize.y)/2 - anchorPosition.y + leftMap.GetAnchorTranslation().y;

			return TranslateY(offsetY);
		}



		public IMap<TPoint> AlignCenter(IGridSpace<TPoint> grid)
		{
			var gridSize = leftMap.CalcGridDimensions(grid);
			var anchorPosition = leftMap.CalcBottomLeft(grid);

			var offsetX = window.x + (window.width - gridSize.x)/2 - anchorPosition.x + leftMap.GetAnchorTranslation().x;

			return TranslateX(offsetX);
		}

		public IMap<TPoint> AlignTopLeft(IGridSpace<TPoint> grid)
		{
			return
				AlignLeft(grid)
					.WithWindow(window)
					.AlignTop(grid);
		}

		public IMap<TPoint> AlignTopCenter(IGridSpace<TPoint> grid)
		{
			return
				AlignCenter(grid)
					.WithWindow(window)
					.AlignTop(grid);
		}

		public IMap<TPoint> AlignTopRight(IGridSpace<TPoint> grid)
		{
			return
				AlignRight(grid)
					.WithWindow(window)
					.AlignTop(grid);
		}

		public IMap<TPoint> AlignMiddleLeft(IGridSpace<TPoint> grid)
		{
			return
				AlignMiddle(grid)
					.WithWindow(window)
					.AlignLeft(grid);
		}



		public IMap<TPoint> AlignMiddleRight(IGridSpace<TPoint> grid)
		{
			return
				AlignMiddle(grid)
					.WithWindow(window)
					.AlignRight(grid);
		}



		public IMap<TPoint> AlignBottomCenter(IGridSpace<TPoint> grid)
		{
			return
				AlignCenter(grid)
					.WithWindow(window)
					.AlignBottom(grid);
		}

		public IMap<TPoint> AlignMiddleCenter(IGridSpace<TPoint> grid)
		{
			return
				AlignCenter(grid)
					.WithWindow(window)
					.AlignMiddle(grid);
		}

		public IMap<TPoint> AlignBottomLeft(IGridSpace<TPoint> grid)
		{
			return
				AlignLeft(grid)
					.WithWindow(window)
					.AlignBottom(grid);
		}

		public IMap<TPoint> AlignBottomRight(IGridSpace<TPoint> grid)
		{
			return
				AlignRight(grid)
					.WithWindow(window)
					.AlignBottom(grid);
		}

		/**
			@version1_8

			Stretches a grid over the given rectangle.
		*/
		public IMap<TPoint> Stretch(IGridSpace<TPoint> grid)
		{
			var gridDimensions = CalcGridDimensions(grid);
			var anchorPoint = GetAnchorTranslation();

			return
				Translate(-anchorPoint)
					.Scale((window.width/gridDimensions.x), (window.height/gridDimensions.y))
					.Translate(window.xMin, window.yMin);
		}
	}
}