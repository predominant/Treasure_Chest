//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



namespace Gamelogic.Grids
{
	/**
		This wrapper wraps pointy hex points horizontally. 
		
		@version1_7
		@ingroup Scaffolding
	*/
	[Experimental]
	public class PointyHexHorizontalRectangleWrapper : IPointWrapper<PointyHexPoint>
	{
		readonly int width;

		public PointyHexHorizontalRectangleWrapper(int width)
		{
			this.width = width;
		}

		public PointyHexPoint Wrap(PointyHexPoint point)
		{
			return new PointyHexPoint(GLMathf.Mod(point.X + GLMathf.Div(point.Y, 2), width) - GLMathf.Div(point.Y, 2), point.Y);
		}
	}
}