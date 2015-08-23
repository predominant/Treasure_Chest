using System;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Gamelogic.Grids
{
	/**
		Base class for tile grid builders where all the tiles are the same.
		
		@version1_8		
		@ingroup UnityEditorSupport

		@tparam TPoint the type of point of the grid that this builder is building.
	*/
	public abstract class TileGridBuilder<TPoint> : GridBuilder<TPoint>, ITileGrid<TPoint>, 
		IGLScriptableObject
		where TPoint : IGridPoint<TPoint>
	{
		#region Fields
		[SerializeField]
		[Tooltip("The object that will be used to display each cell.")]
		protected TileCell cellPrefab =
			null;

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
		protected Vector2 cellSpacingFactor
			= Vector2.one;

		[SerializeField]
		[Tooltip("Whether to set cells colors")]
		protected bool useColor;

		[SerializeField]
		[Tooltip("The colors to use to color cells.")]
		[ContextMenuItem("Reset", "ResetColors")]
		protected Color[] colors = GridBuilderUtils.DefaultColors;

		[SerializeField]
		[Tooltip("The color function to use to color cells.")]
		protected ColorFunction colorFunction =
			new ColorFunction {x0 = 1, x1 = 1, y1 = 1};

		

		#endregion

		#region Properties (Maintenance and Access)
		/**
			Returns the grid built with this builder.
		*/
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

		/**
			Returns the map this builder used to built the grid.
		*/
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
		public TileCell CellPrefab
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
		public Vector2 CellSpacingFactor
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

		#region Utility Properties
		/*
		public TileCell this[TPoint point]
		{
			get { return grid[point]; }
			set { throw new InvalidOperationException("You cannot assign cells in a TileGridBuilder."); }
		}

		object IGrid<TPoint>.this[TPoint point]
		{
			get { return this[point]; }
			set { this[point] = (TileCell) value; }
		}
		*/
		

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
		#endregion

		#region Abstract methods

		protected abstract void InitGrid();
		protected abstract WindowedMap<TPoint> CreateWindowedMap();
		protected abstract Func<TPoint, int> GetColorFunc(int x0, int x1, int y1);

		#endregion

		#region Implementation
		private void MakeInnerGrid()
		{
			if (cellPrefab == null) return;

			InitGrid();
			InitMap();
			RelinkCells();
			InitUserGrid();
		}

		/**
			@usedbyeditor
		*/
		public void __UpdatePresentation(bool forceUpdate)
		{
			if (cellPrefab == null) return;

			if (forceUpdate || updateType == UpdateType.EditorAuto)
			{
				InitGrid();
				InitMap();
				SetupGrid();
				InitUserGrid();
			}
		}

		private void InitUserGrid()
		{
			var gridInitializer = GetComponent<GridBehaviour<TPoint>>();

			if (gridInitializer != null)
			{
				gridInitializer.InitGrid();
			}
		}

		private void RelinkCells()
		{
			var gridPoints = grid.ToArray();

			for (int i = 0; i < cells.Length; i++)
			{
				grid[gridPoints[i]] = cells[i];
			}
		}

		private void SetupGrid()
		{
			if (cellPrefab == null)
			{
				WarnNoCellPrefab();

				return;
			}

			DestroyChildren();

			var colorFunc = GetColorFunc(colorFunction.x0, colorFunction.x1, colorFunction.y1);

			foreach (var point in grid)
			{
				var cell = GridBuilderUtils.Instantiate(cellPrefab);

				cell.transform.parent = transform;
				cell.transform.localScale = Vector3.one;
				cell.transform.localPosition = map[point];

				if (useColor)
				{
					var color = colors[colorFunc(point)%colors.Length];
					cell.Color = color;
				}

				cell.name = point.ToString();

				int spliceCount = point.SpliceCount;
				int index = point.SpliceIndex;

				cell.SetAngle(-360f/spliceCount*index);

				grid[point] = cell;
			}

			cells = grid.Values.ToArray();
		}

		private void WarnNoCellPrefab()
		{
#if !UNITY_EDITOR
		Debug.LogWarning("No cell prefab set");
#endif
		}

		private void DestroyChildren()
		{
#if UNITY_EDITOR
			transform.DestroyChildrenImmediate();
#else
		transform.DestroyChildren();
#endif
		}

		private void InitMap()
		{
			Func<WindowedMap<TPoint>, IMap<TPoint>> alignmentFunc;
			WindowedMap<TPoint> windowedHexMap = CreateWindowedMap();

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

		protected IGrid<TileCell, TPoint> GetCustomGrid()
		{
			var shapeBuilder = GetComponent<CustomGridBuilder>();

			if (shapeBuilder == null)
			{
				Debug.LogError(
					"You must attach a MonoBehaviour that inherits from <color=blue><b>CustomGridBuilder</b></color> to use a custom shape.",
					this);

				return null;
			}

			var newGrid = shapeBuilder.MakeGrid<TileCell, TPoint>();

			if (newGrid == null)
			{
				Debug.LogError(
					"The custom grid builder returned null instead of a grid. Make sure it supports grids that take points f the correct type.", this);
			}

			return newGrid;
		}
		#endregion

		#region Inspector
		private void ResetColors()
		{
			colors = GridBuilderUtils.DefaultColors;
		}
		#endregion
	}
}