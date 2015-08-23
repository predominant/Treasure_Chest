using System;
using System.Linq;
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Base class for polar tile grid builders.

		@version1_8

		@ingroup UnityEditorSupport
	*/
	public abstract class PolarTileGridBuilder<TPoint> : GridBuilder<TPoint>, ITileGrid<TPoint>, IGLScriptableObject
		where TPoint : IGridPoint<TPoint>
	{
		#region Constants
		protected readonly Rect CenterRect = new Rect(0, 0, 0, 0);
		#endregion

		#region Fields
		[SerializeField]
		[Tooltip("The object that will be used to display each cell.")]
		protected MeshTileCell cellPrefab
			= null;

		[SerializeField]
		public PolarGridProperties polarGridProperties;

		[SerializeField]
		[Tooltip("The width and height of your shape (in cells).")]
		protected InspectableVectorPoint
			dimensions = new InspectableVectorPoint();

		[SerializeField]
		[Tooltip("The size of your shape (in cells).")]
		protected int size = 1;

		[SerializeField]
		[Tooltip("The 2D plane in which your grid will lie.")]
		protected MapPlane plane = MapPlane.XY;

		[SerializeField]
		[Tooltip("How the anchor of the grid lies relative to the grid.")]
		protected MapAlignment
			alignment = MapAlignment.MiddleCenter;

		[SerializeField]
		[Tooltip("A factor that controls the spacing between cells.")]
		protected float cellSpacingFactor
			= 1;

		[SerializeField]
		[Tooltip("Whether to set cells DefaultColors")]
		protected bool useColor;

		[SerializeField] [Tooltip("The DefaultColors to use to color cells.")] protected Color[] colors 
			= GridBuilderUtils.DefaultColors; 

		[SerializeField]
		[Tooltip("The color function to use to color cells.")]
		protected ColorFunction colorFunction =
			new ColorFunction {x0 = 1, x1 = 1, y1 = 1};

		protected IPolarMap<TPoint> polarMap;
		#endregion

		#region Properties
		public IGrid<TileCell, TPoint> Grid
		{
			get
			{
				if (grid == null)
				{
					MakeInnerGrid();
				}

				return grid;
			}

			protected set { grid = value; }
		}

		public IMap3D<TPoint> Map
		{
			get { return map; }
			protected set { map = value; }
		}

		/**
			Returns the dimensions for this grid if it makes sense for the current shape.
 
			(The dimension field is hidden in the inspector if it does not make sense).

			@usedbyeditor
		*/
		public VectorPoint Dimensions
		{
			get { return dimensions.GetVectorPoint(); }

			set
			{
				dimensions = new InspectableVectorPoint
				{
					x = value.X,
					y = value.Y
				};
			}
		}

		/**
			Returns the size for this grid if it makes sense for the current shape.
 
			(The dimension field is hidden in the inspector if it does not make sense).

			@usedbyeditor
		*/
		public int Size
		{
			get { return size; }
			set { size = value; }
		}

		/**
			Returns the cell prefab this builder uses to build the grid.
		*/
		public MeshTileCell CellPrefab
		{
			get { return cellPrefab; }
			set { cellPrefab = value; }
		}

		/**
			@usedbyeditor
		*/
		public MapPlane Plane
		{
			get { return plane; }
			set { plane = value; }
		}

		/**
			@usedbyeditor
		*/
		public MapAlignment Alignment
		{
			get { return alignment; }
			set { alignment = value; }
		}

		/**
			@usedbyeditor
		*/
		public float CellSpacingFactor
		{
			get { return cellSpacingFactor; }
			set { cellSpacingFactor = value; }
		}

		/**
			@usedbyeditor
		*/
		public bool UseColor
		{
			get { return useColor; }
			set { useColor = value; }
		}

		/**
			@usedbyeditor
		*/
		public Color[] Colors
		{
			get { return colors; }
			set { colors = value; }
		}

		/**
			@usedbyeditor
		*/
		public ColorFunction ColorFunction
		{
			get { return colorFunction; }
			set { colorFunction = value; }
		}

		#endregion

		#region Unity Callbacks

		public void Start()
		{
#if UNITY_IPHONE
			if (Application.isPlaying)
			{
				if (!__CompilerHints.__CompilerHint__Rect()) return;
				if (!__CompilerHints.__CompilerHint__Diamond()) return;
				if (!__CompilerHints.__CompilerHint__PointyHex()) return;
				if (!__CompilerHints.__CompilerHint__FlatHex()) return;

				if (!__CompilerHints.__CompilerHint__PointyTri()) return;
				if (!__CompilerHints.__CompilerHint__FlatTri()) return;
				if (!__CompilerHints.__CompilerHint__PointyRhomb()) return;
				if (!__CompilerHints.__CompilerHint__FlatRhomb()) return;

				if (!__CompilerHints.__CompilerHint__Cairo()) return;
			}
#endif
			if (cellPrefab == null)
			{
				WarnNoCellPrefab();

				return;
			}

			InitGrid();
			InitMap();

			if (cells != null && cells.Length == grid.Count())
			{
				RelinkCells();
			}
			else
			{
				SetupGrid();
			}

			InitUserGrid();
		}

		private void RelinkCells()
		{
			var gridPoints = grid.ToArray();

			for (int i = 0; i < cells.Length; i++)
			{
				grid[gridPoints[i]] = cells[i];
			}
		}
		
		#endregion

		#region Abstract methods

		protected abstract void InitGrid();
		//protected abstract WindowedMap<TPoint> CreateWindowedMap();
		protected abstract IPolarMap<TPoint> CreatePolarMap();
		protected abstract Func<TPoint, int> GetColorFunc(int x0, int x1, int y1);

		#endregion

		#region Implementation
		private void InitUserGrid()
		{
			var gridInitializer = GetComponent<GridBehaviour<TPoint>>();

			if (gridInitializer != null)
			{
				gridInitializer.InitGrid();
			}
		}

		private void MakeInnerGrid()
		{
			InitGrid();
			InitMap();
			RelinkCells();
			InitUserGrid();
		}

		public void __UpdatePresentation(bool forceUpdate)
		{
			if (forceUpdate || updateType == UpdateType.EditorAuto)
			{
				InitGrid();
				InitMap();
				SetupGrid();
				InitUserGrid();
			}
		}

		private void SetupGrid()
		{
			if (cellPrefab == null)
			{
				WarnNoCellPrefab();

				return;
			}

#if UNITY_EDITOR
			transform.DestroyChildrenImmediate();
#else
		transform.DestroyChildren();
#endif

			var colorFunc = GetColorFunc(colorFunction.x0, colorFunction.x1, colorFunction.y1);

			foreach (var point in grid)
			{
				var cell = Instantiate(cellPrefab);
				cell.transform.parent = transform;
				cell.transform.localPosition = Vector3.zero;
				
				float innerRadius = polarMap.GetInnerRadius(point) + polarGridProperties.border/2;
				float outerRadius = polarMap.GetOuterRadius(point) - polarGridProperties.border/2;
				float startAngle = polarMap.GetStartAngleZ(point);
				float endAngle = polarMap.GetEndAngleZ(point) - polarGridProperties.border*Mathf.Rad2Deg/outerRadius;
				int quadCount = Mathf.CeilToInt(outerRadius*2*Mathf.PI/(polarGridProperties.quadSize*Dimensions.X));

				var mesh = new Mesh();
				MeshUtils.MakeBandedSector(mesh, startAngle, endAngle, innerRadius, outerRadius, quadCount, v => v);
				cell.GetComponent<MeshFilter>().sharedMesh = mesh;
				
				if (useColor)
				{
					var color = colors[colorFunc(point)%colors.Length];
					cell.Color = color;
				}
				else
				{
					cell.Color = Color.white;
				}

				cell.name = point.ToString();
				cell.__CenterOffset = map[point];

				grid[point] = cell;
			}

			cells = grid.Values.ToArray();
		}

		private void InitMap()
		{
			Func<WindowedMap<TPoint>, IMap<TPoint>> alignmentFunc;

			polarMap = CreatePolarMap();


			WindowedMap<TPoint> windowedHexMap = polarMap.WithWindow(CenterRect);

			switch (alignment)
			{
				case MapAlignment.TopLeft:
					alignmentFunc = windowedMap => windowedMap.AlignTopLeft(grid);
					break;
				case MapAlignment.TopCenter:
					alignmentFunc = windowedMap => windowedMap.AlignTopCenter(grid);
					break;
				case MapAlignment.TopRight:
					alignmentFunc = windowedMap => windowedMap.AlignRight(grid);
					break;
				case MapAlignment.MiddleLeft:
					alignmentFunc = windowedMap => windowedMap.AlignMiddleLeft(grid);
					break;
				case MapAlignment.MiddleCenter:
					alignmentFunc = windowedMap => windowedMap.AlignMiddleCenter(grid);
					break;
				case MapAlignment.MiddleRight:
					alignmentFunc = windowedMap => windowedMap.AlignMiddleRight(grid);
					break;
				case MapAlignment.BottomLeft:
					alignmentFunc = windowedMap => windowedMap.AlignBottomLeft(grid);
					break;
				case MapAlignment.BottomCenter:
					alignmentFunc = windowedMap => windowedMap.AlignBottomCenter(grid);
					break;
				case MapAlignment.BottomRight:
					alignmentFunc = windowedMap => windowedMap.AlignBottomRight(grid);
					break;
				case MapAlignment.None:
					alignmentFunc = windowedMap => windowedMap;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var hexMap = alignmentFunc(windowedHexMap).Scale(cellSpacingFactor);

			switch (plane)
			{
				case MapPlane.XY:
					map = hexMap.To3DXY();
					break;
				case MapPlane.XZ:
					map = hexMap.To3DXZ();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		private void WarnNoCellPrefab()
		{
#if !UNITY_EDITOR
		Debug.LogWarning("No cell prefab set");
#endif
		}
		#endregion
	}
}