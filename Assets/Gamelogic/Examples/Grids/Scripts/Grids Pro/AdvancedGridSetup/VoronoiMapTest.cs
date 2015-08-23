using UnityEngine;

namespace Gamelogic.Grids.Examples
{

	public class VoronoiMapTest : GLMonoBehaviour
	{
		public Texture2D plane;

		private readonly Vector2 CellDimensions = new Vector2(30, 30);

		public void Start()
		{
			var grid = LineGrid<int>
				.BeginShape()
				.Segment(10)
				.EndShape();

			var map = new ArchimedeanSpiralMap(CellDimensions, grid);

			var voronoiMap = new VoronoiMap<LinePoint>(grid, map);

			ExampleUtils.PaintScreenTexture(plane, voronoiMap, n => GLMathf.Mod(n, 12));
		}
	}
}