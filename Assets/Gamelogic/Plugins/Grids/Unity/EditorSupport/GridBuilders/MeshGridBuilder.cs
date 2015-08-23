using System;
using System.Linq;
using Gamelogic.Diagnostics;
using UnityEngine;

namespace Gamelogic.Grids
{
	/// <summary>
	/// A cell that can be used in mesh grids. It contains
	/// information about the texture to use, and
	/// can be extended to contain other cell information.
	/// </summary>
	[Experimental]
	[Version(1, 14)]
	public class MeshCell
	{
		private readonly int textureIndex;

		public MeshCell(int textureIndex)
		{
			this.textureIndex = textureIndex;
		}

		public int GetTextureIndex()
		{
			return textureIndex;
		}
	}

	/**
		Base class for tile grid builders where grid is rendered as a single mesh.
		
		@ingroup UnityEditorSupport

		@tparam TPoint the type of point of the grid that this builder is building.
	*/
	[Experimental]
	[Version(1, 14)]
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public abstract class MeshGridBuilder<TPoint> : GLMonoBehaviour, ITileGrid<TPoint>,
		IGLScriptableObject
		where TPoint : IGridPoint<TPoint>
	{
		
		#region Fields
		[SerializeField]
		[Tooltip("The width and height of your shape (in cells).")]
		protected InspectableVectorPoint
			dimensions = new InspectableVectorPoint();

		[SerializeField]
		[Tooltip("The size of your shape (in cells).")]
		protected int size = 1;

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
			new ColorFunction { x0 = 1, x1 = 1, y1 = 1 };

		public int textureWidth = 4;
		public int textureHeight = 3;
		public Vector2 cellDimensions;

		#endregion
		#region Constants
		protected readonly Rect CenterRect = new Rect(0, 0, 0, 0);
		#endregion

		#region Fields
		[SerializeField]
		[Tooltip("When to update the grid")]
		protected UpdateType updateType = UpdateType.EditorManual;

		[SerializeField]
		[Tooltip("Whether the grid will respond to mouse clicks")]
		protected bool isInteractive;
		protected IMap3D<TPoint> customMap;
		protected IGrid<MeshCell, TPoint> grid;

		[SerializeField]
		protected MeshCell[] cells;

		protected IMeshMap<TPoint> meshMap; 
		#endregion

		#region Properties (Maintenance and Access)
		
		/// <summary>
		/// Returns the grid built with this builder.
		/// </summary>
		public IGrid<MeshCell, TPoint> Grid
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
			get { return customMap; }
			protected set { customMap = value; }
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
			@usedbyeditor
		*/
		//public MapPlane Plane
		//{
		//	get { return plane; }
		//	set { plane = value; }
		//}

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

		#region Properties
		public TPoint MousePosition
		{
			get
			{
				Vector3 worldPosition = GridBuilderUtils.ScreenToWorld(gameObject, Input.mousePosition);

				return customMap[worldPosition];
			}
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
		//protected abstract WindowedMap<TPoint> CreateWindowedMap();
		
		protected abstract IMap3D<TPoint> CreateMap();
		protected abstract IMeshMap<TPoint> CreateMeshMap();
 
		protected abstract Func<TPoint, int> GetColorFunc(int x0, int x1, int y1);

		#endregion

		#region Implementation
		private void MakeInnerGrid()
		{
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
			DestroyChildren();

			var mesh = new Mesh();
			var meshFilter = GetComponent<MeshFilter>();

			if (meshFilter == null)
			{
				throw new Exception("Add a MeshFilter");
			}

			meshFilter.mesh = mesh;
			var gridBehaviour = GetComponent<MeshGridBehaviour<TPoint>>();

			if (gridBehaviour == null)
			{
				grid.Fill(() => new MeshCell(GLRandom.Range(textureWidth*textureHeight)));
			}
			else
			{
				grid.Fill(gridBehaviour.CreateCell);
			}

			GenerateMesh(mesh);

			cells = grid.Values.ToArray();
		}

		private void DestroyChildren()
		{
#if UNITY_EDITOR
			transform.DestroyChildrenImmediate();
#else
		transform.DestroyChildren();
#endif
		}

		protected IMap3D<TPoint> GetAlignedMap(WindowedMap<TPoint> windowedHexMap)
		{
			Func<WindowedMap<TPoint>, IMap<TPoint>> alignmentFunc;
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

			return hexMap.To3DXY();
		}

		private void InitMap()
		{
			customMap = CreateMap();
		}

		protected IGrid<MeshCell, TPoint> GetCustomGrid()
		{
			var shapeBuilder = GetComponent<CustomGridBuilder>();

			if (shapeBuilder == null)
			{
				Debug.LogError(
					"You must attach a MonoBehaviour that inherits from <color=blue><b>CustomGridBuilder</b></color> to use a custom shape.",
					this);

				return null;
			}

			var newGrid = shapeBuilder.MakeGrid<MeshCell, TPoint>();

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

		#region Unity Methods
		public void Update()
		{
			if (isInteractive)
			{
				ProcessInput();
			}
		}
		#endregion

		private void ProcessInput()
		{
			if (Input.GetMouseButtonDown(0) 
				|| Input.GetMouseButtonDown(1)
				|| Input.GetMouseButtonDown(2))
			{
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				RaycastHit hit;

				if (Physics.Raycast(ray, out hit))
				{
					Vector3 worldPosition = transform.InverseTransformPoint(hit.point);

					var gridPoint = Map[worldPosition];

					GLDebug.Log(worldPosition);

					if (Grid.Contains(gridPoint))
					{
						if (Input.GetMouseButtonDown(0))
						{
							GLDebug.Log(gridPoint);
							SendMessageToGridAndCell(MousePosition, "OnLeftClick");
							SendMessageToGridAndCell(MousePosition, "OnClick");
						}

						if (Input.GetMouseButtonDown(1))
						{
							SendMessageToGridAndCell(MousePosition, "OnRightClick");
							SendMessageToGridAndCell(MousePosition, "OnClick");
						}

						if (Input.GetMouseButtonDown(2))
						{
							SendMessageToGridAndCell(MousePosition, "OnMiddleClick");
							SendMessageToGridAndCell(MousePosition, "OnClick");
						}
					}
				}
			}
		}

		private void SendMessageToGridAndCell(TPoint point, string message)
		{
			SendMessage(message, point, SendMessageOptions.DontRequireReceiver);
		}

		protected WindowedMap<TPoint> GetCustomMap()
		{
			var mapBuilder = GetComponent<CustomMapBuilder>();

			if (mapBuilder == null)
			{
				Debug.LogError("You must have a CustomMapBuilder component attached to your grid if you want to use a custom grid");

				return null;
			}

			return mapBuilder.CreateWindowedMap<TPoint>();
		}

		protected IMap3D<TPoint> GetCustomMap3D()
		{
			var mapBuilder = GetComponent<CustomMap3DBuilder>();

			if (mapBuilder == null)
			{
				Debug.LogError("You must have a CustomMapBuilder component attached to your grid if you want to use a custom grid");

				return null;
			}

			return mapBuilder.CreateMap<TPoint>();
		}

		protected IMeshMap<TPoint> GetCustomMeshMap()
		{
			var mapBuilder = GetComponent<CustomMap3DBuilder>();

			if (mapBuilder == null)
			{
				Debug.LogError("You must have a CustomMapBuilder component attached to your grid if you want to use a custom grid");

				return null;
			}

			return mapBuilder.CreateMeshMap<TPoint>();
		}

		private void GenerateMesh(Mesh mesh)
		{
			meshMap = CreateMeshMap();
			mesh.vertices = MakeVertices();
			mesh.uv = MakeUVs();
			mesh.triangles = MakeTriangles();
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			mesh.MarkDynamic();
			mesh.name = name + "Mesh";
		}

		private int[] MakeTriangles()
		{

			return grid
				.Select((point, i) => meshMap.GetTriangles(point, i))
				.SelectMany(x => x)
				.ToArray();
		}

		public void UpdateUVs()
		{
			var mesh = GetComponent<MeshFilter>().sharedMesh;

			mesh.uv = MakeUVs();
		}

		private Vector2[] MakeUVs()
		{
			return grid
				.SelectMany(p => 
					meshMap
						.GetUVs(p)
						.Select(uv => CalcUV(uv, grid[p].GetTextureIndex())))
				.ToArray();
		}

		private Vector3[] MakeVertices()
		{
			return grid.SelectMany(p => meshMap.GetVertices(p))
				.ToArray();
		}
		
		private Vector2 CalcUV(
			Vector2 fullUV, 
			int textureIndex)
		{
			int textureIndexX = textureIndex % textureWidth;
			int textureIndexY = textureIndex / textureHeight;

			float u = (fullUV.x + textureIndexX) / textureWidth;
			float v = (fullUV.y + textureIndexY) / textureHeight;

			return new Vector2(u, v);
		}
	}
}