//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



namespace Gamelogic.Grids
{
	/**
		This class provides is a mutable class that can be used to construct
		VectorPoints.

		It is provided for use in Unity's inspector.

		Typical usage us this:

			[Serializable]
			public MyClass
			{
				public InspectableVectorPoint playerStart;
			
				private PointyHexPoint playerPosition;

				public void Start()
				{
					playerPosition = playerStart.GetPointyHexPoint();
				}
			}

		
		
		@version1_0

		@ingroup UnityUtilities
	*/
	public partial class InspectableVectorPoint
	{
		/**
			@version1_8
		*/
		public InspectableVectorPoint(PointyHexPoint point)
		{
			x = point.X;
			y = point.Y;
		}

		public PointyHexPoint GetPointyHexPoint()
		{
			return new PointyHexPoint(x, y);
		}
	
		/**
			@version1_8
		*/
		public InspectableVectorPoint(FlatHexPoint point)
		{
			x = point.X;
			y = point.Y;
		}

		public FlatHexPoint GetFlatHexPoint()
		{
			return new FlatHexPoint(x, y);
		}
	}
}