namespace Gamelogic.Grids.Examples
{
	public class PointyHexGrid_Test : CustomGridBuilder
	{
		public override IGrid<TCell, TPoint> MakeGrid<TCell, TPoint>()
		{
			var grid = PointyHexGrid<TCell>
				.BeginShape()
				.Rectangle(2, 2)
				.Translate(2, 2)
				.Union()
				.Rectangle(2, 2)
				.EndShape();

			return (IGrid<TCell, TPoint>) grid;
		}
	}
}
