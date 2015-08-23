//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

// Auto-generated File

using System;

namespace Gamelogic.Grids
{

	/**
		@immutable
	*/
	[Serializable]
	[Immutable]
	public partial struct FlatTriPoint  : 
		ISplicedPoint<FlatTriPoint, PointyHexPoint>,
		ISplicedVectorPoint<FlatTriPoint, PointyHexPoint>
	{
		#region Constants
		public static readonly FlatTriPoint Zero = new FlatTriPoint(0, 0, 0);
		#endregion

		#region Fields
		private readonly PointyHexPoint basePoint;
		private readonly int index;
		#endregion

		#region Properties
		public int X
		{
			get
			{
				return basePoint.X;
			}
		}

		public int Y
		{
			get
			{
				return basePoint.Y;
			}
		}

		public int I
		{
			get
			{
				return index;
			}
		}

		public PointyHexPoint BasePoint
		{
			get
			{
				return basePoint;
			}
		}

		public int SpliceIndex
		{
			get 
			{
				return index;
			}
		}

		int IGridPoint<FlatTriPoint>.SpliceCount
		{	
			get 
			{
				return SpliceCount; 
			}
		}
		#endregion 

		#region Construction
		public FlatTriPoint(int x, int y, int index) :
			this(new PointyHexPoint(x, y), index)
		{
			basePoint = new PointyHexPoint(x, y);
			this.index = index;
		}

		private FlatTriPoint(PointyHexPoint basePoint, int index)
		{
			this.basePoint = basePoint;
			this.index = index;
		}
		#endregion 

		#region Arithmetic
		public FlatTriPoint Translate(PointyHexPoint other)
		{
			return new FlatTriPoint(basePoint.Translate(other), index);
		}

		public FlatTriPoint Negate()
		{
			return new FlatTriPoint(basePoint.Negate(), index);
		}

		public FlatTriPoint ScaleUp(int r)
		{
			return new FlatTriPoint(basePoint.ScaleUp(r), index);
		}

		public FlatTriPoint ScaleDown(int r)
		{
			return new FlatTriPoint(basePoint.ScaleDown(r), index);
		}

		public FlatTriPoint Subtract(PointyHexPoint other)
		{
			return new FlatTriPoint(basePoint.Subtract(other), index);
		}

		public FlatTriPoint MoveBy(FlatTriPoint other)
		{
			return Translate(other.BasePoint).IncIndex(other.index);
		}

		public FlatTriPoint MoveBackBy(FlatTriPoint other)
		{
			return Translate(other.BasePoint.Negate()).DecIndex(other.index);
		}
		#endregion
	
		#region Index Operations
		public FlatTriPoint IncIndex(int n)
		{
			return new FlatTriPoint(basePoint, GLMathf.Mod(index + n, SpliceCount));
		}
	
		public FlatTriPoint DecIndex(int n)
		{
			return IncIndex(-n);
		}
	
		public FlatTriPoint InvertIndex()
		{
			return new FlatTriPoint(basePoint, SpliceCount - index - 1);
		}
		#endregion

		#region Equality
		public bool Equals(FlatTriPoint other)
		{
			return basePoint.Equals(other.BasePoint) && (index == other.I);
		}
	
		public override bool Equals(object other)
		{
			if(other is FlatTriPoint)
			{
				return Equals((FlatTriPoint) other);
			}
			
			return false;
		}

		override public int GetHashCode()
		{
			return basePoint.GetHashCode() ^ index;
		}
		#endregion

		#region ToString
		override public string ToString()
		{
			return string.Format("[({0}, {1}), {2}]", X, Y, I);
		}
		#endregion 

		#region Operators
		public static bool operator ==(FlatTriPoint point1, FlatTriPoint point2)
		{
			return point1.Equals(point2);
		}

		public static bool operator !=(FlatTriPoint point1, FlatTriPoint point2)
		{
			return !point1.Equals(point2);
		}

		public static FlatTriPoint operator +(FlatTriPoint point)
		{
			return point;
		}

		public static FlatTriPoint operator -(FlatTriPoint point)
		{
			return point.Negate();
		}

		public static FlatTriPoint operator +(FlatTriPoint point1, PointyHexPoint point2)
		{
			return point1.Translate(point2);
		}

		public static FlatTriPoint operator -(FlatTriPoint point1, PointyHexPoint point2)
		{
			return point1.Subtract(point2);
		}

		public static FlatTriPoint operator *(FlatTriPoint point, int n)
		{
			return point.ScaleUp(n);
		}

		public static FlatTriPoint operator /(FlatTriPoint point, int n)
		{
			return point.ScaleDown(n);
		}
		#endregion

		#region Colorings
		/**
			Gives a coloring of the grid such that 
			if a point p has color k, then all points
			p + m[ux, 0 | 0] + n[vx, vy | 0] have the same color
			for any integers a and b.

			@since 1.7
		*/
		public int GetColor(int ux, int vx, int vy)
		{
			return BasePoint.GetColor(ux, vx, vy) * SpliceCount + I;
		}
		#endregion
	}


	/**
		@immutable
	*/
	[Serializable]
	[Immutable]
	public partial struct PointyTriPoint  : 
		ISplicedPoint<PointyTriPoint, FlatHexPoint>,
		ISplicedVectorPoint<PointyTriPoint, FlatHexPoint>
	{
		#region Constants
		public static readonly PointyTriPoint Zero = new PointyTriPoint(0, 0, 0);
		#endregion

		#region Fields
		private readonly FlatHexPoint basePoint;
		private readonly int index;
		#endregion

		#region Properties
		public int X
		{
			get
			{
				return basePoint.X;
			}
		}

		public int Y
		{
			get
			{
				return basePoint.Y;
			}
		}

		public int I
		{
			get
			{
				return index;
			}
		}

		public FlatHexPoint BasePoint
		{
			get
			{
				return basePoint;
			}
		}

		public int SpliceIndex
		{
			get 
			{
				return index;
			}
		}

		int IGridPoint<PointyTriPoint>.SpliceCount
		{	
			get 
			{
				return SpliceCount; 
			}
		}
		#endregion 

		#region Construction
		public PointyTriPoint(int x, int y, int index) :
			this(new FlatHexPoint(x, y), index)
		{
			basePoint = new FlatHexPoint(x, y);
			this.index = index;
		}

		private PointyTriPoint(FlatHexPoint basePoint, int index)
		{
			this.basePoint = basePoint;
			this.index = index;
		}
		#endregion 

		#region Arithmetic
		public PointyTriPoint Translate(FlatHexPoint other)
		{
			return new PointyTriPoint(basePoint.Translate(other), index);
		}

		public PointyTriPoint Negate()
		{
			return new PointyTriPoint(basePoint.Negate(), index);
		}

		public PointyTriPoint ScaleUp(int r)
		{
			return new PointyTriPoint(basePoint.ScaleUp(r), index);
		}

		public PointyTriPoint ScaleDown(int r)
		{
			return new PointyTriPoint(basePoint.ScaleDown(r), index);
		}

		public PointyTriPoint Subtract(FlatHexPoint other)
		{
			return new PointyTriPoint(basePoint.Subtract(other), index);
		}

		public PointyTriPoint MoveBy(PointyTriPoint other)
		{
			return Translate(other.BasePoint).IncIndex(other.index);
		}

		public PointyTriPoint MoveBackBy(PointyTriPoint other)
		{
			return Translate(other.BasePoint.Negate()).DecIndex(other.index);
		}
		#endregion
	
		#region Index Operations
		public PointyTriPoint IncIndex(int n)
		{
			return new PointyTriPoint(basePoint, GLMathf.Mod(index + n, SpliceCount));
		}
	
		public PointyTriPoint DecIndex(int n)
		{
			return IncIndex(-n);
		}
	
		public PointyTriPoint InvertIndex()
		{
			return new PointyTriPoint(basePoint, SpliceCount - index - 1);
		}
		#endregion

		#region Equality
		public bool Equals(PointyTriPoint other)
		{
			return basePoint.Equals(other.BasePoint) && (index == other.I);
		}
	
		public override bool Equals(object other)
		{
			if(other is PointyTriPoint)
			{
				return Equals((PointyTriPoint) other);
			}
			
			return false;
		}

		override public int GetHashCode()
		{
			return basePoint.GetHashCode() ^ index;
		}
		#endregion

		#region ToString
		override public string ToString()
		{
			return string.Format("[({0}, {1}), {2}]", X, Y, I);
		}
		#endregion 

		#region Operators
		public static bool operator ==(PointyTriPoint point1, PointyTriPoint point2)
		{
			return point1.Equals(point2);
		}

		public static bool operator !=(PointyTriPoint point1, PointyTriPoint point2)
		{
			return !point1.Equals(point2);
		}

		public static PointyTriPoint operator +(PointyTriPoint point)
		{
			return point;
		}

		public static PointyTriPoint operator -(PointyTriPoint point)
		{
			return point.Negate();
		}

		public static PointyTriPoint operator +(PointyTriPoint point1, FlatHexPoint point2)
		{
			return point1.Translate(point2);
		}

		public static PointyTriPoint operator -(PointyTriPoint point1, FlatHexPoint point2)
		{
			return point1.Subtract(point2);
		}

		public static PointyTriPoint operator *(PointyTriPoint point, int n)
		{
			return point.ScaleUp(n);
		}

		public static PointyTriPoint operator /(PointyTriPoint point, int n)
		{
			return point.ScaleDown(n);
		}
		#endregion

		#region Colorings
		/**
			Gives a coloring of the grid such that 
			if a point p has color k, then all points
			p + m[ux, 0 | 0] + n[vx, vy | 0] have the same color
			for any integers a and b.

			@since 1.7
		*/
		public int GetColor(int ux, int vx, int vy)
		{
			return BasePoint.GetColor(ux, vx, vy) * SpliceCount + I;
		}
		#endregion
	}


	/**
		@immutable
	*/
	[Serializable]
	[Immutable]
	public partial struct FlatRhombPoint  : 
		ISplicedPoint<FlatRhombPoint, FlatHexPoint>,
		ISplicedVectorPoint<FlatRhombPoint, FlatHexPoint>
	{
		#region Constants
		public static readonly FlatRhombPoint Zero = new FlatRhombPoint(0, 0, 0);
		#endregion

		#region Fields
		private readonly FlatHexPoint basePoint;
		private readonly int index;
		#endregion

		#region Properties
		public int X
		{
			get
			{
				return basePoint.X;
			}
		}

		public int Y
		{
			get
			{
				return basePoint.Y;
			}
		}

		public int I
		{
			get
			{
				return index;
			}
		}

		public FlatHexPoint BasePoint
		{
			get
			{
				return basePoint;
			}
		}

		public int SpliceIndex
		{
			get 
			{
				return index;
			}
		}

		int IGridPoint<FlatRhombPoint>.SpliceCount
		{	
			get 
			{
				return SpliceCount; 
			}
		}
		#endregion 

		#region Construction
		public FlatRhombPoint(int x, int y, int index) :
			this(new FlatHexPoint(x, y), index)
		{
			basePoint = new FlatHexPoint(x, y);
			this.index = index;
		}

		private FlatRhombPoint(FlatHexPoint basePoint, int index)
		{
			this.basePoint = basePoint;
			this.index = index;
		}
		#endregion 

		#region Arithmetic
		public FlatRhombPoint Translate(FlatHexPoint other)
		{
			return new FlatRhombPoint(basePoint.Translate(other), index);
		}

		public FlatRhombPoint Negate()
		{
			return new FlatRhombPoint(basePoint.Negate(), index);
		}

		public FlatRhombPoint ScaleUp(int r)
		{
			return new FlatRhombPoint(basePoint.ScaleUp(r), index);
		}

		public FlatRhombPoint ScaleDown(int r)
		{
			return new FlatRhombPoint(basePoint.ScaleDown(r), index);
		}

		public FlatRhombPoint Subtract(FlatHexPoint other)
		{
			return new FlatRhombPoint(basePoint.Subtract(other), index);
		}

		public FlatRhombPoint MoveBy(FlatRhombPoint other)
		{
			return Translate(other.BasePoint).IncIndex(other.index);
		}

		public FlatRhombPoint MoveBackBy(FlatRhombPoint other)
		{
			return Translate(other.BasePoint.Negate()).DecIndex(other.index);
		}
		#endregion
	
		#region Index Operations
		public FlatRhombPoint IncIndex(int n)
		{
			return new FlatRhombPoint(basePoint, GLMathf.Mod(index + n, SpliceCount));
		}
	
		public FlatRhombPoint DecIndex(int n)
		{
			return IncIndex(-n);
		}
	
		public FlatRhombPoint InvertIndex()
		{
			return new FlatRhombPoint(basePoint, SpliceCount - index - 1);
		}
		#endregion

		#region Equality
		public bool Equals(FlatRhombPoint other)
		{
			return basePoint.Equals(other.BasePoint) && (index == other.I);
		}
	
		public override bool Equals(object other)
		{
			if(other is FlatRhombPoint)
			{
				return Equals((FlatRhombPoint) other);
			}
			
			return false;
		}

		override public int GetHashCode()
		{
			return basePoint.GetHashCode() ^ index;
		}
		#endregion

		#region ToString
		override public string ToString()
		{
			return string.Format("[({0}, {1}), {2}]", X, Y, I);
		}
		#endregion 

		#region Operators
		public static bool operator ==(FlatRhombPoint point1, FlatRhombPoint point2)
		{
			return point1.Equals(point2);
		}

		public static bool operator !=(FlatRhombPoint point1, FlatRhombPoint point2)
		{
			return !point1.Equals(point2);
		}

		public static FlatRhombPoint operator +(FlatRhombPoint point)
		{
			return point;
		}

		public static FlatRhombPoint operator -(FlatRhombPoint point)
		{
			return point.Negate();
		}

		public static FlatRhombPoint operator +(FlatRhombPoint point1, FlatHexPoint point2)
		{
			return point1.Translate(point2);
		}

		public static FlatRhombPoint operator -(FlatRhombPoint point1, FlatHexPoint point2)
		{
			return point1.Subtract(point2);
		}

		public static FlatRhombPoint operator *(FlatRhombPoint point, int n)
		{
			return point.ScaleUp(n);
		}

		public static FlatRhombPoint operator /(FlatRhombPoint point, int n)
		{
			return point.ScaleDown(n);
		}
		#endregion

		#region Colorings
		/**
			Gives a coloring of the grid such that 
			if a point p has color k, then all points
			p + m[ux, 0 | 0] + n[vx, vy | 0] have the same color
			for any integers a and b.

			@since 1.7
		*/
		public int GetColor(int ux, int vx, int vy)
		{
			return BasePoint.GetColor(ux, vx, vy) * SpliceCount + I;
		}
		#endregion
	}


	/**
		@immutable
	*/
	[Serializable]
	[Immutable]
	public partial struct PointyRhombPoint  : 
		ISplicedPoint<PointyRhombPoint, PointyHexPoint>,
		ISplicedVectorPoint<PointyRhombPoint, PointyHexPoint>
	{
		#region Constants
		public static readonly PointyRhombPoint Zero = new PointyRhombPoint(0, 0, 0);
		#endregion

		#region Fields
		private readonly PointyHexPoint basePoint;
		private readonly int index;
		#endregion

		#region Properties
		public int X
		{
			get
			{
				return basePoint.X;
			}
		}

		public int Y
		{
			get
			{
				return basePoint.Y;
			}
		}

		public int I
		{
			get
			{
				return index;
			}
		}

		public PointyHexPoint BasePoint
		{
			get
			{
				return basePoint;
			}
		}

		public int SpliceIndex
		{
			get 
			{
				return index;
			}
		}

		int IGridPoint<PointyRhombPoint>.SpliceCount
		{	
			get 
			{
				return SpliceCount; 
			}
		}
		#endregion 

		#region Construction
		public PointyRhombPoint(int x, int y, int index) :
			this(new PointyHexPoint(x, y), index)
		{
			basePoint = new PointyHexPoint(x, y);
			this.index = index;
		}

		private PointyRhombPoint(PointyHexPoint basePoint, int index)
		{
			this.basePoint = basePoint;
			this.index = index;
		}
		#endregion 

		#region Arithmetic
		public PointyRhombPoint Translate(PointyHexPoint other)
		{
			return new PointyRhombPoint(basePoint.Translate(other), index);
		}

		public PointyRhombPoint Negate()
		{
			return new PointyRhombPoint(basePoint.Negate(), index);
		}

		public PointyRhombPoint ScaleUp(int r)
		{
			return new PointyRhombPoint(basePoint.ScaleUp(r), index);
		}

		public PointyRhombPoint ScaleDown(int r)
		{
			return new PointyRhombPoint(basePoint.ScaleDown(r), index);
		}

		public PointyRhombPoint Subtract(PointyHexPoint other)
		{
			return new PointyRhombPoint(basePoint.Subtract(other), index);
		}

		public PointyRhombPoint MoveBy(PointyRhombPoint other)
		{
			return Translate(other.BasePoint).IncIndex(other.index);
		}

		public PointyRhombPoint MoveBackBy(PointyRhombPoint other)
		{
			return Translate(other.BasePoint.Negate()).DecIndex(other.index);
		}
		#endregion
	
		#region Index Operations
		public PointyRhombPoint IncIndex(int n)
		{
			return new PointyRhombPoint(basePoint, GLMathf.Mod(index + n, SpliceCount));
		}
	
		public PointyRhombPoint DecIndex(int n)
		{
			return IncIndex(-n);
		}
	
		public PointyRhombPoint InvertIndex()
		{
			return new PointyRhombPoint(basePoint, SpliceCount - index - 1);
		}
		#endregion

		#region Equality
		public bool Equals(PointyRhombPoint other)
		{
			return basePoint.Equals(other.BasePoint) && (index == other.I);
		}
	
		public override bool Equals(object other)
		{
			if(other is PointyRhombPoint)
			{
				return Equals((PointyRhombPoint) other);
			}
			
			return false;
		}

		override public int GetHashCode()
		{
			return basePoint.GetHashCode() ^ index;
		}
		#endregion

		#region ToString
		override public string ToString()
		{
			return string.Format("[({0}, {1}), {2}]", X, Y, I);
		}
		#endregion 

		#region Operators
		public static bool operator ==(PointyRhombPoint point1, PointyRhombPoint point2)
		{
			return point1.Equals(point2);
		}

		public static bool operator !=(PointyRhombPoint point1, PointyRhombPoint point2)
		{
			return !point1.Equals(point2);
		}

		public static PointyRhombPoint operator +(PointyRhombPoint point)
		{
			return point;
		}

		public static PointyRhombPoint operator -(PointyRhombPoint point)
		{
			return point.Negate();
		}

		public static PointyRhombPoint operator +(PointyRhombPoint point1, PointyHexPoint point2)
		{
			return point1.Translate(point2);
		}

		public static PointyRhombPoint operator -(PointyRhombPoint point1, PointyHexPoint point2)
		{
			return point1.Subtract(point2);
		}

		public static PointyRhombPoint operator *(PointyRhombPoint point, int n)
		{
			return point.ScaleUp(n);
		}

		public static PointyRhombPoint operator /(PointyRhombPoint point, int n)
		{
			return point.ScaleDown(n);
		}
		#endregion

		#region Colorings
		/**
			Gives a coloring of the grid such that 
			if a point p has color k, then all points
			p + m[ux, 0 | 0] + n[vx, vy | 0] have the same color
			for any integers a and b.

			@since 1.7
		*/
		public int GetColor(int ux, int vx, int vy)
		{
			return BasePoint.GetColor(ux, vx, vy) * SpliceCount + I;
		}
		#endregion
	}


	/**
		@immutable
	*/
	[Serializable]
	[Immutable]
	public partial struct CairoPoint  : 
		ISplicedPoint<CairoPoint, PointyHexPoint>,
		ISplicedVectorPoint<CairoPoint, PointyHexPoint>
	{
		#region Constants
		public static readonly CairoPoint Zero = new CairoPoint(0, 0, 0);
		#endregion

		#region Fields
		private readonly PointyHexPoint basePoint;
		private readonly int index;
		#endregion

		#region Properties
		public int X
		{
			get
			{
				return basePoint.X;
			}
		}

		public int Y
		{
			get
			{
				return basePoint.Y;
			}
		}

		public int I
		{
			get
			{
				return index;
			}
		}

		public PointyHexPoint BasePoint
		{
			get
			{
				return basePoint;
			}
		}

		public int SpliceIndex
		{
			get 
			{
				return index;
			}
		}

		int IGridPoint<CairoPoint>.SpliceCount
		{	
			get 
			{
				return SpliceCount; 
			}
		}
		#endregion 

		#region Construction
		public CairoPoint(int x, int y, int index) :
			this(new PointyHexPoint(x, y), index)
		{
			basePoint = new PointyHexPoint(x, y);
			this.index = index;
		}

		private CairoPoint(PointyHexPoint basePoint, int index)
		{
			this.basePoint = basePoint;
			this.index = index;
		}
		#endregion 

		#region Arithmetic
		public CairoPoint Translate(PointyHexPoint other)
		{
			return new CairoPoint(basePoint.Translate(other), index);
		}

		public CairoPoint Negate()
		{
			return new CairoPoint(basePoint.Negate(), index);
		}

		public CairoPoint ScaleUp(int r)
		{
			return new CairoPoint(basePoint.ScaleUp(r), index);
		}

		public CairoPoint ScaleDown(int r)
		{
			return new CairoPoint(basePoint.ScaleDown(r), index);
		}

		public CairoPoint Subtract(PointyHexPoint other)
		{
			return new CairoPoint(basePoint.Subtract(other), index);
		}

		public CairoPoint MoveBy(CairoPoint other)
		{
			return Translate(other.BasePoint).IncIndex(other.index);
		}

		public CairoPoint MoveBackBy(CairoPoint other)
		{
			return Translate(other.BasePoint.Negate()).DecIndex(other.index);
		}
		#endregion
	
		#region Index Operations
		public CairoPoint IncIndex(int n)
		{
			return new CairoPoint(basePoint, GLMathf.Mod(index + n, SpliceCount));
		}
	
		public CairoPoint DecIndex(int n)
		{
			return IncIndex(-n);
		}
	
		public CairoPoint InvertIndex()
		{
			return new CairoPoint(basePoint, SpliceCount - index - 1);
		}
		#endregion

		#region Equality
		public bool Equals(CairoPoint other)
		{
			return basePoint.Equals(other.BasePoint) && (index == other.I);
		}
	
		public override bool Equals(object other)
		{
			if(other is CairoPoint)
			{
				return Equals((CairoPoint) other);
			}
			
			return false;
		}

		override public int GetHashCode()
		{
			return basePoint.GetHashCode() ^ index;
		}
		#endregion

		#region ToString
		override public string ToString()
		{
			return string.Format("[({0}, {1}), {2}]", X, Y, I);
		}
		#endregion 

		#region Operators
		public static bool operator ==(CairoPoint point1, CairoPoint point2)
		{
			return point1.Equals(point2);
		}

		public static bool operator !=(CairoPoint point1, CairoPoint point2)
		{
			return !point1.Equals(point2);
		}

		public static CairoPoint operator +(CairoPoint point)
		{
			return point;
		}

		public static CairoPoint operator -(CairoPoint point)
		{
			return point.Negate();
		}

		public static CairoPoint operator +(CairoPoint point1, PointyHexPoint point2)
		{
			return point1.Translate(point2);
		}

		public static CairoPoint operator -(CairoPoint point1, PointyHexPoint point2)
		{
			return point1.Subtract(point2);
		}

		public static CairoPoint operator *(CairoPoint point, int n)
		{
			return point.ScaleUp(n);
		}

		public static CairoPoint operator /(CairoPoint point, int n)
		{
			return point.ScaleDown(n);
		}
		#endregion

		#region Colorings
		/**
			Gives a coloring of the grid such that 
			if a point p has color k, then all points
			p + m[ux, 0 | 0] + n[vx, vy | 0] have the same color
			for any integers a and b.

			@since 1.7
		*/
		public int GetColor(int ux, int vx, int vy)
		{
			return BasePoint.GetColor(ux, vx, vy) * SpliceCount + I;
		}
		#endregion
	}

}
