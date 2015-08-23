//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

// Auto-generated File

using System;
using System.Collections.Generic;

namespace Gamelogic.Grids
{

	public partial class PointyHexGrid<TCell>
	{
		private static readonly PointyHexPoint[] SpiralIteratorDirections = 
		{
			PointyHexPoint.East,
			PointyHexPoint.NorthEast,
			PointyHexPoint.NorthWest,
			PointyHexPoint.West,
			PointyHexPoint.SouthWest,
			PointyHexPoint.SouthEast
		};

		/**
			An iterator that spirals anti-clockwise around the grid origin (0, 0). 

			@version1_7
		*/
		[Experimental]
		public IEnumerable<PointyHexPoint> GetSpiralIterator(int ringCount)
		{
			return GetSpiralIterator(PointyHexPoint.Zero, ringCount);
		}

		/**
			An iterator that spirals anti-clockwise around the given origin.

			@code
			int k = 0;

			foreach(var point in grid.GetSpiralIterator(point, 3))
			{
				grid[point].name = k.ToString();
			}
			@endcode

			@version1_10
		*/
		[Experimental]
		public IEnumerable<PointyHexPoint> GetSpiralIterator(PointyHexPoint origin, int ringCount) 
		{			
			var hexPoint = origin;
			yield return hexPoint;
		
			for (var k = 1; k < ringCount; k++)
			{
				hexPoint = new PointyHexPoint(0, 0);
				hexPoint = hexPoint + SpiralIteratorDirections[4] * (k);
			
				for (var i = 0; i < 6; i++)
				{
					for (var j = 0; j < k; j++)
					{
						if (Contains(hexPoint))
						{
							yield return hexPoint;
						}

						hexPoint = hexPoint + SpiralIteratorDirections[i];
					}
				}
			}
		}
		
		/**
			Returns a new grid, wrapped along a Hexagon with the given side length. 
			
			@since 1.7
		*/
		public static WrappedGrid<TCell, PointyHexPoint> WrappedHexagon(int side)
		{
			var grid = Hexagon(side);
			var wrapper = new PointyHexHexagonWrapper(side);
			var wrappedGrid = new WrappedGrid<TCell, PointyHexPoint>(grid, wrapper);

			return wrappedGrid;
		}		
	}

	/**
		@since 1.7
		@ingroup Scaffolding
	*/
	[Experimental]
	public class PointyHexHexagonWrapper : IPointWrapper<PointyHexPoint>
	{
		private readonly PointyHexPoint[] wrappedPoints;
		private readonly Func<PointyHexPoint, int> colorFunc; 

		public PointyHexHexagonWrapper(int n)
		{
			int colorCount = 3*n*n - 3*n + 1;
			int x1 = 3*n - 2;

			wrappedPoints = new PointyHexPoint[colorCount];
			var grid = PointyHexGrid<int>.Hexagon(n);
			
			colorFunc = x => x.GetColor(colorCount, x1, 1);

			foreach (var point in grid)
			{
				int color = colorFunc(point);
				wrappedPoints[color] = point;
			}
		}

		public PointyHexPoint Wrap(PointyHexPoint p)
		{
			return wrappedPoints[colorFunc(p)];
		}
	}

	public partial struct PointyHexPoint
	{
		#region Geometry
		/**
			Whether this point is inside the half plane x >= x0.

			@sa http://devmag.org.za/2013/08/31/geometry-with-hex-coordinates/

			@since 1.3
		*/
		public bool IsInPositiveHalfPlaneX(int x0)
		{
			return X >= x0;
		}
		
		/**
			Whether this point is inside the half plane x <= x0. 

			@sa http://devmag.org.za/2013/08/31/geometry-with-hex-coordinates/
			
			@since 1.3
		*/
		public bool IsInNegativeHalfPlaneX(int x0)
		{
			return X <= x0;
		}

		/**
			Whether this point is inside the half plane y >= x0.

			@sa http://devmag.org.za/2013/08/31/geometry-with-hex-coordinates/

			@since 1.3
		*/
		public bool IsInPositiveHalfPlaneY(int y0)
		{
			return Y >= y0;
		}
		
		/**
			Whether this point is inside the half plane y <= y0.

			@sa http://devmag.org.za/2013/08/31/geometry-with-hex-coordinates/
			
			@since 1.3
		*/
		public bool IsInNegativeHalfPlaneY(int y0)
		{
			return Y <= y0;
		}

		/**
			Whether this point is inside the half plane z >= z0.

			@sa http://devmag.org.za/2013/08/31/geometry-with-hex-coordinates/
			
			@since 1.3
		*/
		public bool IsInPositiveHalfPlaneZ(int z0)
		{
			return Z >= z0;
		}
		
		/**
			Whether this point is inside the half plane z <= z0.

			@sa http://devmag.org.za/2013/08/31/geometry-with-hex-coordinates/
			
			@since 1.3
		*/
		public bool IsInNegativeHalfPlaneZ(int z0)
		{
			return Z <= z0;
		}

		/**
			Whether this point is in the hexagon with the given radius 
			and center at the origin.

			The origin is considered the hexagon with zero radius.

			@sa http://devmag.org.za/2013/08/31/geometry-with-hex-coordinates/
			
			@since 1.3
		*/
		//TODO: How should we make this consistent with 
		//Exisiting shape code (where < is used instead of <=
		public bool IsInsideHexagon(int radius)
		{
			return Magnitude() <= radius;
		}

		/**
			Whether this point is in the hexagon with the given radius 
			and given center.

			A single point is considered a hexagon with zero radius.

			@sa http://devmag.org.za/2013/08/31/geometry-with-hex-coordinates/
			
			@since 1.3
		*/
		public bool IsInsideHexagon(PointyHexPoint center, int radius)
		{
			return (this - center).Magnitude() <= radius;
		}

		/**
			Returns whether this point is in either 
			the up triangle given by

			(x >= x0 && y >= y0 && z >= z0)

			or the down triangle given by

			(z <= x0 && y <= y0 && z <= z0)

			@sa http://devmag.org.za/2013/08/31/geometry-with-hex-coordinates/
			
			@since 1.3
		*/
		public bool IsInsideTriangle(int x0, int y0, int z0)
		{
			return 
				(X >= x0 && Y >= y0 && Z >= z0) ||
				(X <= x0 && Y <= y0 && Z <= z0);
		}

		/**
			Whether this point is inside the polygon described by
			
			(x0 <= X <= x1) && (y0 <= Y <= y1) && (z0 <= Z <= z1)

			@sa http://devmag.org.za/2013/08/31/geometry-with-hex-coordinates/
			
			@since 1.3
		*/
		public bool IsInsidePolygon(int x0, int x1, int y0, int y1, int z0, int z1)
		{
			return
				x0 <= X && X <= x1 &&
				y0 <= Y && Y <= y1 &&
				z0 <= Z && Z <= z1;
		}
		#endregion

		#region Colorings
		/**
			Generates a coloring with the following pattern:
			
			<img src="fh_color1_1.png" />
			
			Light Blue corresponds to 0. 
			Light Green corresponds to 1. 
			Light Yellow corresponds to 2. 
			Light Red corresponds to 3. 
			Blue corresponds to 4. 
			Green corresponds to 5. 
			Yellow corresponds to 6. 
			
		*/
		public int GetColor1_1()
		{
			return 0;
		}
		
		/**
			Generates a coloring with the following pattern:
			
			<img src="fh_color1_2.png" />
			
			Light Blue corresponds to 0. 
			Light Green corresponds to 1. 
			Light Yellow corresponds to 2. 
			Light Red corresponds to 3. 
			Blue corresponds to 4. 
			Green corresponds to 5. 
			Yellow corresponds to 6. 
		*/
		public int GetColor1_2()
		{
			return GetColor1_3() % 2;
		}
		
		/**
			Generates a coloring with the following pattern:
			
			<img src="fh_color1_3.png" />
			
			Light Blue corresponds to 0. 
			Light Green corresponds to 1. 
			Light Yellow corresponds to 2. 
			Light Red corresponds to 3. 
			Blue corresponds to 4. 
			Green corresponds to 5. 
			Yellow corresponds to 6. 
			
		*/
		public int GetColor1_3()
		{
			return GetColor(3, 1, 1);
		}
		
		/**
			Generates a coloring with the following pattern:
			
			<img src="fh_color2_2.png" />
			
			Light Blue corresponds to 0. 
			Light Green corresponds to 1. 
			Light Yellow corresponds to 2. 
			Light Red corresponds to 3. 
			Blue corresponds to 4. 
			Green corresponds to 5. 
			Yellow corresponds to 6. 
			
		*/
		public int GetColor2_2()
		{
			return (2 + (X * Y) % 2) % 2;
		}
		
		/**
			Generates a coloring with the following pattern:
			
			<img src="fh_color2_4.png" />
			
			Light Blue corresponds to 0. 
			Light Green corresponds to 1. 
			Light Yellow corresponds to 2. 
			Light Red corresponds to 3. 
			Blue corresponds to 4. 
			Green corresponds to 5. 
			Yellow corresponds to 6. 
			
		*/
		public int GetColor2_4()
		{
			return GetColor(2, 0, 2);
			//return GLMathf.Mod(GLMathf.Mod(X, 2) + 2 * GLMathf.Mod(Y, 2), 4);
		}
		
		/**
			Generates a coloring with the following pattern:
			
			<img src="fh_color3_2.png" />
			
			Light Blue corresponds to 0. 
			Light Green corresponds to 1. 
			Light Yellow corresponds to 2. 
			Light Red corresponds to 3. 
			Blue corresponds to 4. 
			Green corresponds to 5. 
			Yellow corresponds to 6. 
			
		*/
		public int GetColor3_2()
		{
			return GetColor3_7() / 6;
		}
		
		/**
			Generates a coloring with the following pattern:
			
			<img src="fh_color3_7.png" />
			
			Light Blue corresponds to 0. 
			Light Green corresponds to 1. 
			Light Yellow corresponds to 2. 
			Light Red corresponds to 3. 
			Blue corresponds to 4. 
			Green corresponds to 5. 
			Yellow corresponds to 6. 
			
		*/
		public int GetColor3_7()
		{
			return GetColor(7, 2, 1);
			//return GLMathf.Mod(GLMathf.Mod(X, 7) + 5 * Y, 7);
		}

		/**
			@since 1.7
		*/
		public int GetColor5_5()
		{
			return GetColor(5, 2, 1); 
		}

		/**
			@since 1.7
		*/
		public int GetColor6()
		{
			return GetColor(3, 0, 2);
		}
		#endregion
	}


	public partial class FlatHexGrid<TCell>
	{
		private static readonly FlatHexPoint[] SpiralIteratorDirections = 
		{
			FlatHexPoint.North,
			FlatHexPoint.NorthWest,
			FlatHexPoint.SouthWest,
			FlatHexPoint.South,
			FlatHexPoint.SouthEast,
			FlatHexPoint.NorthEast
		};

		/**
			An iterator that spirals anti-clockwise around the grid origin (0, 0). 

			@version1_7
		*/
		[Experimental]
		public IEnumerable<FlatHexPoint> GetSpiralIterator(int ringCount)
		{
			return GetSpiralIterator(FlatHexPoint.Zero, ringCount);
		}

		/**
			An iterator that spirals anti-clockwise around the given origin.

			@code
			int k = 0;

			foreach(var point in grid.GetSpiralIterator(point, 3))
			{
				grid[point].name = k.ToString();
			}
			@endcode

			@version1_10
		*/
		[Experimental]
		public IEnumerable<FlatHexPoint> GetSpiralIterator(FlatHexPoint origin, int ringCount) 
		{			
			var hexPoint = origin;
			yield return hexPoint;
		
			for (var k = 1; k < ringCount; k++)
			{
				hexPoint = new FlatHexPoint(0, 0);
				hexPoint = hexPoint + SpiralIteratorDirections[4] * (k);
			
				for (var i = 0; i < 6; i++)
				{
					for (var j = 0; j < k; j++)
					{
						if (Contains(hexPoint))
						{
							yield return hexPoint;
						}

						hexPoint = hexPoint + SpiralIteratorDirections[i];
					}
				}
			}
		}
		
		/**
			Returns a new grid, wrapped along a Hexagon with the given side length. 
			
			@since 1.7
		*/
		public static WrappedGrid<TCell, FlatHexPoint> WrappedHexagon(int side)
		{
			var grid = Hexagon(side);
			var wrapper = new FlatHexHexagonWrapper(side);
			var wrappedGrid = new WrappedGrid<TCell, FlatHexPoint>(grid, wrapper);

			return wrappedGrid;
		}		
	}

	/**
		@since 1.7
		@ingroup Scaffolding
	*/
	[Experimental]
	public class FlatHexHexagonWrapper : IPointWrapper<FlatHexPoint>
	{
		private readonly FlatHexPoint[] wrappedPoints;
		private readonly Func<FlatHexPoint, int> colorFunc; 

		public FlatHexHexagonWrapper(int n)
		{
			int colorCount = 3*n*n - 3*n + 1;
			int x1 = 3*n - 2;

			wrappedPoints = new FlatHexPoint[colorCount];
			var grid = FlatHexGrid<int>.Hexagon(n);
			
			colorFunc = x => x.GetColor(colorCount, x1, 1);

			foreach (var point in grid)
			{
				int color = colorFunc(point);
				wrappedPoints[color] = point;
			}
		}

		public FlatHexPoint Wrap(FlatHexPoint p)
		{
			return wrappedPoints[colorFunc(p)];
		}
	}

	public partial struct FlatHexPoint
	{
		#region Geometry
		/**
			Whether this point is inside the half plane x >= x0.

			@sa http://devmag.org.za/2013/08/31/geometry-with-hex-coordinates/

			@since 1.3
		*/
		public bool IsInPositiveHalfPlaneX(int x0)
		{
			return X >= x0;
		}
		
		/**
			Whether this point is inside the half plane x <= x0. 

			@sa http://devmag.org.za/2013/08/31/geometry-with-hex-coordinates/
			
			@since 1.3
		*/
		public bool IsInNegativeHalfPlaneX(int x0)
		{
			return X <= x0;
		}

		/**
			Whether this point is inside the half plane y >= x0.

			@sa http://devmag.org.za/2013/08/31/geometry-with-hex-coordinates/

			@since 1.3
		*/
		public bool IsInPositiveHalfPlaneY(int y0)
		{
			return Y >= y0;
		}
		
		/**
			Whether this point is inside the half plane y <= y0.

			@sa http://devmag.org.za/2013/08/31/geometry-with-hex-coordinates/
			
			@since 1.3
		*/
		public bool IsInNegativeHalfPlaneY(int y0)
		{
			return Y <= y0;
		}

		/**
			Whether this point is inside the half plane z >= z0.

			@sa http://devmag.org.za/2013/08/31/geometry-with-hex-coordinates/
			
			@since 1.3
		*/
		public bool IsInPositiveHalfPlaneZ(int z0)
		{
			return Z >= z0;
		}
		
		/**
			Whether this point is inside the half plane z <= z0.

			@sa http://devmag.org.za/2013/08/31/geometry-with-hex-coordinates/
			
			@since 1.3
		*/
		public bool IsInNegativeHalfPlaneZ(int z0)
		{
			return Z <= z0;
		}

		/**
			Whether this point is in the hexagon with the given radius 
			and center at the origin.

			The origin is considered the hexagon with zero radius.

			@sa http://devmag.org.za/2013/08/31/geometry-with-hex-coordinates/
			
			@since 1.3
		*/
		//TODO: How should we make this consistent with 
		//Exisiting shape code (where < is used instead of <=
		public bool IsInsideHexagon(int radius)
		{
			return Magnitude() <= radius;
		}

		/**
			Whether this point is in the hexagon with the given radius 
			and given center.

			A single point is considered a hexagon with zero radius.

			@sa http://devmag.org.za/2013/08/31/geometry-with-hex-coordinates/
			
			@since 1.3
		*/
		public bool IsInsideHexagon(FlatHexPoint center, int radius)
		{
			return (this - center).Magnitude() <= radius;
		}

		/**
			Returns whether this point is in either 
			the up triangle given by

			(x >= x0 && y >= y0 && z >= z0)

			or the down triangle given by

			(z <= x0 && y <= y0 && z <= z0)

			@sa http://devmag.org.za/2013/08/31/geometry-with-hex-coordinates/
			
			@since 1.3
		*/
		public bool IsInsideTriangle(int x0, int y0, int z0)
		{
			return 
				(X >= x0 && Y >= y0 && Z >= z0) ||
				(X <= x0 && Y <= y0 && Z <= z0);
		}

		/**
			Whether this point is inside the polygon described by
			
			(x0 <= X <= x1) && (y0 <= Y <= y1) && (z0 <= Z <= z1)

			@sa http://devmag.org.za/2013/08/31/geometry-with-hex-coordinates/
			
			@since 1.3
		*/
		public bool IsInsidePolygon(int x0, int x1, int y0, int y1, int z0, int z1)
		{
			return
				x0 <= X && X <= x1 &&
				y0 <= Y && Y <= y1 &&
				z0 <= Z && Z <= z1;
		}
		#endregion

		#region Colorings
		/**
			Generates a coloring with the following pattern:
			
			<img src="fh_color1_1.png" />
			
			Light Blue corresponds to 0. 
			Light Green corresponds to 1. 
			Light Yellow corresponds to 2. 
			Light Red corresponds to 3. 
			Blue corresponds to 4. 
			Green corresponds to 5. 
			Yellow corresponds to 6. 
			
		*/
		public int GetColor1_1()
		{
			return 0;
		}
		
		/**
			Generates a coloring with the following pattern:
			
			<img src="fh_color1_2.png" />
			
			Light Blue corresponds to 0. 
			Light Green corresponds to 1. 
			Light Yellow corresponds to 2. 
			Light Red corresponds to 3. 
			Blue corresponds to 4. 
			Green corresponds to 5. 
			Yellow corresponds to 6. 
		*/
		public int GetColor1_2()
		{
			return GetColor1_3() % 2;
		}
		
		/**
			Generates a coloring with the following pattern:
			
			<img src="fh_color1_3.png" />
			
			Light Blue corresponds to 0. 
			Light Green corresponds to 1. 
			Light Yellow corresponds to 2. 
			Light Red corresponds to 3. 
			Blue corresponds to 4. 
			Green corresponds to 5. 
			Yellow corresponds to 6. 
			
		*/
		public int GetColor1_3()
		{
			return GetColor(3, 1, 1);
		}
		
		/**
			Generates a coloring with the following pattern:
			
			<img src="fh_color2_2.png" />
			
			Light Blue corresponds to 0. 
			Light Green corresponds to 1. 
			Light Yellow corresponds to 2. 
			Light Red corresponds to 3. 
			Blue corresponds to 4. 
			Green corresponds to 5. 
			Yellow corresponds to 6. 
			
		*/
		public int GetColor2_2()
		{
			return (2 + (X * Y) % 2) % 2;
		}
		
		/**
			Generates a coloring with the following pattern:
			
			<img src="fh_color2_4.png" />
			
			Light Blue corresponds to 0. 
			Light Green corresponds to 1. 
			Light Yellow corresponds to 2. 
			Light Red corresponds to 3. 
			Blue corresponds to 4. 
			Green corresponds to 5. 
			Yellow corresponds to 6. 
			
		*/
		public int GetColor2_4()
		{
			return GetColor(2, 0, 2);
			//return GLMathf.Mod(GLMathf.Mod(X, 2) + 2 * GLMathf.Mod(Y, 2), 4);
		}
		
		/**
			Generates a coloring with the following pattern:
			
			<img src="fh_color3_2.png" />
			
			Light Blue corresponds to 0. 
			Light Green corresponds to 1. 
			Light Yellow corresponds to 2. 
			Light Red corresponds to 3. 
			Blue corresponds to 4. 
			Green corresponds to 5. 
			Yellow corresponds to 6. 
			
		*/
		public int GetColor3_2()
		{
			return GetColor3_7() / 6;
		}
		
		/**
			Generates a coloring with the following pattern:
			
			<img src="fh_color3_7.png" />
			
			Light Blue corresponds to 0. 
			Light Green corresponds to 1. 
			Light Yellow corresponds to 2. 
			Light Red corresponds to 3. 
			Blue corresponds to 4. 
			Green corresponds to 5. 
			Yellow corresponds to 6. 
			
		*/
		public int GetColor3_7()
		{
			return GetColor(7, 2, 1);
			//return GLMathf.Mod(GLMathf.Mod(X, 7) + 5 * Y, 7);
		}

		/**
			@since 1.7
		*/
		public int GetColor5_5()
		{
			return GetColor(5, 2, 1); 
		}

		/**
			@since 1.7
		*/
		public int GetColor6()
		{
			return GetColor(3, 0, 2);
		}
		#endregion
	}

}
