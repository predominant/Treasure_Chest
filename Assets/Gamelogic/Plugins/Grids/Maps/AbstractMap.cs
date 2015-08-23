//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		This class is suitable to use as base class for most maps. 
	
		@implementers AbstractMap implements the IMap interface almost completely; you only 
		have to implement to methods that correspond to the two calculations: converting 
		from world to grid coordinates, and converting from grid to world coordinates. 
	
		
		
		@version1_0

		@ingroup Scaffolding
	*/
	[Immutable]
	abstract public class AbstractMap<TPoint> : IMap<TPoint>
		where TPoint : IGridPoint<TPoint>
	{
		#region Fields
		protected Vector2 cellDimensions;
		protected Vector2 anchorTranslation;

		protected Func<TPoint, TPoint> gridPointTransform;
		protected Func<TPoint, TPoint> inverseGridPointTransform;
		#endregion

		#region Construction

		protected AbstractMap(Vector2 cellDimensions) :
			this(cellDimensions, Vector2.zero)
		{ }

		protected AbstractMap(Vector2 cellDimensions, Vector2 anchorTranslation)
		{
			this.cellDimensions = cellDimensions;
			this.anchorTranslation = anchorTranslation;
			gridPointTransform = x => x;
			inverseGridPointTransform = x => x;
		}

		#endregion

		public Vector2 GetCellDimensions()
		{
			return cellDimensions;
		}

		public Vector2 GetAnchorTranslation()
		{
			return anchorTranslation;
		}

		public Vector2 this[TPoint point]
		{
			get
			{
				return GridToWorld(inverseGridPointTransform(point));
			}
		}

		public TPoint this[Vector2 point]
		{
			get
			{
				return gridPointTransform(RawWorldToGrid(point + anchorTranslation));
			}
		}

		public Func<TPoint, TPoint> GridPointTransform
		{
			get
			{
				return gridPointTransform;
			}
		}

		public Func<TPoint, TPoint> InverseGridPointTransform
		{
			get
			{
				return inverseGridPointTransform;
			}
		}

		/**
			This method maps a world point to a grid point, assuming that 
			the anchor point is centered in the cell. This will allow the 
			index accessors [] to give correct results for any anchoring.
		*/
		abstract public TPoint RawWorldToGrid(Vector2 worldPoint);

		abstract public Vector2 GridToWorld(TPoint gridPoint);

		/**
			Overrides this method to get custom Grid dimension calculations
		*/
		virtual public Vector2 GetCellDimensions(TPoint point)
		{
			return cellDimensions;
		}

		virtual public Vector2 CalcGridDimensions(IGridSpace<TPoint> grid)
		{
			if (!grid.Any()) return Vector2.zero;

			var rawGrid = grid.Select(point => InverseGridPointTransform(point));
			var rawGridAsList = rawGrid as IList<TPoint> ?? rawGrid.ToList();
			var firstPoint = rawGridAsList.First();

			var halfCellDimensions = GetCellDimensions(firstPoint) / 2;
			var worldPoint = GridToWorld(firstPoint);

			var minX = worldPoint.x - halfCellDimensions.x;
			var maxX = worldPoint.x + halfCellDimensions.x;
			var minY = worldPoint.y - halfCellDimensions.y;
			var maxY = worldPoint.y + halfCellDimensions.y;

			foreach (var point in rawGridAsList.ButFirst())
			{
				halfCellDimensions = GetCellDimensions(point) / 2;
				worldPoint = GridToWorld(point);

				minX = Mathf.Min(minX, worldPoint.x - halfCellDimensions.x);
				maxX = Mathf.Max(maxX, worldPoint.x + halfCellDimensions.x);
				minY = Mathf.Min(minY, worldPoint.y - halfCellDimensions.y);
				maxY = Mathf.Max(maxY, worldPoint.y + halfCellDimensions.y);
			}

			var width = maxX - minX;
			var height = maxY - minY;

			return new Vector2(width, height);
		}

		public Vector2 CalcAnchorBottomLeft(IGridSpace<TPoint> grid)
		{
			if (!grid.Any())
			{
				return Vector2.zero;
			}

			var rawGrid = grid.Select(point => InverseGridPointTransform(point));
			var rawGridAsList = rawGrid as IList<TPoint> ?? rawGrid.ToList();
			var firstPoint = rawGridAsList.First();

			var worldPoint = GridToWorld(firstPoint);

			var minX = worldPoint.x;
			var minY = worldPoint.y;

			foreach (var point in rawGridAsList.ButFirst())
			{
				worldPoint = GridToWorld(point);

				minX = Mathf.Min(minX, worldPoint.x);
				minY = Mathf.Min(minY, worldPoint.y);
			}

			return new Vector2(minX, minY);
		}

		virtual public Vector2 CalcBottomLeft(IGridSpace<TPoint> grid)
		{
			if (!grid.Any()) return Vector2.zero;

			var rawGrid = grid.Select(point => InverseGridPointTransform(point));
			var rawGridAsList = rawGrid as IList<TPoint> ?? rawGrid.ToList();
			var firstPoint = rawGridAsList.First();

			var halfCellDimensions = GetCellDimensions(firstPoint) / 2;
			var worldPoint = GridToWorld(firstPoint);

			var minX = worldPoint.x - halfCellDimensions.x;
			var minY = worldPoint.y - halfCellDimensions.y;

			foreach (var point in rawGridAsList.ButFirst())
			{
				halfCellDimensions = GetCellDimensions(point) / 2;
				worldPoint = GridToWorld(point);

				minX = Mathf.Min(minX, worldPoint.x - halfCellDimensions.x);
				minY = Mathf.Min(minY, worldPoint.y - halfCellDimensions.y);
			}

			return new Vector2(minX, minY);
		}

		public Vector2 CalcAnchorDimensions(IGridSpace<TPoint> grid)
		{
			if (!grid.Any())
			{
				return Vector2.zero;
			}
			var rawGrid = grid.Select(point => InverseGridPointTransform(point));
			var rawGridAsList = rawGrid as IList<TPoint> ?? rawGrid.ToList();
			var firstPoint = rawGridAsList.First();

			var worldPoint = GridToWorld(firstPoint);

			var minX = worldPoint.x;
			var maxX = worldPoint.x;
			var minY = worldPoint.y;
			var maxY = worldPoint.y;

			foreach (var point in rawGridAsList.ButFirst())
			{
				worldPoint = GridToWorld(point);

				minX = Mathf.Min(minX, worldPoint.x);
				maxX = Mathf.Max(maxX, worldPoint.x);
				minY = Mathf.Min(minY, worldPoint.y);
				maxY = Mathf.Max(maxY, worldPoint.y);
			}

			var width = maxX - minX;
			var height = maxY - minY;

			return new Vector2(width, height);
		}

		public IMap<TPoint> Translate(Vector2 offset)
		{
			return new CompoundMap<TPoint>(
				this, x => x + offset, x => x - offset);
		}

		public IMap<TPoint> Translate(float offsetX, float offsetY)
		{
			return Translate(new Vector2(offsetX, offsetY));
		}

		public IMap<TPoint> TranslateX(float offsetX)
		{
			return Translate(offsetX, 0);
		}

		public IMap<TPoint> TranslateY(float offsetY)
		{
			return Translate(0, offsetY);
		}

		public IMap<TPoint> ReflectAboutY()
		{
			return new CompoundMap<TPoint>(
				this, x => x.ReflectAboutY(), x => x.ReflectAboutY());
		}

		public IMap<TPoint> ReflectAboutX()
		{
			return new CompoundMap<TPoint>(
				this, x => x.ReflectAboutX(), x => x.ReflectAboutX());
		}

		public IMap<TPoint> FlipXY()
		{
			return new CompoundMap<TPoint>(
				this, x => x.ReflectXY(), x => x.ReflectXY());
		}

		public IMap<TPoint> Rotate(float angle)
		{
			return new CompoundMap<TPoint>(
				this, x => x.Rotate(angle), x => x.Rotate(-angle));
		}

		public IMap<TPoint> RotateAround(float angle, Vector2 point)
		{
			return new CompoundMap<TPoint>(
				this, x => x.RotateAround(angle, point), x => x.RotateAround(-angle, point));
		}

		public IMap<TPoint> Rotate90()
		{
			return new CompoundMap<TPoint>(
				this, x => x.Rotate90(), x => x.Rotate270());
		}

		public IMap<TPoint> Rotate180()
		{
			return new CompoundMap<TPoint>(
				this, x => x.Rotate180(), x => x.Rotate180());
		}

		//TODO: Should this alter cell dimensions?
		public IMap<TPoint> Scale(float factor)
		{
			return new CompoundMap<TPoint>(
				this, x => x * factor, x => x / factor);
		}

		//TODO: Should this alter cell dimensions?
		public IMap<TPoint> Scale(float factorX, float factorY)
		{
			return new CompoundMap<TPoint>(
				this, v => new Vector2(v.x * factorX, v.y * factorY), v => new Vector2(v.x / factorX, v.y / factorY));
		}

		//TODO: Should this alter cell dimensions?
		public IMap<TPoint> Scale(Vector2 factor)
		{
			return new CompoundMap<TPoint>(
				this, v => new Vector2(v.x * factor.x, v.y * factor.y), v => new Vector2(v.x / factor.x, v.y / factor.y));
		}

		public IMap<TPoint> SetGridPointTransforms(
			Func<TPoint, TPoint> coordinateTransformation,
			Func<TPoint, TPoint> inverseCoordinateTransformation)
		{
			gridPointTransform = coordinateTransformation;
			inverseGridPointTransform = inverseCoordinateTransformation;

			return this;
		}

		public WindowedMap<TPoint> WithWindow(Rect window)
		{
			return new WindowedMap<TPoint>(this, window);
		}

		public IMap<TPoint> AnchorCellLeft()
		{
			anchorTranslation.x = cellDimensions.x / 2 - cellDimensions.x;

			return this;
		}

		public IMap<TPoint> AnchorCellCenter()
		{
			return this;
		}

		public IMap<TPoint> AnchorCellRight()
		{
			anchorTranslation.x = cellDimensions.x / 2;

			return this;
		}

		public IMap<TPoint> AnchorCellTop()
		{
			anchorTranslation.y = cellDimensions.y / 2;

			return this;
		}

		public IMap<TPoint> AnchorCellBottom()
		{
			anchorTranslation.y = cellDimensions.y / 2 - cellDimensions.y;

			return this;
		}

		public IMap<TPoint> AnchorCellMiddle()
		{
			return this;
		}

		public IMap<TPoint> AnchorCellTopLeft()
		{
			return AnchorCellTop().AnchorCellLeft();
		}

		public IMap<TPoint> AnchorCellTopCenter()
		{
			return AnchorCellTop().AnchorCellCenter();
		}

		public IMap<TPoint> AnchorCellTopRight()
		{
			return AnchorCellTop().AnchorCellRight();
		}

		public IMap<TPoint> AnchorCellMiddleLeft()
		{
			return AnchorCellMiddle().AnchorCellLeft();
		}

		public IMap<TPoint> AnchorCellMiddleCenter()
		{
			return AnchorCellMiddle().AnchorCellCenter();
		}

		public IMap<TPoint> AnchorCellMiddleRight()
		{
			return AnchorCellMiddle().AnchorCellMiddleRight();
		}

		public IMap<TPoint> AnchorCellBottomLeft()
		{
			return AnchorCellBottom().AnchorCellLeft();
		}

		public IMap<TPoint> AnchorCellBottomCenter()
		{
			return AnchorCellBottom().AnchorCellCenter();
		}

		public IMap<TPoint> AnchorCellBottomRight()
		{
			return AnchorCellBottom().AnchorCellRight();
		}

		/**
			Animates a according to a fixed function. This is useful for animating grids
			when the animation does not depend on any input.

			@param animation This is a function that takes as parameters the a point in 
				the static grid and a time t (in seconds) and returns the new point
				at that time. An example of an animation is (x, t) => x + 5 * Mathf.Sin(t / 100)*Vector(1, 0),
				which will move the grid right and left periodically.
			@param This inverse function returns the point in the static grid, given the 
				animated point and time. If animate(p, t) returns r, then iverseAnimation(r, t) 
				must return p.
			
		*/
		public IMap<TPoint> Animate(Func<Vector2, float, Vector2> animation, Func<Vector2, float, Vector2> inverseAnimation)
		{
			return new AnimatableMap<TPoint>(cellDimensions, this, animation, inverseAnimation);
		}

		/**
			Makes a map that maps to 3D, with the grid positioned in the
			XY plane, with the given z-coordinate.
		*/
		public IMap3D<TPoint> To3DXY(float z)
		{
			return new Map3DXY<TPoint>(this, z);
		}

		/**
			Makes a map that maps to 3D, with the grid positioned in the
			XY plane, with the z-coordinate 0.
		*/
		public IMap3D<TPoint> To3DXY()
		{
			return new Map3DXY<TPoint>(this, 0);
		}

		/**
			Makes a map that maps to 3D, with the grid positioned in the
			XZ plane, with the given z-coordinate.
		*/
		public IMap3D<TPoint> To3DXZ(float y)
		{
			return new Map3DXZ<TPoint>(this, y);
		}

		/**
			Makes a map that maps to 3D, with the grid positioned in the
			XZ plane, with the z-coordinate 0.
		*/
		public IMap3D<TPoint> To3DXZ()
		{
			return new Map3DXZ<TPoint>(this, 0);
		}
	}
}