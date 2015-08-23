//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2014 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		A point that represent cells in a LayeredGrid.

		@version1_8
		@ingroup Points
	*/
	[Experimental]
	public struct LayeredPoint<TPoint> : IGridPoint<LayeredPoint<TPoint>> where TPoint : IGridPoint<TPoint>
	{
		private readonly TPoint point;
		private readonly int layer;

		public TPoint Point
		{
			get { return point; }
		}

		public int Layer
		{
			get { return layer; }
		}

		public int SpliceIndex
		{
			get
			{
				return point.SpliceIndex;
			}
		}

		public int SpliceCount
		{
			get
			{
				return point.SpliceCount;
			}
		}

		public LayeredPoint(TPoint point, int layer)
		{
			this.point = point;
			this.layer = layer;
		}

		public bool Equals(LayeredPoint<TPoint> other)
		{
			return layer == other.layer && point.Equals(other.point);
		}

		public int DistanceFrom(LayeredPoint<TPoint> other)
		{
			return point.DistanceFrom(other.point) + Mathf.Abs(layer - other.layer);
		}

		override public string ToString()
		{
			return "" + layer + " : " + point.ToString();
		}
	}

	/**
		A layered grid is a 3D grid made out of layers of 2D grids with the same point type. Each layer can have a different 
		shape.

		Layered grids are accessed through LayeredPoints.

		Note that neighbors are not implemented for this grid. If you need to use neighbor relationships, you need
		to extend this class and configure neighbors. See [
A new look at layered grids: setting up neighbors](http://gamelogic.co.za/2014/05/24/a-new-look-at-layered-grids-setting-up-neighbors/).

		@version1_8
		@ingroup Grids
	*/
	[Experimental]
	public class LayeredGrid<TCell, TPoint> : IGrid<TCell, LayeredPoint<TPoint>> where TPoint : IGridPoint<TPoint>
	{
		private readonly IGrid<TCell, TPoint>[] layers;

		public int LayerCount
		{
			get
			{
				return layers.Length;
			}
		}

		public LayeredGrid(int width, int height, int layerCount, Func<TPoint, bool> contains,
			Func<int, int, Func<TPoint, bool>, IGrid<TCell, TPoint>> constructor)
		{
			layers = new IGrid<TCell, TPoint>[layerCount];

			for (int i = 0; i < layerCount; i++)
			{
				layers[i] = constructor(width, height, contains);
			}
		}

		public LayeredGrid(int width, int height, int layerCount, Func<TPoint, bool> contains,
			Func<int, int, int, Func<TPoint, bool>, IGrid<TCell, TPoint>> constructor)
		{
			layers = new IGrid<TCell, TPoint>[layerCount];

			for (int i = 0; i < layerCount; i++)
			{
				layers[i] = constructor(i, width, height, contains);
			}
		}

		public static LayeredGrid<TCell, TPoint2> Make<TShapeInfo, TGrid, TPoint2, TVectorPoint, TShapeOp>(
			TShapeInfo[] shapes)

			where TShapeInfo : AbstractShapeInfo<TShapeInfo, TGrid, TPoint2, TVectorPoint, TShapeOp>
			where TPoint2 : IGridPoint<TPoint2>, ISplicedVectorPoint<TPoint2, TVectorPoint>
			where TVectorPoint : IVectorPoint<TVectorPoint>
			where TGrid : IGrid<TCell, TPoint2>

		{
			var layers = new IGrid<TCell, TPoint2>[shapes.Length];
			for (int i = 0; i < layers.Length; i++)
			{
				layers[i] = shapes[i].EndShape();
			}

			return new LayeredGrid<TCell, TPoint2>(layers);
		}

		public static LayeredGrid<TCell, TPoint2> Make<TShapeInfo, TGrid, TPoint2, TVectorPoint, TShapeOp>(int layerCount,
			TShapeInfo shape)

			where TShapeInfo : AbstractShapeInfo<TShapeInfo, TGrid, TPoint2, TVectorPoint, TShapeOp>
			where TPoint2 : IGridPoint<TPoint2>, ISplicedVectorPoint<TPoint2, TVectorPoint>
			where TVectorPoint : IVectorPoint<TVectorPoint>
			where TGrid : IGrid<TCell, TPoint2>
		{
			var layers = new IGrid<TCell, TPoint2>[layerCount];

			if (layerCount > 0)
			{
				layers[0] = shape.EndShape();
			}

			for (int i = 1; i < layerCount; i++)
			{
				layers[i] = layers[0].CloneStructure<TCell>();
			}

			return new LayeredGrid<TCell, TPoint2>(layers);
		}

		protected LayeredGrid(IGrid<TCell, TPoint>[] layers)
		{
			this.layers = layers;
		}

		public bool Contains(LayeredPoint<TPoint> point)
		{
			if (point.Layer < 0 || point.Layer >= layers.Length)
			{
				return false;
			}

			return layers[point.Layer].Contains(point.Point);
		}

		public IEnumerator<LayeredPoint<TPoint>> GetEnumerator()
		{
			for (int i = 0; i < layers.Length; i++)
			{
				foreach (var layerPoint in layers[i])
				{
					yield return new LayeredPoint<TPoint>(layerPoint, i);
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IGrid<TNewCell, LayeredPoint<TPoint>> CloneStructure<TNewCell>()
		{
			return new LayeredGrid<TNewCell, TPoint>(layers.Select(layer => layer.CloneStructure<TNewCell>()).ToArray());
		}

		/**
			This method is not implemented. If you need a layered grid with neighbors configured (for example,
			if you want to use path finding or connected shapes), you need to make your own layered grid by extending from this one,
			and implement this method.
		*/
		virtual public IEnumerable<LayeredPoint<TPoint>> GetAllNeighbors(LayeredPoint<TPoint> point)
		{
			//override this for your use!
			throw new NotImplementedException();
		}

		public IEnumerable<LayeredPoint<TPoint>> GetLargeSet(int n)
		{
			var largeSet = new List<LayeredPoint<TPoint>>();

			for (int i = -n; i <= n; i++)
			{
				var largeSetLayer = layers[i].GetLargeSet(n);

				largeSet.AddRange(largeSetLayer.Select(largeSetPoint => new LayeredPoint<TPoint>(largeSetPoint, i)));
			}

			return largeSet;
		}

		public IEnumerable<LayeredPoint<TPoint>> GetStoragePoints()
		{
			var storagePoints = new List<LayeredPoint<TPoint>>();

			for (int i = 0; i < LayerCount; i++)
			{
				storagePoints.AddRange(layers[i].GetStoragePoints().Select(layerPoint => new LayeredPoint<TPoint>(layerPoint, i)));
			}

			return storagePoints;
		}

		public TCell this[LayeredPoint<TPoint> point]
		{
			get
			{
				return layers[point.Layer][point.Point];
			}

			set
			{
				layers[point.Layer][point.Point] = value;
			}
		}

		object IGrid<LayeredPoint<TPoint>>.this[LayeredPoint<TPoint> point]
		{
			get { return this[point]; }
			set { this[point] = (TCell)value; }
		}

		public IEnumerable<TCell> Values
		{
			get
			{
				return this.Select(p => this[p]);
			}
		}

		IEnumerable IGrid<LayeredPoint<TPoint>>.Values
		{
			get { return Values; }
		}
	}

	/**
		A simple map that can be used for Layered Grids.

		@since 1.8
		@ingroup Maps
	*/
	[Experimental]
	public class SimpleLayeredMap<TPoint> : IMap3D<LayeredPoint<TPoint>>
		where TPoint : IGridPoint<TPoint>
	{
		private readonly float layerDistance;
		private readonly float layerOffset;
		private readonly IMap<TPoint> baseMap;

		public SimpleLayeredMap(IMap<TPoint> baseMap, float layerDistance, float layerOffset)
		{
			this.layerDistance = layerDistance;
			this.layerOffset = layerOffset;
			this.baseMap = baseMap;
		}

		/**
			This method is not implemented. Unlike most grids, this is not a 2D grid embedded in 3D,
			so projecting it to 2D does not make sense.
		*/
		public IMap<LayeredPoint<TPoint>> To2D()
		{
			throw new NotImplementedException();
		}

		public Vector3 this[LayeredPoint<TPoint> point]
		{
			get
			{
				Vector2 worldPoint2D = baseMap[point.Point];
				var layerHeight = point.Layer*layerDistance;

				return new Vector3(worldPoint2D.x, layerHeight + layerOffset, worldPoint2D.y);
			}
		}

		public LayeredPoint<TPoint> this[Vector3 point]
		{
			get
			{
				int layerIndex = Mathf.RoundToInt((point.y - layerOffset)/layerDistance);

				var point2D = baseMap[new Vector2(point.x, point.z)];

				return new LayeredPoint<TPoint>(point2D, layerIndex);
			}
		}
	}
}
