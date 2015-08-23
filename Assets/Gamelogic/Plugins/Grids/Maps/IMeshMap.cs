using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamelogic.Grids
{
	[Experimental]
	[Version(1, 14)]
	public interface IMeshMap<TPoint> where TPoint : IGridPoint<TPoint>
	{
		IMap<TPoint> Map2D { get; }
		
		IEnumerable<Vector3> GetVertices(TPoint point);

		IEnumerable<Vector2> GetUVs(TPoint point);

		IEnumerable<int> GetTriangles(TPoint point, int vertexIndex);
	}

	[Experimental]
	[Version(1, 14)]
	public class PointyHexMeshMap : IMeshMap<PointyHexPoint>
	{
		private static readonly Vector3 PreScale = new Vector3(2 / GLMathf.Sqrt3, 1, 1);
		private readonly Vector2 cellDimensions;
		private readonly PointyHexMap map;

		private static readonly Vector3[] VertexDirections =
		{
			new Vector3(0, 1f/2),
			new Vector3(GLMathf.Sqrt3/4, 1f/4),
			new Vector3(GLMathf.Sqrt3/4, -1f/4),
			new Vector3(0, -1f/2),
			new Vector3(-GLMathf.Sqrt3/4, -1f/4),
			new Vector3(-GLMathf.Sqrt3/4, 1f/4),
			Vector3.zero
		};

		private readonly int[] Triangles =
		{
			6, 0, 1,
			6, 1, 2,
			6, 2, 3,
			6, 3, 4,
			6, 4, 5,
			6, 5, 0
		};

		public PointyHexMeshMap(Vector2 cellDimensions, PointyHexMap map)
		{
			this.cellDimensions = cellDimensions;
			this.map = map;

		}

		public IMap<PointyHexPoint> Map2D
		{
			get { return map; }
			
		}

		public IEnumerable<Vector3> GetVertices(PointyHexPoint point)
		{
			return VertexDirections.Select(v => v
				.HadamardMul(PreScale)
				.HadamardMul(cellDimensions.To3DXY()) + map[point].To3DXY());
		}

		public IEnumerable<Vector2> GetUVs(PointyHexPoint point)
		{
			return VertexDirections.Select(v => new Vector2(v.x, v.y) + Vector2.one*0.5f)
				;
		}

		public IEnumerable<int> GetTriangles(PointyHexPoint point, int vertexIndex)
		{
			return Triangles.Select(t => t + 7*vertexIndex);
		}
	}

	[Experimental]
	[Version(1, 14)]
	public class PointyBrickMeshMap : IMeshMap<PointyHexPoint>
	{
		private readonly Vector2 cellDimensions;

		private readonly static Vector3[] VertexDirections =
		{
			new Vector3(-1/2f, -1/2f, 0),
			new Vector3(-1/2f, 1/2f, 0),
			new Vector3(1/2f, 1/2f, 0),
			new Vector3(1/2f, -1/2f, 0)
		};

		private readonly static int[] Triangles =
		{
			0, 1, 2,
			2, 3, 0
		};

		public PointyBrickMeshMap(Vector2 cellDimensions)
		{
			this.cellDimensions = cellDimensions;
		}

		public IMap<PointyHexPoint> Map2D { get; private set; }

		public IEnumerable<Vector3> GetVertices(PointyHexPoint point)
		{
			return VertexDirections.Select(v => v.HadamardMul(cellDimensions.To3DXY()));
		}

		public IEnumerable<Vector2> GetUVs(PointyHexPoint point)
		{
			return VertexDirections.Select(v => new Vector2(v.x, v.y) + Vector2.one*0.5f);
		}

		public IEnumerable<int> GetTriangles(PointyHexPoint point, int vertexIndex)
		{
			return Triangles.Select(t => t + 4*vertexIndex);
		}
	}

	[Experimental]
	[Version(1, 14)]
	public class FlatHexMeshMap : IMeshMap<FlatHexPoint>
	{
		private static readonly Vector3 PreScale = new Vector3(1, 2 / GLMathf.Sqrt3, 1);

		private readonly Vector2 cellDimensions;

		private readonly static Vector3[] VertexDirections =
		{
			new Vector3(0, 1f/2).YXZ(),
			new Vector3(GLMathf.Sqrt3/4, 1f/4).YXZ(),
			new Vector3(GLMathf.Sqrt3/4, -1f/4).YXZ(),
			new Vector3(0, -1f/2).YXZ(),
			new Vector3(-GLMathf.Sqrt3/4, -1f/4).YXZ(),
			new Vector3(-GLMathf.Sqrt3/4, 1f/4).YXZ(),
			Vector3.zero
		};
		
		private readonly static int[] Triangles =
		{
			6, 1, 0,
			6, 2, 1,
			6, 3, 2,
			6, 4, 3,
			6, 5, 4,
			6, 0, 5
		};

		public FlatHexMeshMap(Vector2 cellDimensions)
		{
			this.cellDimensions = cellDimensions;
		}


		public IMap<FlatHexPoint> Map2D { get; private set; }

		public IEnumerable<Vector3> GetVertices(FlatHexPoint point)
		{
			return VertexDirections.Select(v => v.
				HadamardMul(PreScale).
				HadamardMul(cellDimensions.To3DXY()));
		}

		public IEnumerable<Vector2> GetUVs(FlatHexPoint point)
		{
			return VertexDirections.Select(v => new Vector2(v.x, v.y) + Vector2.one*0.5f);
		}

		public IEnumerable<int> GetTriangles(FlatHexPoint point, int vertexIndex)
		{
			return Triangles.Select(t => t + 7*vertexIndex);
		}
	}

	[Experimental]
	[Version(1, 14)]
	public class FlatBrickMeshMap : IMeshMap<FlatHexPoint>
	{
		private readonly Vector2 cellDimensions;

		private readonly static Vector3[] VertexDirections =
		{
			new Vector3(-1/2f, -1/2f, 0).YXZ(),
			new Vector3(-1/2f, 1/2f, 0).YXZ(),
			new Vector3(1/2f, 1/2f, 0).YXZ(),
			new Vector3(1/2f, -1/2f, 0).YXZ()
		};

		private readonly static int[] Triangles =
		{
			0, 1, 2,
			2, 3, 0
		};

		public FlatBrickMeshMap(Vector2 cellDimensions)
		{
			this.cellDimensions = cellDimensions;
		}

		public IMap<FlatHexPoint> Map2D { get; private set; }

		public IEnumerable<Vector3> GetVertices(FlatHexPoint point)
		{
			return VertexDirections.Select(v => v.HadamardMul(cellDimensions.To3DXY()));
		}

		public IEnumerable<Vector2> GetUVs(FlatHexPoint point)
		{
			return VertexDirections.Select(v => new Vector2(v.x, v.y) + Vector2.one*0.5f);
		}

		public IEnumerable<int> GetTriangles(FlatHexPoint point, int vertexIndex)
		{
			return Triangles.Select(t => t + 4*vertexIndex);
		}
	}

	[Experimental]
	[Version(1, 14)]
	public class RectMeshMap : IMeshMap<RectPoint>
	{
		private readonly Vector2 cellDimensions;

		private readonly static Vector3[] VertexDirections =
		{
			new Vector3(-1/2f, -1/2f, 0),
			new Vector3(-1/2f, 1/2f, 0),
			new Vector3(1/2f, 1/2f, 0),
			new Vector3(1/2f, -1/2f, 0)
		};

		private readonly static int[] Triangles =
		{
			0, 1, 2,
			2, 3, 0
		};

		public RectMeshMap(Vector2 cellDimensions)
		{
			this.cellDimensions = cellDimensions;
		}

		public IMap<RectPoint> Map2D { get; private set; }

		public IEnumerable<Vector3> GetVertices(RectPoint point)
		{
			return VertexDirections
				.Select(v=>v.HadamardMul(cellDimensions.To3DXY()));
		}

		public IEnumerable<Vector2> GetUVs(RectPoint point)
		{
			return VertexDirections.Select(v => new Vector2(v.x, v.y) + Vector2.one*0.5f);
		}

		public IEnumerable<int> GetTriangles(RectPoint point, int vertexIndex)
		{
			return Triangles.Select(t => t + 4*vertexIndex);
		}
	}

	[Experimental]
	[Version(1, 14)]
	public class DiamondMeshMap : IMeshMap<DiamondPoint>
	{
		private readonly Vector2 cellDimensions;

		private readonly static Vector3[] VertexDirections =
		{
			new Vector3(-1/2f, 0, 0),
			new Vector3(0, 1/2f, 0),
			new Vector3(1/2f, 0, 0),
			new Vector3(0, -1/2f, 0)
		};

		private readonly static int[] Triangles =
		{
			0, 1, 2,
			2, 3, 0
		};

		public DiamondMeshMap(Vector2 cellDimensions)
		{
			this.cellDimensions = cellDimensions;
		}

		public IMap<DiamondPoint> Map2D { get; private set; }

		public IEnumerable<Vector3> GetVertices(DiamondPoint point)
		{
			return VertexDirections.Select(v => v.HadamardMul(cellDimensions.To3DXY()));
		}

		public IEnumerable<Vector2> GetUVs(DiamondPoint point)
		{
			return VertexDirections.Select(v => new Vector2(v.x, v.y) + Vector2.one*0.5f);
		}

		public IEnumerable<int> GetTriangles(DiamondPoint point, int vertexIndex)
		{
			return Triangles.Select(t => t + 4*vertexIndex);
		}
	}
}