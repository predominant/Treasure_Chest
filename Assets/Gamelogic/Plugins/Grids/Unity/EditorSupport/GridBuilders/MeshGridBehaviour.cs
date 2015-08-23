namespace Gamelogic.Grids
{
	/// <summary>
	/// Attach this component to Mesh Grid Builders to chnage the grid's behaviour.
	/// </summary>
	/// <typeparam name="TPoint">The point type of the grid.</typeparam>
	[Experimental]
	[Version(1, 14)]
	public class MeshGridBehaviour<TPoint> : GridBehaviour<TPoint> 
		where TPoint : IGridPoint<TPoint>
	{
		public virtual MeshCell CreateCell(TPoint point)
		{
			return new MeshCell(0);
		}
	}
}