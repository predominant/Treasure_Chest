using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		A map that can be used for general spliced grids made from polygons.

		To use this map:
			- Find a rectangular tesselation for your grid.
			- This rectangle will consist out of several polygons, each which represents a (partial) cell of your grid.
			- One of these polygons should correspond to the splice point (0, 0, 0). 
			- Find the center of this polygon relative to the rectangle in normalized coordinates, that is, so that the entire rectangle corresponds to (0, 0) -> (1, 1).
			- Label the polygons 0, 1, 2, ... etc.
			- Find the vertices of each polygon in normalized coordinates.
			- Now calculate the spliced vectorpoint offsets for each polygon, relative to the (0, 0, 0)
				polygon, in normalised coordinates.

		
		
		@version1_2
	*/
	[Experimental]
	public abstract class PolygonGridMap<TPoint, TBasePoint> : AbstractMap<TPoint>
		where TPoint : ISplicedPoint<TPoint, TBasePoint>
		where TBasePoint : IGridPoint<TBasePoint>, IVectorPoint<TBasePoint>
	{
		private readonly List<List<Vector2>> polies;
		private readonly PointList<TPoint> offsets;
		private readonly List<Vector2> rectOffsets;

		private readonly Func<int, int, int, TPoint> pointFactory;
		private readonly Func<TBasePoint, Vector2> baseMap;
		private readonly Func<VectorPoint, TBasePoint> rectPointMap;
		private Vector2 rectDimensions;
		private readonly Vector2 zeroShapeOffset;

		/**
			@param cellDimensions size of the cells in world coordinates
			@param rectDimensions size of the rectangle in world coodrinates
			@param zeroShapeOffset normalized world coordinates of the shape (0, 0, 0) relative to the rectangle bottom left corner
			@param polies each item in the list corresponds to a polygon, andd each polygon is a list of vertices in order (each vertice needs to be included only once).
			@param offsets offsets in grid coordinates for each poly relative to (0, 0, 0) in the same order as the polies list
			@param rectOffsets the offsets in normalised world coordinates for each splice index
			@param pointFactory returns a new spliced point with the given coordinates
			@param baseMap returns the world coordinate for the given base point
			@param rectPointMap maps the rectangular patches with the correct base point (that corresponds to (0, 0, 0) in that rectangle).
		*/

		protected PolygonGridMap(
			Vector2 cellDimensions, 
			Vector2 rectDimensions,
			Vector2 zeroShapeOffset,
			IEnumerable<IEnumerable<Vector2>> polies, 
			IEnumerable<TPoint> offsets,
			IEnumerable<Vector2> rectOffsets,
			Func<int, int, int, TPoint> pointFactory,
			Func<TBasePoint, Vector2> baseMap,
			Func<VectorPoint, TBasePoint> rectPointMap
			) :
			base(cellDimensions)
		{
			this.rectDimensions = rectDimensions;			
			this.zeroShapeOffset = zeroShapeOffset.HadamardMul(rectDimensions);
			this.polies = Scale(polies, rectDimensions).Select(e => e.ToList()).ToList();
			this.offsets = new PointList<TPoint>(offsets);
			this.pointFactory = pointFactory;			
			this.rectOffsets = Scale(rectOffsets, rectDimensions).ToList();
			this.baseMap = baseMap;
			this.rectPointMap = rectPointMap;
		}

		/**
			Scales a list of points componntwise with the factor.
		*/
		public static IEnumerable<Vector2> Scale(IEnumerable<Vector2> points, Vector2 factor)
		{
			return points.Select(point => point.HadamardMul(factor));
		}

		/**
			Scales a list of points componntwise the factor.
		*/
		public static IEnumerable<Vector2> Scale(IEnumerable<Vector2> points, float factor)
		{
			return points.Select(point => point * factor);
		}

		/**
			Scales the vertices of a list of polygons (each represented as a list of vertices) compontwise with the factor.
		*/
		public static IEnumerable<IEnumerable<Vector2>> Scale(IEnumerable<IEnumerable<Vector2>> polies, Vector2 factor)
		{
			return polies.Select(poly => Scale(poly, factor));
		}

		/**
			Scales the vertices of a list of polygons (each represented as a list of vertices) with the factor.
		*/
		public static IEnumerable<IEnumerable<Vector2>> Scale(IEnumerable<IEnumerable<Vector2>> polies, float factor)
		{
			return polies.Select(poly => Scale(poly, factor));
		}

		public override TPoint RawWorldToGrid(Vector2 worldPoint)
		{
			var worldPoint2 = worldPoint + zeroShapeOffset;			
			var rectPoint = worldPoint2.HadamardDiv(rectDimensions).FloorToVectorPoint();
			var rectOffset = rectDimensions.HadamardMul(rectPoint);
			var normalisedWorldPoint = worldPoint2 - rectOffset;
			var basePoint = rectPointMap(rectPoint);

			for (var i = 0; i < polies.Count; i++)
			{
				var poly = polies[i];

				if (IsInsidePoly(normalisedWorldPoint, poly))
				{
					return pointFactory(basePoint.X, basePoint.Y, 0).MoveBy(offsets[i]);
				}
			}

			//should not happen...
			return pointFactory(rectPoint.X, rectPoint.Y, -1);
		}

		/**
			returns True if p1 and p2 are on the same side of the line a->b
		*/
		private static bool SameSide(Vector2 p1, Vector2 p2, Vector2 a, Vector2 b)
		{
			var cp1 = (b.x - a.x) * (p1.y - a.y) - (p1.x - a.x) * (b.y - a.y);
			var cp2 = (b.x - a.x) * (p2.y - a.y) - (p2.x - a.x) * (b.y - a.y);

			return cp1 * cp2 >= 0;
		}

		/**
			Returns whether a point is inside a given triangle.
			If for each pair of points AB in the triangle, P is on the same side of AB as 
			the other point in the triangle, then P is in the triangle. 
		*/
		private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
		{
			return SameSide(p, a, b, c) && SameSide(p, b, a, c) && SameSide(p, c, a, b);
		}

		/**
			Returns True if (x,y) is inside convex polygon defined by list of points
			points must be in clockwise (or anticlockwise) order.
			won't work for just any polygon!
			Works by splitting polygon into triangles, and checking each of those
		*/
		private static bool IsInsidePoly(Vector2 normalisedWorldPoint, IList<Vector2> poly)
		{
			var p = poly[0];

			for (var i = 1; i < poly.Count - 1; i++)
			{
				var p1 = poly[i];
				var p2 = poly[i + 1];

				if (PointInTriangle(normalisedWorldPoint, p1, p2, p))
				{
					return true;
				}
			}

			return false;
		}		

		public override Vector2 GridToWorld(TPoint gridPoint)
		{
			return baseMap(gridPoint.BasePoint).HadamardMul(rectDimensions) + 
				rectOffsets[gridPoint.I];
		}

		/**
			Returns the cell dimensions for this cell.

			Generally, for spliced points, these are the same 
			for cells (points) with the same splice index.
		*/
		abstract public override Vector2 GetCellDimensions(TPoint point);
	}
}
