namespace Gamelogic.Grids
{
	/**
		Inherit from this class to implement custom grid shapes.

		This class should return a grid. To have the grid builder 
		used this grid, you should set the Shape to Custom in the 
		editor, and attached this script to the same game object
		as the grid builder.

		@version1_8
		@ingroup UnityEditorSupport
	*/
	public abstract class CustomGridBuilder : GLMonoBehaviour
	{
		/**
			@implementers note:
				Check the type; if it is "your" type, return the grid.
				Otherwise return null.
		*/

		public virtual IGrid<TCell, TPoint> MakeGrid<TCell, TPoint>()
			where TPoint : IGridPoint<TPoint>

		{
			return null;
		}
	}
}