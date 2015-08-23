//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using System;
using System.Collections.Generic;
using System.Diagnostics;
using Gamelogic.Grids;
using Vector2 = UnityEngine.Vector2;

namespace XXX
{
	/**
		This class provides utilities for constructing points, vectors and other 
		objects by examining the type parameters. These methods are mostly used for
		creating generic tests, and may be too slow to use in production code.
		
		@version1_0

		@ingroup Utilities
	*/
	public static class GridUtils
	{

		/**
			Note: this method is provided for generic testing purposes, 
			and should generally not be used in production code: it may be 
			slow.
		*/
		public static TPoint Zero<TPoint>()
				where TPoint : IGridPoint<TPoint>
		{
			if (typeof(TPoint) == typeof(PointyHexPoint))
			{
				return (TPoint)(object)PointyHexPoint.Zero;
			}
			if (typeof(TPoint) == typeof(FlatHexPoint))
			{
				return (TPoint)(object)FlatHexPoint.Zero;
			}
			if (typeof(TPoint) == typeof(RectPoint))
			{
				return (TPoint)(object)RectPoint.Zero;
			}
			if (typeof(TPoint) == typeof(DiamondPoint))
			{
				return (TPoint)(object)DiamondPoint.Zero;
			}
			if (typeof(TPoint) == typeof(FlatTriPoint))
			{
				return (TPoint)(object)FlatTriPoint.Zero;
			}
			if (typeof(TPoint) == typeof(PointyTriPoint))
			{
				return (TPoint)(object)PointyTriPoint.Zero;
			}
			if (typeof(TPoint) == typeof(FlatRhombPoint))
			{
				return (TPoint)(object)FlatRhombPoint.Zero;
			}
			if (typeof(TPoint) == typeof(PointyRhombPoint))
			{
				return (TPoint)(object)PointyRhombPoint.Zero;
			}
			throw new NotSupportedException();
		}

		public static int RenderIndex<TPoint>(this TPoint point)
		{

			if (typeof(TPoint) == typeof(PointyTriPoint))
			{
				return ((PointyTriPoint)(object)point).I;
			}
			if (typeof(TPoint) == typeof(FlatTriPoint))
			{
				return ((FlatTriPoint)(object)point).I;
			}
			if (typeof(TPoint) == typeof(PointyRhombPoint))
			{
				return ((PointyRhombPoint)(object)point).I;
			}
			if (typeof(TPoint) == typeof(FlatRhombPoint))
			{
				return ((FlatRhombPoint)(object)point).I;
			}
			if (typeof(TPoint) == typeof(CairoPoint))
			{
				return -(((CairoPoint)(object)point).I);
			}

			return 0;
		}


		/**
			Note: this method is provided for generic testing purposes, 
			and should generally not be used in production code: it may be 
			slow.
		*/
		public static TPoint MakePoint<TPoint>(int x, int y)
				where TPoint : IVectorPoint<TPoint>
		{
			if (typeof(TPoint) == typeof(PointyHexPoint))
			{
				return (TPoint)(object)new PointyHexPoint(x, y);
			}
			if (typeof(TPoint) == typeof(FlatHexPoint))
			{
				return (TPoint)(object)new FlatHexPoint(x, y);
			}
			if (typeof(TPoint) == typeof(RectPoint))
			{
				return (TPoint)(object)new RectPoint(x, y);
			}
			if (typeof(TPoint) == typeof(DiamondPoint))
			{
				return (TPoint)(object)new DiamondPoint(x, y);
			}
			throw new NotSupportedException();
		}

		public static TPoint MakePoint<TPoint, TBasePoint>(int x, int y, int index)
			where TPoint : ISplicedPoint<TPoint, TBasePoint>
			where TBasePoint : IVectorPoint<TBasePoint>, IGridPoint<TBasePoint>
		{
			if (typeof(TPoint) == typeof(PointyTriPoint))
			{
				return (TPoint)(object)new PointyTriPoint(x, y, index);
			}
			if (typeof(TPoint) == typeof(FlatTriPoint))
			{
				return (TPoint)(object)new FlatTriPoint(x, y, index);
			}
			if (typeof(TPoint) == typeof(PointyRhombPoint))
			{
				return (TPoint)(object)new PointyRhombPoint(x, y, index);
			}
			if (typeof(TPoint) == typeof(FlatRhombPoint))
			{
				return (TPoint)(object)new FlatRhombPoint(x, y, index);
			}
			throw new NotSupportedException();
		}

		public static IEnumerable<TPoint> MakePointList<TPoint>(params int[] coordinates)
			where TPoint : IVectorPoint<TPoint>, IGridPoint<TPoint>
		{
			int coordiateCount = coordinates.Length;

			Debug.Assert(coordiateCount % 2 == 0);

			var list = new PointList<TPoint>();

			for (int i = 0; i < coordiateCount; i += 2)
			{
				list.Add(MakePoint<TPoint>(coordinates[i], coordinates[i + 1]));
			}

			return list;
		}

		public static IEnumerable<TPoint> MakePointList<TPoint, TBasePoint>(params int[] coordinates)
			where TPoint : ISplicedPoint<TPoint, TBasePoint>
			where TBasePoint : IVectorPoint<TBasePoint>, IGridPoint<TBasePoint>
		{
			int coordiateCount = coordinates.Length;

			Debug.Assert(coordiateCount % 3 == 0);

			var list = new PointList<TPoint>();

			for (int i = 0; i < coordiateCount; i += 3)
			{
				list.Add(MakePoint<TPoint, TBasePoint>(coordinates[i], coordinates[i + 1], coordinates[i + 2]));
			}

			return list;
		}

		/**
			This method is provided for generic testing purposes. 
			It should generally not be used in production code: it may 
			be slow and not type-safe.
		*/
		public static TGrid MakeGrid<TPoint, TGrid, TCell>(int width, int height)
			where TPoint : IGridPoint<TPoint>
			where TGrid : IGrid<TCell, TPoint>
		{
			return MakeGrid<TPoint, TGrid, TCell>(width, height, Zero<TPoint>());
		}

		/**
			This method is provided for generic testing purposes. 
			It should generally not be used in production code: it may 
			be slow and not type-safe.
		*/
		public static TGrid MakeGrid<TPoint, TGrid, TCell>(int width, int height, Func<TPoint, bool> isInside)
			where TPoint : IGridPoint<TPoint>
			where TGrid : IGrid<TCell, TPoint>
		{
			return MakeGrid<TPoint, TGrid, TCell>(width, height, isInside, Zero<TPoint>());
		}

		/**
			This method is provided for generic testing purposes. 
			It should generally not be used in production code: it may 
			be slow and not type-safe.
		*/
		public static TGrid MakeGrid<TPoint, TGrid, TCell>(int width, int height, Func<TPoint, bool> isInside, TPoint offset)
			where TPoint : IGridPoint<TPoint>
			where TGrid : IGrid<TCell, TPoint>
		{
			if (typeof(TPoint) == typeof(PointyHexPoint))
			{
				Debug.Assert(typeof(TGrid) == typeof(PointyHexGrid<TCell>));

				return (TGrid)(object)new PointyHexGrid<TCell>(
					width,
					height,
					(Func<PointyHexPoint, bool>)(object)isInside,
					(PointyHexPoint)(object)offset);
			}
			if (typeof(TPoint) == typeof(FlatHexPoint))
			{
				Debug.Assert(typeof(TGrid) == typeof(FlatHexGrid<TCell>));

				return (TGrid)(object)new FlatHexGrid<TCell>(
					width,
					height,
					(Func<FlatHexPoint, bool>)(object)isInside,
					(FlatHexPoint)(object)offset);
			}
			if (typeof(TPoint) == typeof(RectPoint))
			{
				Debug.Assert(typeof(TGrid) == typeof(RectGrid<TCell>));

				return (TGrid)(object)new RectGrid<TCell>(
					width,
					height,
					(Func<RectPoint, bool>)(object)isInside,
					(RectPoint)(object)offset);
			}
			if (typeof(TPoint) == typeof(DiamondPoint))
			{
				Debug.Assert(typeof(TGrid) == typeof(DiamondGrid<TCell>));

				return (TGrid)(object)new DiamondGrid<TCell>(
					width,
					height,
					(Func<DiamondPoint, bool>)(object)isInside,
					(DiamondPoint)(object)offset);
			}

			throw new NotSupportedException();
		}

		/**
			This method is provided for generic testing purposes. 
			It should generally not be used in production code: it may 
			be slow and not type-safe.
		*/
		public static TGrid MakeGrid<TPoint, TGrid, TCell>(int width, int height, TPoint offset)
			where TPoint : IGridPoint<TPoint>
			where TGrid : IGrid<TCell, TPoint>
		{
			if (typeof(TPoint) == typeof(PointyHexPoint))
			{
				Debug.Assert(typeof(TGrid) == typeof(PointyHexGrid<TCell>));

				return (TGrid)(object)new PointyHexGrid<TCell>(
					width,
					height,
					x => PointyHexGrid<TCell>.DefaultContains(x, width, height),
					(PointyHexPoint)(object)offset);
			}
			if (typeof(TPoint) == typeof(FlatHexPoint))
			{
				Debug.Assert(typeof(TGrid) == typeof(FlatHexGrid<TCell>));

				return (TGrid)(object)new FlatHexGrid<TCell>(
					width,
					height,
					x => FlatHexGrid<TCell>.DefaultContains(x, width, height),
					(FlatHexPoint)(object)offset);
			}
			if (typeof(TPoint) == typeof(RectPoint))
			{
				Debug.Assert(typeof(TGrid) == typeof(RectGrid<TCell>));

				return (TGrid)(object)new RectGrid<TCell>(
					width,
					height,
					x => RectGrid<TCell>.DefaultContains(x, width, height),
					(RectPoint)(object)offset);
			}
			if (typeof(TPoint) == typeof(DiamondPoint))
			{
				Debug.Assert(typeof(TGrid) == typeof(DiamondGrid<TCell>));

				return (TGrid)(object)new DiamondGrid<TCell>(
					width,
					height,
					x => DiamondGrid<TCell>.DefaultContains(x, width, height),
					(DiamondPoint)(object)offset);
			}

			throw new NotSupportedException();
		}

		public static int SpliceCount<TPoint>(this TPoint point)
			where TPoint : IGridPoint<TPoint>
		{
			
			if (typeof(TPoint) == typeof(PointyTriPoint))
			{
				return PointyTriPoint.SpliceCount;
			}
			if (typeof(TPoint) == typeof(FlatTriPoint))
			{
				return FlatTriPoint.SpliceCount;
			}
			if (typeof(TPoint) == typeof(PointyRhombPoint))
			{
				return PointyRhombPoint.SpliceCount;
			}
			if (typeof(TPoint) == typeof(FlatRhombPoint))
			{
				return FlatRhombPoint.SpliceCount;
			}
			if (typeof(TPoint) == typeof(CairoPoint))
			{
				return CairoPoint.SpliceCount;
			}

			return 1;
		}


		public static IMap<TPoint> MakeMap<TMap, TPoint>(Vector2 cellDimensions)
			where TPoint : IGridPoint<TPoint>
			where TMap : IMap<TPoint>
		{
			if (typeof(TMap) == typeof(RectMap))
			{
				Debug.Assert(typeof(TPoint) == typeof(RectPoint));

				return (IMap<TPoint>)new RectMap(cellDimensions);
			}
			if (typeof(TMap) == typeof(DiamondMap))
			{
				Debug.Assert(typeof(TPoint) == typeof(DiamondPoint));

				return (IMap<TPoint>)new DiamondMap(cellDimensions);
			}
			if (typeof(TMap) == typeof(FlatHexMap))
			{
				Debug.Assert(typeof(TPoint) == typeof(FlatHexPoint));

				return (IMap<TPoint>)new FlatHexMap(cellDimensions);
			}
			if (typeof(TMap) == typeof(PointyHexMap))
			{
				Debug.Assert(typeof(TPoint) == typeof(PointyHexPoint));

				return (IMap<TPoint>)new PointyHexMap(cellDimensions);
			}
			if (typeof(TMap) == typeof(FlatTriMap))
			{
				Debug.Assert(typeof(TPoint) == typeof(FlatTriPoint));

				return (IMap<TPoint>)new FlatTriMap(cellDimensions);
			}
			if (typeof(TMap) == typeof(PointyTriMap))
			{
				Debug.Assert(typeof(TPoint) == typeof(PointyTriPoint));

				return (IMap<TPoint>)new PointyTriMap(cellDimensions);
			}
			if (typeof(TMap) == typeof(FlatRhombMap))
			{
				Debug.Assert(typeof(TPoint) == typeof(FlatRhombPoint));

				return (IMap<TPoint>)new FlatRhombMap(cellDimensions);
			}
			if (typeof(TMap) == typeof(PointyRhombMap))
			{
				Debug.Assert(typeof(TPoint) == typeof(PointyRhombPoint));

				return (IMap<TPoint>)new PointyRhombMap(cellDimensions);
			}
			throw new NotSupportedException();
		}

		public static string ToGizmoString(this object obj)
		{
			if (obj == null) return "null";

			var p1 = obj as InspectableVectorPoint;

			if (p1 != null) return p1.ToGizmoString();

			var p2 = obj as InspectableSplicedVectorPoint;

			if (p2 != null) return p2.ToGizmoString();

			return obj.ToString();
		}

		public static string ToGizmoString(this InspectableVectorPoint point)
		{
			return "(" + point.x + " " + point.y + ")";
		}

		public static string ToGizmoString(this InspectableSplicedVectorPoint point)
		{
			return "(" + point.x + " " + point.y + " | " + point.index + ")";
		}

		public static int GetColor(this object point, int x0, int x1, int y1)
		{
			var p1 = point as InspectableVectorPoint;

			if (p1 != null) return p1.GetColor(x0, x1, y1);


			var p2 = point as InspectableSplicedVectorPoint;

			if (p2 != null) return p2.GetColor(x0, x1, y1);

			return 0;
		}

		public static int GetColor(this InspectableVectorPoint point, int x0, int x1, int y1)
		{
			return point.GetRectPoint().GetColor(x0, x1, y1);
		}


		public static int GetColor(this InspectableSplicedVectorPoint point, int x0, int x1, int y1)
		{
			return point.GetPointyTriPoint().GetColor(x0, x1, y1);
		}

		public static TPoint VectorPointToGridPoint<TPoint>(InspectableVectorPoint p)
			where TPoint : IVectorPoint<TPoint>
		{
			return MakePoint<TPoint>(p.x, p.y);
		}

		public static TPoint VectorPointToGridPoint<TPoint>(object point)
		{
			var p1 = point as InspectableVectorPoint;

			if (p1 != null)
			{

				if (typeof(TPoint) == typeof(PointyHexPoint))
				{
					return (TPoint)(object)VectorPointToGridPoint<PointyHexPoint>(p1);
				}
				if (typeof(TPoint) == typeof(FlatHexPoint))
				{
					return (TPoint)(object)VectorPointToGridPoint<FlatHexPoint>(p1);
				}
				if (typeof(TPoint) == typeof(RectPoint))
				{
					return (TPoint)(object)VectorPointToGridPoint<RectPoint>(p1);
				}
				if (typeof(TPoint) == typeof(DiamondPoint))
				{
					return (TPoint)(object)VectorPointToGridPoint<DiamondPoint>(p1);
				}
			}

			var p2 = point as InspectableSplicedVectorPoint;

			if (p2 != null)
			{
				if (typeof(TPoint) == typeof(PointyTriPoint))
				{
					return (TPoint)(object)VectorPointToGridPoint<PointyTriPoint, FlatHexPoint>(p2);
				}
				if (typeof(TPoint) == typeof(FlatTriPoint))
				{
					return (TPoint)(object)VectorPointToGridPoint<FlatTriPoint, PointyHexPoint>(p2);
				}
				if (typeof(TPoint) == typeof(PointyRhombPoint))
				{
					return (TPoint)(object)VectorPointToGridPoint<PointyRhombPoint, PointyHexPoint>(p2);
				}
				if (typeof(TPoint) == typeof(FlatRhombPoint))
				{
					return (TPoint)(object)VectorPointToGridPoint<FlatRhombPoint, FlatHexPoint>(p2);
				}
				if (typeof(TPoint) == typeof(CairoPoint))
				{
					return (TPoint)(object)VectorPointToGridPoint<CairoPoint, PointyHexPoint>(p2);
				}
			}

			return (TPoint)point;
		}

		public static TPoint VectorPointToGridPoint<TPoint, TBasePoint>(InspectableSplicedVectorPoint p)
			where TPoint : ISplicedPoint<TPoint, TBasePoint>
			where TBasePoint : IVectorPoint<TBasePoint>, IGridPoint<TBasePoint>
		{
			return MakePoint<TPoint, TBasePoint>(p.x, p.y, p.index);
		}

		private static InspectableVectorPoint GridPointToVectorPoint<TPoint>(TPoint p)
			where TPoint : IVectorPoint<TPoint>
		{
			return new InspectableVectorPoint
			{
				x = p.X,
				y = p.Y
			};
		}

		private static InspectableSplicedVectorPoint GridPointToVectorPoint<TPoint, TBasePoint>(TPoint p)
			where TPoint : ISplicedPoint<TPoint, TBasePoint>
			where TBasePoint : IVectorPoint<TBasePoint>, IGridPoint<TBasePoint>
		{
			return new InspectableSplicedVectorPoint
			{
				x = p.X,
				y = p.Y,
				index = p.I
			};
		}

		public static object GridPointToVectorPoint<TPoint>(IGridPoint<TPoint> p)
			where TPoint : IGridPoint<TPoint>
		{
			if (typeof(TPoint) == typeof(PointyHexPoint))
			{
				return GridPointToVectorPoint((PointyHexPoint)(object)p);
			}
			if (typeof(TPoint) == typeof(FlatHexPoint))
			{
				return GridPointToVectorPoint((FlatHexPoint)(object)p);
			}
			if (typeof(TPoint) == typeof(RectPoint))
			{
				return GridPointToVectorPoint((RectPoint)(object)p);
			}
			if (typeof(TPoint) == typeof(DiamondPoint))
			{
				return GridPointToVectorPoint((DiamondPoint)(object)p);
			}
			if (typeof(TPoint) == typeof(PointyTriPoint))
			{
				return GridPointToVectorPoint<PointyTriPoint, FlatHexPoint>((PointyTriPoint)(object)p);
			}
			if (typeof(TPoint) == typeof(FlatTriPoint))
			{
				return GridPointToVectorPoint<FlatTriPoint, PointyHexPoint>((FlatTriPoint)(object)p);
			}
			if (typeof(TPoint) == typeof(PointyRhombPoint))
			{
				return GridPointToVectorPoint<PointyRhombPoint, PointyHexPoint>((PointyRhombPoint)(object)p);
			}
			if (typeof(TPoint) == typeof(FlatRhombPoint))
			{
				return GridPointToVectorPoint<FlatRhombPoint, FlatHexPoint>((FlatRhombPoint)(object)p);
			}
			if (typeof(TPoint) == typeof(CairoPoint))
			{
				return GridPointToVectorPoint<CairoPoint, PointyHexPoint>((CairoPoint)(object)p);
			}

			return p;
		}

		public static int SpliceCount<TPoint, TBasePoint>()
			where TPoint: ISplicedPoint<TPoint, TBasePoint>
			where TBasePoint: IVectorPoint<TBasePoint>, IGridPoint<TBasePoint>
		{
			if (typeof(TPoint) == typeof(PointyTriPoint))
			{
				return PointyTriPoint.SpliceCount;
			}
			if (typeof(TPoint) == typeof(FlatTriPoint))
			{
				return FlatTriPoint.SpliceCount;
			}
			if (typeof(TPoint) == typeof(PointyRhombPoint))
			{
				return PointyRhombPoint.SpliceCount;
			}
			if (typeof(TPoint) == typeof(FlatRhombPoint))
			{
				return FlatRhombPoint.SpliceCount;
			}
			if (typeof(TPoint) == typeof(CairoPoint))
			{
				return CairoPoint.SpliceCount;
			}

			throw new NotSupportedException();
		}
	}
}