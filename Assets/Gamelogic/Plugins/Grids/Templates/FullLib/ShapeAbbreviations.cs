//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

// Auto-generated File

using System;

namespace Gamelogic.Grids
{
	public partial class RectGrid<TCell>
	{
		/**
			\copydoc RectOp<TCell>::FixedWidth
		*/
		public static RectGrid<TCell> FixedWidth(Int32 width, Int32 cellCount)
		{
			return BeginShape().FixedWidth(width, cellCount).EndShape();
		}

		/**
			\copydoc RectOp<TCell>::FixedHeight
		*/
		public static RectGrid<TCell> FixedHeight(Int32 height, Int32 cellCount)
		{
			return BeginShape().FixedHeight(height, cellCount).EndShape();
		}

		/**
			\copydoc RectOp<TCell>::Rectangle
		*/
		public static RectGrid<TCell> Rectangle(Int32 width, Int32 height)
		{
			return BeginShape().Rectangle(width, height).EndShape();
		}

		/**
			\copydoc RectOp<TCell>::Circle
		*/
		public static RectGrid<TCell> Circle(Int32 radius)
		{
			return BeginShape().Circle(radius).EndShape();
		}

		/**
			\copydoc RectOp<TCell>::Parallelogram
		*/
		public static RectGrid<TCell> Parallelogram(Int32 width, Int32 height)
		{
			return BeginShape().Parallelogram(width, height).EndShape();
		}

		/**
			\copydoc RectOp<TCell>::CheckerBoard
		*/
		public static RectGrid<TCell> CheckerBoard(Int32 width, Int32 height)
		{
			return BeginShape().CheckerBoard(width, height).EndShape();
		}

		/**
			\copydoc RectOp<TCell>::CheckerBoard
		*/
		public static RectGrid<TCell> CheckerBoard(Int32 width, Int32 height, Boolean includesOrigin)
		{
			return BeginShape().CheckerBoard(width, height, includesOrigin).EndShape();
		}

		/**
			\copydoc RectOp<TCell>::Default
		*/
		public static RectGrid<TCell> Default(Int32 width, Int32 height)
		{
			return BeginShape().Default(width, height).EndShape();
		}

		/**
			\copydoc RectOp<TCell>::Single
		*/
		public static RectGrid<TCell> Single()
		{
			return BeginShape().Single().EndShape();
		}

	}
	public partial class DiamondGrid<TCell>
	{
		/**
			\copydoc DiamondOp<TCell>::Diamond
		*/
		public static DiamondGrid<TCell> Diamond(Int32 side)
		{
			return BeginShape().Diamond(side).EndShape();
		}

		/**
			\copydoc DiamondOp<TCell>::Parallelogram
		*/
		public static DiamondGrid<TCell> Parallelogram(Int32 width, Int32 height)
		{
			return BeginShape().Parallelogram(width, height).EndShape();
		}

		/**
			\copydoc DiamondOp<TCell>::Rectangle
		*/
		public static DiamondGrid<TCell> Rectangle(Int32 width, Int32 height)
		{
			return BeginShape().Rectangle(width, height).EndShape();
		}

		/**
			\copydoc DiamondOp<TCell>::ThinRectangle
		*/
		public static DiamondGrid<TCell> ThinRectangle(Int32 width, Int32 height)
		{
			return BeginShape().ThinRectangle(width, height).EndShape();
		}

		/**
			\copydoc DiamondOp<TCell>::FatRectangle
		*/
		public static DiamondGrid<TCell> FatRectangle(Int32 width, Int32 height)
		{
			return BeginShape().FatRectangle(width, height).EndShape();
		}

		/**
			\copydoc DiamondOp<TCell>::Default
		*/
		public static DiamondGrid<TCell> Default(Int32 width, Int32 height)
		{
			return BeginShape().Default(width, height).EndShape();
		}

		/**
			\copydoc DiamondOp<TCell>::Single
		*/
		public static DiamondGrid<TCell> Single()
		{
			return BeginShape().Single().EndShape();
		}

	}
	public partial class PointyHexGrid<TCell>
	{
		/**
			\copydoc PointyHexOp<TCell>::Rectangle
		*/
		public static PointyHexGrid<TCell> Rectangle(Int32 width, Int32 height)
		{
			return BeginShape().Rectangle(width, height).EndShape();
		}

		/**
			\copydoc PointyHexOp<TCell>::Hexagon
		*/
		public static PointyHexGrid<TCell> Hexagon(Int32 side)
		{
			return BeginShape().Hexagon(side).EndShape();
		}

		/**
			\copydoc PointyHexOp<TCell>::Hexagon
		*/
		public static PointyHexGrid<TCell> Hexagon(PointyHexPoint centre, Int32 side)
		{
			return BeginShape().Hexagon(centre, side).EndShape();
		}

		/**
			\copydoc PointyHexOp<TCell>::Parallelogram
		*/
		public static PointyHexGrid<TCell> Parallelogram(Int32 width, Int32 height)
		{
			return BeginShape().Parallelogram(width, height).EndShape();
		}

		/**
			\copydoc PointyHexOp<TCell>::UpTriangle
		*/
		public static PointyHexGrid<TCell> UpTriangle(Int32 side)
		{
			return BeginShape().UpTriangle(side).EndShape();
		}

		/**
			\copydoc PointyHexOp<TCell>::DownTriangle
		*/
		public static PointyHexGrid<TCell> DownTriangle(Int32 side)
		{
			return BeginShape().DownTriangle(side).EndShape();
		}

		/**
			\copydoc PointyHexOp<TCell>::Diamond
		*/
		public static PointyHexGrid<TCell> Diamond(Int32 side)
		{
			return BeginShape().Diamond(side).EndShape();
		}

		/**
			\copydoc PointyHexOp<TCell>::ThinRectangle
		*/
		public static PointyHexGrid<TCell> ThinRectangle(Int32 width, Int32 height)
		{
			return BeginShape().ThinRectangle(width, height).EndShape();
		}

		/**
			\copydoc PointyHexOp<TCell>::FatRectangle
		*/
		public static PointyHexGrid<TCell> FatRectangle(Int32 width, Int32 height)
		{
			return BeginShape().FatRectangle(width, height).EndShape();
		}

		/**
			\copydoc PointyHexOp<TCell>::Default
		*/
		public static PointyHexGrid<TCell> Default(Int32 width, Int32 height)
		{
			return BeginShape().Default(width, height).EndShape();
		}

		/**
			\copydoc PointyHexOp<TCell>::Single
		*/
		public static PointyHexGrid<TCell> Single()
		{
			return BeginShape().Single().EndShape();
		}

	}
	public partial class FlatHexGrid<TCell>
	{
		/**
			\copydoc FlatHexOp<TCell>::Rectangle
		*/
		public static FlatHexGrid<TCell> Rectangle(Int32 width, Int32 height)
		{
			return BeginShape().Rectangle(width, height).EndShape();
		}

		/**
			\copydoc FlatHexOp<TCell>::FatRectangle
		*/
		public static FlatHexGrid<TCell> FatRectangle(Int32 width, Int32 height)
		{
			return BeginShape().FatRectangle(width, height).EndShape();
		}

		/**
			\copydoc FlatHexOp<TCell>::ThinRectangle
		*/
		public static FlatHexGrid<TCell> ThinRectangle(Int32 width, Int32 height)
		{
			return BeginShape().ThinRectangle(width, height).EndShape();
		}

		/**
			\copydoc FlatHexOp<TCell>::Hexagon
		*/
		public static FlatHexGrid<TCell> Hexagon(Int32 side)
		{
			return BeginShape().Hexagon(side).EndShape();
		}

		/**
			\copydoc FlatHexOp<TCell>::Hexagon
		*/
		public static FlatHexGrid<TCell> Hexagon(FlatHexPoint centre, Int32 side)
		{
			return BeginShape().Hexagon(centre, side).EndShape();
		}

		/**
			\copydoc FlatHexOp<TCell>::LeftTriangle
		*/
		public static FlatHexGrid<TCell> LeftTriangle(Int32 side)
		{
			return BeginShape().LeftTriangle(side).EndShape();
		}

		/**
			\copydoc FlatHexOp<TCell>::RightTriangle
		*/
		public static FlatHexGrid<TCell> RightTriangle(Int32 side)
		{
			return BeginShape().RightTriangle(side).EndShape();
		}

		/**
			\copydoc FlatHexOp<TCell>::Parallelogram
		*/
		public static FlatHexGrid<TCell> Parallelogram(Int32 width, Int32 height)
		{
			return BeginShape().Parallelogram(width, height).EndShape();
		}

		/**
			\copydoc FlatHexOp<TCell>::Diamond
		*/
		public static FlatHexGrid<TCell> Diamond(Int32 side)
		{
			return BeginShape().Diamond(side).EndShape();
		}

		/**
			\copydoc FlatHexOp<TCell>::Default
		*/
		public static FlatHexGrid<TCell> Default(Int32 width, Int32 height)
		{
			return BeginShape().Default(width, height).EndShape();
		}

		/**
			\copydoc FlatHexOp<TCell>::Single
		*/
		public static FlatHexGrid<TCell> Single()
		{
			return BeginShape().Single().EndShape();
		}

	}
	public partial class PointyTriGrid<TCell>
	{
		/**
			\copydoc PointyTriOp<TCell>::Rectangle
		*/
		public static PointyTriGrid<TCell> Rectangle(Int32 width, Int32 height)
		{
			return BeginShape().Rectangle(width, height).EndShape();
		}

		/**
			\copydoc PointyTriOp<TCell>::FatRectangle
		*/
		public static PointyTriGrid<TCell> FatRectangle(Int32 width, Int32 height)
		{
			return BeginShape().FatRectangle(width, height).EndShape();
		}

		/**
			\copydoc PointyTriOp<TCell>::ThinRectangle
		*/
		public static PointyTriGrid<TCell> ThinRectangle(Int32 width, Int32 height)
		{
			return BeginShape().ThinRectangle(width, height).EndShape();
		}

		/**
			\copydoc PointyTriOp<TCell>::Hexagon
		*/
		public static PointyTriGrid<TCell> Hexagon(Int32 side)
		{
			return BeginShape().Hexagon(side).EndShape();
		}

		/**
			\copydoc PointyTriOp<TCell>::RightTriangle
		*/
		public static PointyTriGrid<TCell> RightTriangle(Int32 side)
		{
			return BeginShape().RightTriangle(side).EndShape();
		}

		/**
			\copydoc PointyTriOp<TCell>::LeftTriangle
		*/
		public static PointyTriGrid<TCell> LeftTriangle(Int32 side)
		{
			return BeginShape().LeftTriangle(side).EndShape();
		}

		/**
			\copydoc PointyTriOp<TCell>::ParallelogramXY
		*/
		public static PointyTriGrid<TCell> ParallelogramXY(Int32 width, Int32 height)
		{
			return BeginShape().ParallelogramXY(width, height).EndShape();
		}

		/**
			\copydoc PointyTriOp<TCell>::ParallelogramXZ
		*/
		public static PointyTriGrid<TCell> ParallelogramXZ(Int32 width, Int32 height)
		{
			return BeginShape().ParallelogramXZ(width, height).EndShape();
		}

		/**
			\copydoc PointyTriOp<TCell>::Star
		*/
		public static PointyTriGrid<TCell> Star(Int32 side)
		{
			return BeginShape().Star(side).EndShape();
		}

		/**
			\copydoc PointyTriOp<TCell>::Default
		*/
		public static PointyTriGrid<TCell> Default(Int32 width, Int32 height)
		{
			return BeginShape().Default(width, height).EndShape();
		}

		/**
			\copydoc PointyTriOp<TCell>::Single
		*/
		public static PointyTriGrid<TCell> Single()
		{
			return BeginShape().Single().EndShape();
		}

		/**
			\copydoc PointyTriOp<TCell>::SingleGroup
		*/
		public static PointyTriGrid<TCell> SingleGroup()
		{
			return BeginShape().SingleGroup().EndShape();
		}

	}
	public partial class FlatTriGrid<TCell>
	{
		/**
			\copydoc FlatTriOp<TCell>::Rectangle
		*/
		public static FlatTriGrid<TCell> Rectangle(Int32 width, Int32 height)
		{
			return BeginShape().Rectangle(width, height).EndShape();
		}

		/**
			\copydoc FlatTriOp<TCell>::UpTriangle
		*/
		public static FlatTriGrid<TCell> UpTriangle(Int32 side)
		{
			return BeginShape().UpTriangle(side).EndShape();
		}

		/**
			\copydoc FlatTriOp<TCell>::DownTriangle
		*/
		public static FlatTriGrid<TCell> DownTriangle(Int32 side)
		{
			return BeginShape().DownTriangle(side).EndShape();
		}

		/**
			\copydoc FlatTriOp<TCell>::ParallelogramXY
		*/
		public static FlatTriGrid<TCell> ParallelogramXY(Int32 width, Int32 height)
		{
			return BeginShape().ParallelogramXY(width, height).EndShape();
		}

		/**
			\copydoc FlatTriOp<TCell>::ParallelogramXZ
		*/
		public static FlatTriGrid<TCell> ParallelogramXZ(Int32 width, Int32 height)
		{
			return BeginShape().ParallelogramXZ(width, height).EndShape();
		}

		/**
			\copydoc FlatTriOp<TCell>::Star
		*/
		public static FlatTriGrid<TCell> Star(Int32 side)
		{
			return BeginShape().Star(side).EndShape();
		}

		/**
			\copydoc FlatTriOp<TCell>::Hexagon
		*/
		public static FlatTriGrid<TCell> Hexagon(Int32 side)
		{
			return BeginShape().Hexagon(side).EndShape();
		}

		/**
			\copydoc FlatTriOp<TCell>::Default
		*/
		public static FlatTriGrid<TCell> Default(Int32 width, Int32 height)
		{
			return BeginShape().Default(width, height).EndShape();
		}

		/**
			\copydoc FlatTriOp<TCell>::Single
		*/
		public static FlatTriGrid<TCell> Single()
		{
			return BeginShape().Single().EndShape();
		}

		/**
			\copydoc FlatTriOp<TCell>::SingleGroup
		*/
		public static FlatTriGrid<TCell> SingleGroup()
		{
			return BeginShape().SingleGroup().EndShape();
		}

	}
	public partial class PointyRhombGrid<TCell>
	{
		/**
			\copydoc PointyRhombOp<TCell>::Rectangle
		*/
		public static PointyRhombGrid<TCell> Rectangle(Int32 width, Int32 height)
		{
			return BeginShape().Rectangle(width, height).EndShape();
		}

		/**
			\copydoc PointyRhombOp<TCell>::Hexagon
		*/
		public static PointyRhombGrid<TCell> Hexagon(Int32 side)
		{
			return BeginShape().Hexagon(side).EndShape();
		}

		/**
			\copydoc PointyRhombOp<TCell>::Parallelogram
		*/
		public static PointyRhombGrid<TCell> Parallelogram(Int32 width, Int32 height)
		{
			return BeginShape().Parallelogram(width, height).EndShape();
		}

		/**
			\copydoc PointyRhombOp<TCell>::FatRectangle
		*/
		public static PointyRhombGrid<TCell> FatRectangle(Int32 width, Int32 height)
		{
			return BeginShape().FatRectangle(width, height).EndShape();
		}

		/**
			\copydoc PointyRhombOp<TCell>::ThinRectangle
		*/
		public static PointyRhombGrid<TCell> ThinRectangle(Int32 width, Int32 height)
		{
			return BeginShape().ThinRectangle(width, height).EndShape();
		}

		/**
			\copydoc PointyRhombOp<TCell>::UpTriangle
		*/
		public static PointyRhombGrid<TCell> UpTriangle(Int32 side)
		{
			return BeginShape().UpTriangle(side).EndShape();
		}

		/**
			\copydoc PointyRhombOp<TCell>::DownTriangle
		*/
		public static PointyRhombGrid<TCell> DownTriangle(Int32 side)
		{
			return BeginShape().DownTriangle(side).EndShape();
		}

		/**
			\copydoc PointyRhombOp<TCell>::Diamond
		*/
		public static PointyRhombGrid<TCell> Diamond(Int32 side)
		{
			return BeginShape().Diamond(side).EndShape();
		}

		/**
			\copydoc PointyRhombOp<TCell>::Default
		*/
		public static PointyRhombGrid<TCell> Default(Int32 width, Int32 height)
		{
			return BeginShape().Default(width, height).EndShape();
		}

		/**
			\copydoc PointyRhombOp<TCell>::Single
		*/
		public static PointyRhombGrid<TCell> Single()
		{
			return BeginShape().Single().EndShape();
		}

		/**
			\copydoc PointyRhombOp<TCell>::SingleGroup
		*/
		public static PointyRhombGrid<TCell> SingleGroup()
		{
			return BeginShape().SingleGroup().EndShape();
		}

	}
	public partial class FlatRhombGrid<TCell>
	{
		/**
			\copydoc FlatRhombOp<TCell>::Rectangle
		*/
		public static FlatRhombGrid<TCell> Rectangle(Int32 width, Int32 height)
		{
			return BeginShape().Rectangle(width, height).EndShape();
		}

		/**
			\copydoc FlatRhombOp<TCell>::Hexagon
		*/
		public static FlatRhombGrid<TCell> Hexagon(Int32 side)
		{
			return BeginShape().Hexagon(side).EndShape();
		}

		/**
			\copydoc FlatRhombOp<TCell>::Parallelogram
		*/
		public static FlatRhombGrid<TCell> Parallelogram(Int32 width, Int32 height)
		{
			return BeginShape().Parallelogram(width, height).EndShape();
		}

		/**
			\copydoc FlatRhombOp<TCell>::FatRectangle
		*/
		public static FlatRhombGrid<TCell> FatRectangle(Int32 width, Int32 height)
		{
			return BeginShape().FatRectangle(width, height).EndShape();
		}

		/**
			\copydoc FlatRhombOp<TCell>::ThinRectangle
		*/
		public static FlatRhombGrid<TCell> ThinRectangle(Int32 width, Int32 height)
		{
			return BeginShape().ThinRectangle(width, height).EndShape();
		}

		/**
			\copydoc FlatRhombOp<TCell>::Default
		*/
		public static FlatRhombGrid<TCell> Default(Int32 width, Int32 height)
		{
			return BeginShape().Default(width, height).EndShape();
		}

		/**
			\copydoc FlatRhombOp<TCell>::Single
		*/
		public static FlatRhombGrid<TCell> Single()
		{
			return BeginShape().Single().EndShape();
		}

		/**
			\copydoc FlatRhombOp<TCell>::SingleGroup
		*/
		public static FlatRhombGrid<TCell> SingleGroup()
		{
			return BeginShape().SingleGroup().EndShape();
		}

	}
	public partial class CairoGrid<TCell>
	{
		/**
			\copydoc CairoOp<TCell>::Hexagon
		*/
		public static CairoGrid<TCell> Hexagon(Int32 side)
		{
			return BeginShape().Hexagon(side).EndShape();
		}

		/**
			\copydoc CairoOp<TCell>::FatRectangle
		*/
		public static CairoGrid<TCell> FatRectangle(Int32 width, Int32 height)
		{
			return BeginShape().FatRectangle(width, height).EndShape();
		}

		/**
			\copydoc CairoOp<TCell>::ThinRectangle
		*/
		public static CairoGrid<TCell> ThinRectangle(Int32 width, Int32 height)
		{
			return BeginShape().ThinRectangle(width, height).EndShape();
		}

		/**
			\copydoc CairoOp<TCell>::Rectangle
		*/
		public static CairoGrid<TCell> Rectangle(Int32 width, Int32 height)
		{
			return BeginShape().Rectangle(width, height).EndShape();
		}

		/**
			\copydoc CairoOp<TCell>::Parallelogram
		*/
		public static CairoGrid<TCell> Parallelogram(Int32 width, Int32 height)
		{
			return BeginShape().Parallelogram(width, height).EndShape();
		}

		/**
			\copydoc CairoOp<TCell>::Default
		*/
		public static CairoGrid<TCell> Default(Int32 width, Int32 height)
		{
			return BeginShape().Default(width, height).EndShape();
		}

		/**
			\copydoc CairoOp<TCell>::Single
		*/
		public static CairoGrid<TCell> Single()
		{
			return BeginShape().Single().EndShape();
		}

		/**
			\copydoc CairoOp<TCell>::SingleGroup
		*/
		public static CairoGrid<TCell> SingleGroup()
		{
			return BeginShape().SingleGroup().EndShape();
		}

	}
	public partial class LineGrid<TCell>
	{
		/**
			\copydoc LineOp<TCell>::Segment
		*/
		public static LineGrid<TCell> Segment(Int32 length)
		{
			return BeginShape().Segment(length).EndShape();
		}

	}
}
