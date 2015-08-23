//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2014 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using Gamelogic.Internal.KDTree;

namespace Gamelogic.Grids
{
	/**
		A map that uses a Voronoi diagram to map points.

		@version1_8
		@ingroup Maps
	*/
	public class VoronoiMap<TPoint> : AbstractMap<TPoint>
		where TPoint: IGridPoint<TPoint>
	{
		#region Fields
		private readonly KDTree<TPoint> tree;
		private readonly IGridToWorldMap<TPoint> map;
		private readonly IDistanceFunction distanceFunction = new SquareEuclideanDistanceFunction();

		private readonly Func<Vector2, Vector2> func;
		private readonly Func<Vector2, Vector2> inverseFunc;
		#endregion

		#region Constructors
		public VoronoiMap(IEnumerable<TPoint> points, IGridToWorldMap<TPoint> map)
			:base(Vector2.zero)
		{
			this.map = map;
			/* TODO: make these parameters 
			const float alpha = .5f;
			const float beta = 20f;
			func = x => beta * x * Mathf.Pow(x.magnitude, alpha - 1);
			inverseFunc = x => x / beta * Mathf.Pow(x.magnitude / beta, 1 / alpha - 1);
			*/

			func = x => x;
			inverseFunc = x => x;
			
			tree = new KDTree<TPoint>(2);
			
			foreach (var point in points)
			{
				tree.AddPoint((map[point]), point);
			}
		}
		#endregion

		#region Abstract implementation
		public override TPoint RawWorldToGrid(Vector2 worldPoint)
		{
			var iter = tree.NearestNeighbors(func(worldPoint), distanceFunction, 1, -1);

			if (iter.MoveNext())
			{
				return iter.Current;
			}

			throw new IndexOutOfRangeException();
		}

		public override Vector2 GridToWorld(TPoint gridPoint)
		{
			return inverseFunc(map[gridPoint]);
		}
		#endregion

		#region Factory methods
		public static VoronoiMap<LinePoint> MakeMap(IEnumerable<Vector2> pointList)
		{
			var points = new List<LinePoint>();
			
			for (int i = 0; i < pointList.Count(); i++)
			{
				points.Add(i);
			}

			return new VoronoiMap<LinePoint>(points, new PointListMap(pointList));
		}
		#endregion
	}
}