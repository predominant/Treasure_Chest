using System;
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Provides methods for making specialised meshes 
		that can be used with grids.

		@version1_7
	*/
	public static class MeshUtils
	{
		/**
			These meshes can be used with polar grids to render cells.
		*/
		[Experimental]
		public static void MakeBandedSector(
			Mesh mesh,
			float startAngle,
			float endAngle,
			float innerRadius,
			float outerRadius,
			int quadCount, 
			Func<Vector2, Vector2> transform)
		{
			mesh.Clear();
			mesh.vertices = MakeBandedSectorVertices(startAngle, endAngle, innerRadius, outerRadius, quadCount, transform);
			
			mesh.uv = MakeBandedSectorUVs(quadCount);
			mesh.triangles = MakeBandedSectorTriangles(quadCount);
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
		}

		private static Vector3[] MakeBandedSectorVertices(
			float startAngle,
			float endAngle,
			float innerRadius,
			float outerRadius,
			int quadCount,
			Func<Vector2, Vector2> transform)
		{
			var vertices = new Vector3[2 * quadCount + 2];
			float currentAngleRad = startAngle * Mathf.Deg2Rad;
			float angleIncrementRad = (endAngle - startAngle) * Mathf.Deg2Rad / quadCount;

			for (int i = 0; i < 2 * quadCount + 2; i++)
			{
				var x = Mathf.Cos(currentAngleRad);
				var y = Mathf.Sin(currentAngleRad);

				if (i % 2 == 0)
				{
					vertices[i] = (transform(new Vector2(
						innerRadius * x,
						innerRadius * y))).To3DXY();
				}
				else
				{
					vertices[i] = (transform(new Vector2(
						outerRadius * x,
						outerRadius * y))).To3DXY();

					currentAngleRad += angleIncrementRad;
				}
			}

			return vertices;
		}

		private static Vector2[] MakeBandedSectorUVs(int quadCount)
		{
			var uvs = new Vector2[2 * quadCount + 2];
			float currentUV = 0;
			float uvIncrement = 1 / (float)quadCount;

			for (int i = 0; i < 2 * quadCount + 2; i++)
			{
				if (i % 2 == 0)
				{
					uvs[i] = new Vector2(currentUV, 0);
				}
				else
				{
					uvs[i] = new Vector2(currentUV, 1);
					currentUV += uvIncrement;
				}
			}

			return uvs;
		}

		private static int[] MakeBandedSectorTriangles(int quadCount)
		{
			var triangles = new int[2 * 3 * quadCount];
			int triangleIndex = 0;

			for (int i = 0; i < quadCount * 2; i++)
			{
				if (i % 2 == 0)
				{
					triangles[triangleIndex] = i + 2;
					triangles[triangleIndex + 1] = i + 1;
					triangles[triangleIndex + 2] = i;
				}
				else
				{
					triangles[triangleIndex] = i + 1;
					triangles[triangleIndex + 1] = i + 2;
					triangles[triangleIndex + 2] = i;
				}

				triangleIndex += 3;
			}

			return triangles;
		}
	}
}
