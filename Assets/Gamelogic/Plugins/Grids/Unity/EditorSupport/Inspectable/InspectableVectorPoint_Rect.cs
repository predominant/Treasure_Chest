//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

namespace Gamelogic.Grids
{
	public partial class InspectableVectorPoint
	{
		/**
			@version1_8
		*/
		public InspectableVectorPoint(RectPoint point)
		{
			x = point.X;
			y = point.Y;
		}

		/**
			@version1_11
		*/

		public InspectableVectorPoint(DiamondPoint point)
		{
			x = point.X;
			y = point.Y;
		}

		public RectPoint GetRectPoint()
		{
			return new RectPoint(x, y);
		}
	
		public DiamondPoint GetDiamondPoint()
		{
			return new DiamondPoint(x, y);
		}
	}
}