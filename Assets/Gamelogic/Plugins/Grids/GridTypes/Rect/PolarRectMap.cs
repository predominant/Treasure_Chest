//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		This map can be used with a horizontally wrapped RectGrid.

		For now, alignment does not work as with the other maps.

		Example:
		<code>
			IGrid<RectPoint> grid;
			IMap3D map;

			private void BuildGrid()
			{
				grid = RectGrid<TCell>.HorizontallyWrappedParallelogram(width, height);
				map = new PolarRectMap(Vector3.zero, 10, 110, newRectPoint(5, 10).To3DXY();

				foreach(var point in grid)
				{
					var cell = Instantiate(cellPrefab);
					cell.transform.localPosition = map[
				}
			}

			public void Click(Vector3 worldPoint)
			{
				var gridPoint = map[worldPoint] 
				if(grid.Contains(gridPoint))
				{
					ClickCell(grid[gridPoint]);
				}
			}		
		</code>

		@version1_7
		@ingroup Maps
	*/

	[Experimental]
	public class PolarRectMap : AbstractMap<RectPoint>, IPolarMap<RectPoint>
	{
		#region Fields

		private readonly float sectorAngleRad;

		#endregion

		#region Properties

		public float SectorAngle
		{
			get;
			private set;
		}

		public Vector2 Center
		{
			get;
			private set;
		}

		public VectorPoint SectorsAndBands
		{
			get;
			private set;
		}

		public float InnerRadius
		{
			get;
			private set;
		}

		public float OuterRadius
		{
			get;
			private set;
		}

		#endregion

		#region Construction

		public PolarRectMap(Vector2 center, float innerRadius, float outerRadius, VectorPoint sectorsAndBands)
			: base(Vector2.one)
		{
			Center = center;
			InnerRadius = innerRadius;
			OuterRadius = outerRadius;
			SectorsAndBands = sectorsAndBands;
			sectorAngleRad = (2f*Mathf.PI)/SectorsAndBands.X;
			SectorAngle = 360f/SectorsAndBands.X;
		}

		#endregion

		#region AbstractMap Implementation

		public override RectPoint RawWorldToGrid(Vector2 worldPoint)
		{
			float angleRad = Mathf.Atan2(worldPoint.y - Center.y, worldPoint.x - Center.x);

			if (angleRad < 0)
			{
				angleRad += 2*Mathf.PI;
			}

			float radius = (new Vector2(worldPoint.x - Center.x, worldPoint.y - Center.y)).magnitude;

			int n = Mathf.FloorToInt((radius - InnerRadius)/(OuterRadius - InnerRadius)*SectorsAndBands.Y);
			int m = Mathf.FloorToInt(angleRad/sectorAngleRad);

			if (m < 0)
			{
				m += SectorsAndBands.X;
			}

			return new RectPoint(m, n);
		}

		public override Vector2 GridToWorld(RectPoint gridPoint)
		{
			float m = gridPoint.X;
			float n = gridPoint.Y;
			float angleRad = (m/SectorsAndBands.X)*2f*Mathf.PI + (Mathf.PI/SectorsAndBands.X);
			float radius = (n/SectorsAndBands.Y)*(OuterRadius - InnerRadius) + InnerRadius +
			               (OuterRadius - InnerRadius)/(2f*SectorsAndBands.Y);
			float x = radius*Mathf.Cos(angleRad) + Center.x;
			float y = radius*Mathf.Sin(angleRad) + Center.y;

			return new Vector2(x, y);
		}

		#endregion

		#region Interface

		public float GetStartAngleZ(RectPoint gridPoint)
		{
			float m = gridPoint.X;

			float angle = m*SectorAngle;

			return angle;
		}

		public float GetEndAngleZ(RectPoint gridPoint)
		{
			float angle = GetStartAngleZ(gridPoint) + SectorAngle;

			return angle;
		}

		public float GetInnerRadius(RectPoint gridPoint)
		{
			return (gridPoint.Y/(float) SectorsAndBands.Y)*(OuterRadius - InnerRadius) + InnerRadius;
		}

		public float GetOuterRadius(RectPoint gridPoint)
		{
			return ((gridPoint.Y + 1)/(float) SectorsAndBands.Y)*(OuterRadius - InnerRadius) + InnerRadius;
		}

		#endregion
	}
}