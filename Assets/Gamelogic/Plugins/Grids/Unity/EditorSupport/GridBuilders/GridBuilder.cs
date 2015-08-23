
using Gamelogic.Diagnostics;
using UnityEngine;

namespace Gamelogic.Grids
{
	public class GridBuilder<TPoint> : GLMonoBehaviour
		where TPoint : IGridPoint<TPoint>
	{
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

		
		protected IMap3D<TPoint> map;
		protected IGrid<TileCell, TPoint> grid;

		[SerializeField]
		protected TileCell[] cells;
		#endregion

		#region Properties
		public TPoint MousePosition
		{
			get
			{
				Vector3 worldPosition = GridBuilderUtils.ScreenToWorld(gameObject, Input.mousePosition);

				return map[worldPosition];
			}
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
			if (Input.GetMouseButtonDown(0))
			{
				if (grid.Contains(MousePosition))
				{
					SendMessageToGridAndCell(MousePosition, "OnLeftClick");
					SendMessageToGridAndCell(MousePosition, "OnClick");
				}
			}

			if (Input.GetMouseButtonDown(1))
			{
				if (grid.Contains(MousePosition))
				{
					SendMessageToGridAndCell(MousePosition, "OnRightClick");
					SendMessageToGridAndCell(MousePosition, "OnClick");
				}
			}

			if (Input.GetMouseButtonDown(2))
			{
				if (grid.Contains(MousePosition))
				{
					SendMessageToGridAndCell(MousePosition, "OnMiddleClick");
					SendMessageToGridAndCell(MousePosition, "OnClick");
				}
			}
		}

		private void SendMessageToGridAndCell(TPoint point, string message)
		{
			SendMessage(message, point, SendMessageOptions.DontRequireReceiver);

			if (grid[point] != null)
			{
				grid[point].SendMessage(message, SendMessageOptions.DontRequireReceiver);
			}
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
	}
}
