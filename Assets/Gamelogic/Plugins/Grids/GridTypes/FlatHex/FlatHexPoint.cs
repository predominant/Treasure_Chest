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
		A point of a hexagonal grid, where the hexagons have two horizontal edges.
	
		
		
		@version1_0

		@ingroup Points
	*/
	[Serializable]
	[Immutable]
	public partial struct FlatHexPoint :
		IGridPoint<FlatHexPoint>,
		IVectorPoint<FlatHexPoint>
	{
		#region Constants
		public static readonly FlatHexPoint NorthEast = new FlatHexPoint(1, 0);
		public static readonly FlatHexPoint North = new FlatHexPoint(0, 1);
		public static readonly FlatHexPoint NorthWest = new FlatHexPoint(-1, 1);
		public static readonly FlatHexPoint SouthWest = new FlatHexPoint(-1, 0);
		public static readonly FlatHexPoint South = new FlatHexPoint(0, -1);
		public static readonly FlatHexPoint SouthEast = new FlatHexPoint(1, -1);

		public static readonly IEnumerable<FlatHexPoint> MainDirections = new PointList<FlatHexPoint>
		{
			NorthEast,
			North,
			NorthWest,
			SouthWest,
			South,
			SouthEast
		};

		/**
			List of transforms, exlcuding the identity transform.
		 */
		private readonly static IEnumerable<Func<FlatHexPoint, FlatHexPoint>>
			PointTransforms = new List<Func<FlatHexPoint, FlatHexPoint>>
		{
			x => x.Rotate60(),
			x => x.Rotate120(),
			x => x.Rotate180(),
			x => x.Rotate240(),
			x => x.Rotate300(),
			x => x.ReflectAboutY(),
			x => x.Rotate60().ReflectAboutY(),
			x => x.Rotate120().ReflectAboutY(),
			x => x.Rotate180().ReflectAboutY(),
			x => x.Rotate240().ReflectAboutY(),
			x => x.Rotate300().ReflectAboutY(),
		};
		#endregion

		#region Properties
		/**
		Gets the Z coordinate of the point. The Z-coordinate is redundent, but is used for convenience 
		by some algorithms. The coordinates satisfy \f$x + y + z = 0\f$. 
	*/
		public int Z
		{
			get
			{
				return -(X + Y);
			}
		}
		#endregion

		#region Distance
		/**
		The magnitude of a hex point is the hex-distance between the point and the origin.

		This notation makes using hex points useful to use as vectors.
	*/
		public int Magnitude()
		{
			return (Mathf.Abs(X) + Mathf.Abs(Y) + Mathf.Abs(X + Y)) / 2;
		}
		#endregion

		#region Transformations
		public static bool IsEquivalentUnderTransformsAndTranslation(
			IEnumerable<FlatHexPoint> shape1,
			IEnumerable<FlatHexPoint> shape2)
		{
			return Algorithms.IsEquivalentUnderTransformsAndTranslation<FlatHexPoint>(
				shape1,
				shape2,
				PointTransforms,
				ToCanonicalPosition);
		}

		public static IEnumerable<FlatHexPoint> ToCanonicalPosition(IEnumerable<FlatHexPoint> shape)
		{
			FlatHexPoint firstPoint = shape.First();

			int minX = firstPoint.X;
			int minY = firstPoint.Y;

			foreach (FlatHexPoint point in shape.ButFirst())
			{
				if (point.X < minX)
				{
					minX = point.X;
				}

				if (point.Y < minY)
				{
					minY = point.Y;
				}
			}

			return
				from point in shape
				select new FlatHexPoint(point.X - minX, point.Y - minY);
		}


		public FlatHexPoint Rotate60()
		{
			return new FlatHexPoint(-Y, X + Y);
		}

		public FlatHexPoint Rotate120()
		{
			return new FlatHexPoint(-X - Y, X);
		}

		public FlatHexPoint Rotate180()
		{
			return new FlatHexPoint(-X, -Y);
		}

		public FlatHexPoint Rotate240()
		{
			return new FlatHexPoint(Y, -X - Y);
		}

		public FlatHexPoint Rotate300()
		{
			return new FlatHexPoint(X + Y, -X);
		}

		public FlatHexPoint ReflectAboutY()
		{
			return new FlatHexPoint(-X - Y, Y);
		}

		public FlatHexPoint ReflectAboutX()
		{
			return new FlatHexPoint(X + Y, -Y);
		}

		public FlatHexPoint Rotate60AndReflectAboutY()
		{
			return Rotate60().ReflectAboutY();
		}

		public FlatHexPoint Rotate120AndReflectAboutY()
		{
			return Rotate120().ReflectAboutY();
		}

		public FlatHexPoint Rotate180AndReflectAboutY()
		{
			return Rotate180().ReflectAboutY();
		}

		public FlatHexPoint Rotate240AndReflectAboutY()
		{
			return Rotate240().ReflectAboutY();
		}

		public FlatHexPoint Rotate300AndReflectAboutY()
		{
			return Rotate300().ReflectAboutY();
		}
		#endregion
	}
}