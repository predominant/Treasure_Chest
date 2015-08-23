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
		Represents coordinates of the faces in a regular hexagonal
		lattice. The hexagons have the Pointy orientation.
	
		@immutable
	
		
		
		@version1_0

		@ingroup Points
	*/

	[Serializable]
	[Immutable]
	public partial struct PointyHexPoint :
		IGridPoint<PointyHexPoint>,
		IVectorPoint<PointyHexPoint>
	{
		#region Constants

		public static readonly PointyHexPoint East = new PointyHexPoint(1, 0);
		public static readonly PointyHexPoint NorthEast = new PointyHexPoint(0, 1);
		public static readonly PointyHexPoint NorthWest = new PointyHexPoint(-1, 1);
		public static readonly PointyHexPoint West = new PointyHexPoint(-1, 0);
		public static readonly PointyHexPoint SouthWest = new PointyHexPoint(0, -1);
		public static readonly PointyHexPoint SouthEast = new PointyHexPoint(1, -1);

		public static readonly IEnumerable<PointyHexPoint> MainDirections = new PointList<PointyHexPoint>
		{
			East,
			NorthEast,
			NorthWest,
			West,
			SouthWest,
			SouthEast
		};

		/**
			List of transforms, exlcuding the identity transform.
		 */

		private static readonly IEnumerable<Func<PointyHexPoint, PointyHexPoint>>
			PointTransforms = new List<Func<PointyHexPoint, PointyHexPoint>>
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
			get { return -(X + Y); }
		}

		#endregion

		#region Neighbord

		public IEnumerable<PointyHexPoint> GetNeighbors()
		{
			yield return this + East;
			yield return this + NorthEast;
			yield return this + NorthWest;

			yield return this + West;
			yield return this + SouthWest;
			yield return this + SouthEast;
		}

		#endregion

		#region Distance

		/**
		The magnitude of a hex point is the hex-distance between the point and the origin.

		This notation makes using hex points useful to use as vectors.
	*/

		public int Magnitude()
		{
			return (Mathf.Abs(X) + Mathf.Abs(Y) + Mathf.Abs(X + Y))/2;
		}

		#endregion

		#region Transformations

		public static bool IsEquivalentUnderTransformsAndTranslation(
			IEnumerable<PointyHexPoint> shape1,
			IEnumerable<PointyHexPoint> shape2)
		{
			return Algorithms.IsEquivalentUnderTransformsAndTranslation(
				shape1,
				shape2,
				PointTransforms,
				ToCanonicalPosition);
		}

		public static IEnumerable<PointyHexPoint> ToCanonicalPosition(IEnumerable<PointyHexPoint> shape)
		{
			PointyHexPoint firstPoint = shape.First();

			int minX = firstPoint.X;
			int minY = firstPoint.Y;

			foreach (PointyHexPoint point in shape.ButFirst())
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
				select new PointyHexPoint(point.X - minX, point.Y - minY);
		}

		public PointyHexPoint Rotate60()
		{
			return new PointyHexPoint(-Y, X + Y);
		}

		public PointyHexPoint Rotate120()
		{
			return new PointyHexPoint(-X - Y, X);
		}

		public PointyHexPoint Rotate180()
		{
			return new PointyHexPoint(-X, -Y);
		}

		public PointyHexPoint Rotate240()
		{
			return new PointyHexPoint(Y, -X - Y);
		}

		public PointyHexPoint Rotate300()
		{
			return new PointyHexPoint(X + Y, -X);
		}

		public PointyHexPoint ReflectAboutY()
		{
			return new PointyHexPoint(-X - Y, Y);
		}

		public PointyHexPoint ReflectAboutX()
		{
			return new PointyHexPoint(X + Y, -Y);
		}

		public PointyHexPoint Rotate60AndReflectAboutY()
		{
			return Rotate60().ReflectAboutY();
		}

		public PointyHexPoint Rotate120AndReflectAboutY()
		{
			return Rotate120().ReflectAboutY();
		}

		public PointyHexPoint Rotate180AndReflectAboutY()
		{
			return Rotate180().ReflectAboutY();
		}

		public PointyHexPoint Rotate240AndReflectAboutY()
		{
			return Rotate240().ReflectAboutY();
		}

		public PointyHexPoint Rotate300AndReflectAboutY()
		{
			return Rotate300().ReflectAboutY();
		}

		#endregion
	}
}