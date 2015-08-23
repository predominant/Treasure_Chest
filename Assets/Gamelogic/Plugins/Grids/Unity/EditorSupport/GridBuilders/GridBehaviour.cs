namespace Gamelogic.Grids
{
	/**
		Extend from this class to hook in your own grid initialisation code. This is also a 
		useful place for other logic that interacts with the grid (typically, your game logic). 
		It has properties to access the grid and map.

		You cannot use this to customize the shape or map of the grid (instead, use 
		CustomGridBuilder and CustomMapBuilder).

		

		@version1_8
		
		@ingroup UnityEditorSupport
	*/
	public class GridBehaviour<TPoint> : GLMonoBehaviour
		where TPoint : IGridPoint<TPoint>
	{
		private TileGridBuilder<TPoint> gridBuilder = null;

		/**
			Get's the mouse position in Grid coordinates.
			
			Currently it is only useful for 2D grids, rendered with 
			orthographic cameras.
		*/
		public TPoint MousePosition
		{
			get { return GridBuilder.MousePosition; }
		}

		/**
			Returns the grid builder attached to the same game object as this
			grid behaviour.

			(It's provided, but you will mostly need only the Grid and Map.)
		*/
		public TileGridBuilder<TPoint> GridBuilder
		{
			get
			{
				if (gridBuilder == null)
				{
					gridBuilder = GetComponent<TileGridBuilder<TPoint>>();
				}

				return gridBuilder;
			}
		}

		/**
			The map used to build the grid.
		*/
		public IMap3D<TPoint> Map
		{
			get { return GridBuilder.Map; }
		}

		/**
			The grid data structure, containing cells as elements.

			(It's likely that this will return a grid of a different 
			(more general) cell type in the future).
			
		*/
		public IGrid<TileCell, TPoint> Grid
		{
			get { return GridBuilder.Grid; }
		}

		/**
			When this behaviour is attached to a grid builder, this method is called
			once the grid is created, and all cells (tiles) have been instantiated.

			Override this to implement custom initialisation code. (You can access the 
			grid through the Grid property).
		*/
		public virtual void InitGrid()
		{
			//NOP
		}
	}
}
