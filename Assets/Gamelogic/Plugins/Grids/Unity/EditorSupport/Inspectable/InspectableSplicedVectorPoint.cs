//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using System;

namespace Gamelogic.Grids
{
	[Serializable]
	/**
		This class provides is a mutable class that can be used to construct
		partial vector points.
	
		It is provided for use in Unity's inspector.
	
		Typical usage us this:
	
		\code
		[Serializable]
		public MyClass
		{
			public InspectableVectorPoint playerStart;
				
			private PointyTriPoint playerPosition;
	
			public void Start()
			{
				playerPosition = playerStart.GetPointyTriPoint();
			}
		}
		\endcode
	
		
		
		@version1_0

		@ingroup UnityUtilities
	*/
	public class InspectableSplicedVectorPoint
	{
		public int x;
		public int y;
		public int index;

		public InspectableSplicedVectorPoint()
		{
			x = 0;
			y = 0;
			index = 0;
		}

		/**
			@version1_8
		*/
		public InspectableSplicedVectorPoint(PointyTriPoint point)
		{
			x = point.X;
			y = point.Y;
			index = point.I;
		}

		/**
			@version1_8
		*/
		public InspectableSplicedVectorPoint(FlatTriPoint point)
		{
			x = point.X;
			y = point.Y;
			index = point.I;
		}

		/**
			@version1_8
		*/
		public InspectableSplicedVectorPoint(PointyRhombPoint point)
		{
			x = point.X;
			y = point.Y;
			index = point.I;
		}

		/**
			@version1_8
		*/
		public InspectableSplicedVectorPoint(FlatRhombPoint point)
		{
			x = point.X;
			y = point.Y;
			index = point.I;
		}

		/**
			@version1_8
		*/
		public InspectableSplicedVectorPoint(CairoPoint point)
		{
			x = point.X;
			y = point.Y;
			index = point.I;
		}

		/**
			@version1_8
		*/
		public static InspectableSplicedVectorPoint Create<TPoint, TBasePoint>(ISplicedPoint<TPoint, TBasePoint> point)
			where TPoint : ISplicedVectorPoint<TPoint, TBasePoint>, IGridPoint<TPoint>
			where TBasePoint : IVectorPoint<TBasePoint>, IGridPoint<TBasePoint>
		{
			return new InspectableSplicedVectorPoint
			{
				x = point.X,
				y = point.Y,
				index =  point.I
			};
		}

		/**
			@version1_8
		*/
		public static InspectableSplicedVectorPoint Create<TBasePoint>(SplicedPoint<TBasePoint> point)
			where TBasePoint : IVectorPoint<TBasePoint>, IGridPoint<TBasePoint>
		{
			return new InspectableSplicedVectorPoint
			{
				x = point.BasePoint.X,
				y = point.BasePoint.Y,
				index = point.I
			};
		}
	
		public PointyTriPoint GetPointyTriPoint()
		{
			return new PointyTriPoint(x, y, index);
		}
	
		public FlatTriPoint GetPointyFlatPoint()
		{
			return new FlatTriPoint(x, y, index);
		}
	
		public PointyRhombPoint GetPointyRhombPoint()
		{
			return new PointyRhombPoint(x, y, index);
		}
	
		public FlatRhombPoint GetFlatRhombPoint()
		{
			return new FlatRhombPoint(x, y, index);
		}

		/**
			@version1_8
		*/
		public override string ToString()
		{
			return "(" + x + ", " + y + " | " + index + ")";
		}
	}
}