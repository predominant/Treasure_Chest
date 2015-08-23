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
		Class for making RectGrids in different shapes.
		
		@link_constructing_grids
			
		@copyright Gamelogic.
		@author Herman Tulleken
		@since 1.0
		@see @ref AbstractOp
		@ingroup BuilderInterface
	*/
	public partial class RectOp<TCell> : AbstractOp<ShapeStorageInfo<RectPoint>>
	{
		public RectOp(){}

		public RectOp(
			ShapeStorageInfo<RectPoint> leftShapeInfo,
			Func<ShapeStorageInfo<RectPoint>, ShapeStorageInfo<RectPoint>, ShapeStorageInfo<RectPoint>> combineShapeInfo) :
			base(leftShapeInfo, combineShapeInfo)
		{}

		/**
			Use this function to create shapes to ensure they fit into memory.
		
			The test function can test shapes anywhere in space. If you specify the bottom corner 
			(in terms of the storage rectangle), the shape is automatically translated in memory 
			to fit, assuming memory width and height is big enough.

			Strategy for implementing new shapes:
				- First, determine the test function.
				- Next, draw a storage rectangle that contains the shape.
				- Determine the storgae rectangle width and height.
				- Finally, determine the grid-space coordinate of the left bottom corner of the storage rectangle.
		
			Then define your function as follows:

			\code{cs}
			public RectShapeInfo<TCell> MyShape()
			{
				Shape(stargeRectangleWidth, storageRectangleHeight, isInsideMyShape, storageRectangleBottomleft);
			}
			\endcode

			\param width The widh of the storage rectangle
			\param height The height of the storage rectangle
			\param isInside A function that returns true if a passed point lies inside the shape being defined
			\param bottomLeftCorner The grid-space coordinate of the bottom left corner of the storage rect.

		*/
		public RectShapeInfo<TCell> Shape(int width, int height, Func<RectPoint, bool> isInside, RectPoint bottomLeftCorner)
		{
			var shapeInfo = MakeShapeStorageInfo<RectPoint>(width, height, x=>isInside(x + bottomLeftCorner));
			return new RectShapeInfo<TCell>(shapeInfo).Translate(bottomLeftCorner);
		}

		/**
			The same as Shape with all parameters, but with bottomLeft Point set to  RectPoint.Zero.
		*/
		public RectShapeInfo<TCell> Shape(int width, int height, Func<RectPoint, bool> isInside)
		{
			return Shape(width, height, isInside, RectPoint.Zero);
		}

		/**
			Creates the grid in a shape that spans 
			the entire storage rectangle of the given width and height.
		*/
		[ShapeMethod]
		public RectShapeInfo<TCell> Default(int width, int height)
		{
			var rawInfow = MakeShapeStorageInfo<RectPoint>(
				width, 
				height,
				x => RectGrid<TCell>.DefaultContains(x, width, height));

			return new RectShapeInfo<TCell>(rawInfow);
		}

		/**
			Makes a grid with a single cell that corresponds to the origin.
		*/
		[ShapeMethod]
		public RectShapeInfo<TCell> Single()
		{
			var rawInfow = MakeShapeStorageInfo<RectPoint>(
				1, 
				1,
				x => x == RectPoint.Zero);

			return new RectShapeInfo<TCell>(rawInfow);
		}

		/**
			Starts a compound shape operation.

			Any shape that is defined in terms of other shape operations must use this method, and use Endgroup() to end the definition.

				public static RectShapeInfo<TCell> MyCustomShape(this RectOp<TCell> op)
				{
					return 
						BeginGroup()
							.Shape1()
							.Union()
							.Shape2()
						.EndGroup(op);
				}

			@since 1.1
		*/
		public RectOp<TCell> BeginGroup()
		{
			return RectGrid<TCell>.BeginShape();
		}
	}
	/**
		Class for making DiamondGrids in different shapes.
		
		@link_constructing_grids
			
		@copyright Gamelogic.
		@author Herman Tulleken
		@since 1.0
		@see @ref AbstractOp
		@ingroup BuilderInterface
	*/
	public partial class DiamondOp<TCell> : AbstractOp<ShapeStorageInfo<DiamondPoint>>
	{
		public DiamondOp(){}

		public DiamondOp(
			ShapeStorageInfo<DiamondPoint> leftShapeInfo,
			Func<ShapeStorageInfo<DiamondPoint>, ShapeStorageInfo<DiamondPoint>, ShapeStorageInfo<DiamondPoint>> combineShapeInfo) :
			base(leftShapeInfo, combineShapeInfo)
		{}

		/**
			Use this function to create shapes to ensure they fit into memory.
		
			The test function can test shapes anywhere in space. If you specify the bottom corner 
			(in terms of the storage rectangle), the shape is automatically translated in memory 
			to fit, assuming memory width and height is big enough.

			Strategy for implementing new shapes:
				- First, determine the test function.
				- Next, draw a storage rectangle that contains the shape.
				- Determine the storgae rectangle width and height.
				- Finally, determine the grid-space coordinate of the left bottom corner of the storage rectangle.
		
			Then define your function as follows:

			\code{cs}
			public DiamondShapeInfo<TCell> MyShape()
			{
				Shape(stargeRectangleWidth, storageRectangleHeight, isInsideMyShape, storageRectangleBottomleft);
			}
			\endcode

			\param width The widh of the storage rectangle
			\param height The height of the storage rectangle
			\param isInside A function that returns true if a passed point lies inside the shape being defined
			\param bottomLeftCorner The grid-space coordinate of the bottom left corner of the storage rect.

		*/
		public DiamondShapeInfo<TCell> Shape(int width, int height, Func<DiamondPoint, bool> isInside, DiamondPoint bottomLeftCorner)
		{
			var shapeInfo = MakeShapeStorageInfo<DiamondPoint>(width, height, x=>isInside(x + bottomLeftCorner));
			return new DiamondShapeInfo<TCell>(shapeInfo).Translate(bottomLeftCorner);
		}

		/**
			The same as Shape with all parameters, but with bottomLeft Point set to  DiamondPoint.Zero.
		*/
		public DiamondShapeInfo<TCell> Shape(int width, int height, Func<DiamondPoint, bool> isInside)
		{
			return Shape(width, height, isInside, DiamondPoint.Zero);
		}

		/**
			Creates the grid in a shape that spans 
			the entire storage rectangle of the given width and height.
		*/
		[ShapeMethod]
		public DiamondShapeInfo<TCell> Default(int width, int height)
		{
			var rawInfow = MakeShapeStorageInfo<DiamondPoint>(
				width, 
				height,
				x => DiamondGrid<TCell>.DefaultContains(x, width, height));

			return new DiamondShapeInfo<TCell>(rawInfow);
		}

		/**
			Makes a grid with a single cell that corresponds to the origin.
		*/
		[ShapeMethod]
		public DiamondShapeInfo<TCell> Single()
		{
			var rawInfow = MakeShapeStorageInfo<DiamondPoint>(
				1, 
				1,
				x => x == DiamondPoint.Zero);

			return new DiamondShapeInfo<TCell>(rawInfow);
		}

		/**
			Starts a compound shape operation.

			Any shape that is defined in terms of other shape operations must use this method, and use Endgroup() to end the definition.

				public static DiamondShapeInfo<TCell> MyCustomShape(this DiamondOp<TCell> op)
				{
					return 
						BeginGroup()
							.Shape1()
							.Union()
							.Shape2()
						.EndGroup(op);
				}

			@since 1.1
		*/
		public DiamondOp<TCell> BeginGroup()
		{
			return DiamondGrid<TCell>.BeginShape();
		}
	}
	/**
		Class for making PointyHexGrids in different shapes.
		
		@link_constructing_grids
			
		@copyright Gamelogic.
		@author Herman Tulleken
		@since 1.0
		@see @ref AbstractOp
		@ingroup BuilderInterface
	*/
	public partial class PointyHexOp<TCell> : AbstractOp<ShapeStorageInfo<PointyHexPoint>>
	{
		public PointyHexOp(){}

		public PointyHexOp(
			ShapeStorageInfo<PointyHexPoint> leftShapeInfo,
			Func<ShapeStorageInfo<PointyHexPoint>, ShapeStorageInfo<PointyHexPoint>, ShapeStorageInfo<PointyHexPoint>> combineShapeInfo) :
			base(leftShapeInfo, combineShapeInfo)
		{}

		/**
			Use this function to create shapes to ensure they fit into memory.
		
			The test function can test shapes anywhere in space. If you specify the bottom corner 
			(in terms of the storage rectangle), the shape is automatically translated in memory 
			to fit, assuming memory width and height is big enough.

			Strategy for implementing new shapes:
				- First, determine the test function.
				- Next, draw a storage rectangle that contains the shape.
				- Determine the storgae rectangle width and height.
				- Finally, determine the grid-space coordinate of the left bottom corner of the storage rectangle.
		
			Then define your function as follows:

			\code{cs}
			public PointyHexShapeInfo<TCell> MyShape()
			{
				Shape(stargeRectangleWidth, storageRectangleHeight, isInsideMyShape, storageRectangleBottomleft);
			}
			\endcode

			\param width The widh of the storage rectangle
			\param height The height of the storage rectangle
			\param isInside A function that returns true if a passed point lies inside the shape being defined
			\param bottomLeftCorner The grid-space coordinate of the bottom left corner of the storage rect.

		*/
		public PointyHexShapeInfo<TCell> Shape(int width, int height, Func<PointyHexPoint, bool> isInside, PointyHexPoint bottomLeftCorner)
		{
			var shapeInfo = MakeShapeStorageInfo<PointyHexPoint>(width, height, x=>isInside(x + bottomLeftCorner));
			return new PointyHexShapeInfo<TCell>(shapeInfo).Translate(bottomLeftCorner);
		}

		/**
			The same as Shape with all parameters, but with bottomLeft Point set to  PointyHexPoint.Zero.
		*/
		public PointyHexShapeInfo<TCell> Shape(int width, int height, Func<PointyHexPoint, bool> isInside)
		{
			return Shape(width, height, isInside, PointyHexPoint.Zero);
		}

		/**
			Creates the grid in a shape that spans 
			the entire storage rectangle of the given width and height.
		*/
		[ShapeMethod]
		public PointyHexShapeInfo<TCell> Default(int width, int height)
		{
			var rawInfow = MakeShapeStorageInfo<PointyHexPoint>(
				width, 
				height,
				x => PointyHexGrid<TCell>.DefaultContains(x, width, height));

			return new PointyHexShapeInfo<TCell>(rawInfow);
		}

		/**
			Makes a grid with a single cell that corresponds to the origin.
		*/
		[ShapeMethod]
		public PointyHexShapeInfo<TCell> Single()
		{
			var rawInfow = MakeShapeStorageInfo<PointyHexPoint>(
				1, 
				1,
				x => x == PointyHexPoint.Zero);

			return new PointyHexShapeInfo<TCell>(rawInfow);
		}

		/**
			Starts a compound shape operation.

			Any shape that is defined in terms of other shape operations must use this method, and use Endgroup() to end the definition.

				public static PointyHexShapeInfo<TCell> MyCustomShape(this PointyHexOp<TCell> op)
				{
					return 
						BeginGroup()
							.Shape1()
							.Union()
							.Shape2()
						.EndGroup(op);
				}

			@since 1.1
		*/
		public PointyHexOp<TCell> BeginGroup()
		{
			return PointyHexGrid<TCell>.BeginShape();
		}
	}
	/**
		Class for making FlatHexGrids in different shapes.
		
		@link_constructing_grids
			
		@copyright Gamelogic.
		@author Herman Tulleken
		@since 1.0
		@see @ref AbstractOp
		@ingroup BuilderInterface
	*/
	public partial class FlatHexOp<TCell> : AbstractOp<ShapeStorageInfo<FlatHexPoint>>
	{
		public FlatHexOp(){}

		public FlatHexOp(
			ShapeStorageInfo<FlatHexPoint> leftShapeInfo,
			Func<ShapeStorageInfo<FlatHexPoint>, ShapeStorageInfo<FlatHexPoint>, ShapeStorageInfo<FlatHexPoint>> combineShapeInfo) :
			base(leftShapeInfo, combineShapeInfo)
		{}

		/**
			Use this function to create shapes to ensure they fit into memory.
		
			The test function can test shapes anywhere in space. If you specify the bottom corner 
			(in terms of the storage rectangle), the shape is automatically translated in memory 
			to fit, assuming memory width and height is big enough.

			Strategy for implementing new shapes:
				- First, determine the test function.
				- Next, draw a storage rectangle that contains the shape.
				- Determine the storgae rectangle width and height.
				- Finally, determine the grid-space coordinate of the left bottom corner of the storage rectangle.
		
			Then define your function as follows:

			\code{cs}
			public FlatHexShapeInfo<TCell> MyShape()
			{
				Shape(stargeRectangleWidth, storageRectangleHeight, isInsideMyShape, storageRectangleBottomleft);
			}
			\endcode

			\param width The widh of the storage rectangle
			\param height The height of the storage rectangle
			\param isInside A function that returns true if a passed point lies inside the shape being defined
			\param bottomLeftCorner The grid-space coordinate of the bottom left corner of the storage rect.

		*/
		public FlatHexShapeInfo<TCell> Shape(int width, int height, Func<FlatHexPoint, bool> isInside, FlatHexPoint bottomLeftCorner)
		{
			var shapeInfo = MakeShapeStorageInfo<FlatHexPoint>(width, height, x=>isInside(x + bottomLeftCorner));
			return new FlatHexShapeInfo<TCell>(shapeInfo).Translate(bottomLeftCorner);
		}

		/**
			The same as Shape with all parameters, but with bottomLeft Point set to  FlatHexPoint.Zero.
		*/
		public FlatHexShapeInfo<TCell> Shape(int width, int height, Func<FlatHexPoint, bool> isInside)
		{
			return Shape(width, height, isInside, FlatHexPoint.Zero);
		}

		/**
			Creates the grid in a shape that spans 
			the entire storage rectangle of the given width and height.
		*/
		[ShapeMethod]
		public FlatHexShapeInfo<TCell> Default(int width, int height)
		{
			var rawInfow = MakeShapeStorageInfo<FlatHexPoint>(
				width, 
				height,
				x => FlatHexGrid<TCell>.DefaultContains(x, width, height));

			return new FlatHexShapeInfo<TCell>(rawInfow);
		}

		/**
			Makes a grid with a single cell that corresponds to the origin.
		*/
		[ShapeMethod]
		public FlatHexShapeInfo<TCell> Single()
		{
			var rawInfow = MakeShapeStorageInfo<FlatHexPoint>(
				1, 
				1,
				x => x == FlatHexPoint.Zero);

			return new FlatHexShapeInfo<TCell>(rawInfow);
		}

		/**
			Starts a compound shape operation.

			Any shape that is defined in terms of other shape operations must use this method, and use Endgroup() to end the definition.

				public static FlatHexShapeInfo<TCell> MyCustomShape(this FlatHexOp<TCell> op)
				{
					return 
						BeginGroup()
							.Shape1()
							.Union()
							.Shape2()
						.EndGroup(op);
				}

			@since 1.1
		*/
		public FlatHexOp<TCell> BeginGroup()
		{
			return FlatHexGrid<TCell>.BeginShape();
		}
	}
	/**
		Class for making PointyTriGrids in different shapes.
		
		@link_constructing_grids
			
		@copyright Gamelogic.
		@author Herman Tulleken
		@since 1.0
		@see @ref AbstractOp
		@ingroup BuilderInterface
	*/
	public partial class PointyTriOp<TCell> : AbstractOp<ShapeStorageInfo<PointyTriPoint>>
	{
		public PointyTriOp(){}

		public PointyTriOp(
			ShapeStorageInfo<PointyTriPoint> leftShapeInfo,
			Func<ShapeStorageInfo<PointyTriPoint>, ShapeStorageInfo<PointyTriPoint>, ShapeStorageInfo<PointyTriPoint>> combineShapeInfo) :
			base(leftShapeInfo, combineShapeInfo)
		{}

		/**
			Use this function to create shapes to ensure they fit into memory.
		
			The test function can test shapes anywhere in space. If you specify the bottom corner 
			(in terms of the storage rectangle), the shape is automatically translated in memory 
			to fit, assuming memory width and height is big enough.

			Strategy for implementing new shapes:
				- First, determine the test function.
				- Next, draw a storage rectangle that contains the shape.
				- Determine the storgae rectangle width and height.
				- Finally, determine the grid-space coordinate of the left bottom corner of the storage rectangle.
		
			Then define your function as follows:

			\code{cs}
			public PointyTriShapeInfo<TCell> MyShape()
			{
				Shape(stargeRectangleWidth, storageRectangleHeight, isInsideMyShape, storageRectangleBottomleft);
			}
			\endcode

			\param width The widh of the storage rectangle
			\param height The height of the storage rectangle
			\param isInside A function that returns true if a passed point lies inside the shape being defined
			\param bottomLeftCorner The grid-space coordinate of the bottom left corner of the storage rect.

		*/
		public PointyTriShapeInfo<TCell> Shape(int width, int height, Func<PointyTriPoint, bool> isInside, FlatHexPoint bottomLeftCorner)
		{
			var shapeInfo = MakeShapeStorageInfo<PointyTriPoint>(width, height, x=>isInside(x + bottomLeftCorner));
			return new PointyTriShapeInfo<TCell>(shapeInfo).Translate(bottomLeftCorner);
		}

		/**
			The same as Shape with all parameters, but with bottomLeft Point set to  PointyTriPoint.Zero.
		*/
		public PointyTriShapeInfo<TCell> Shape(int width, int height, Func<PointyTriPoint, bool> isInside)
		{
			return Shape(width, height, isInside, FlatHexPoint.Zero);
		}

		/**
			Creates the grid in a shape that spans 
			the entire storage rectangle of the given width and height.
		*/
		[ShapeMethod]
		public PointyTriShapeInfo<TCell> Default(int width, int height)
		{
			var rawInfow = MakeShapeStorageInfo<PointyTriPoint>(
				width, 
				height,
				x => PointyTriGrid<TCell>.DefaultContains(x, width, height));

			return new PointyTriShapeInfo<TCell>(rawInfow);
		}

		/**
			Makes a grid with a single cell that corresponds to the origin.
		*/
		[ShapeMethod]
		public PointyTriShapeInfo<TCell> Single()
		{
			var rawInfow = MakeShapeStorageInfo<PointyTriPoint>(
				1, 
				1,
				x => x == PointyTriPoint.Zero);

			return new PointyTriShapeInfo<TCell>(rawInfow);
		}

		/**
			Starts a compound shape operation.

			Any shape that is defined in terms of other shape operations must use this method, and use Endgroup() to end the definition.

				public static PointyTriShapeInfo<TCell> MyCustomShape(this PointyTriOp<TCell> op)
				{
					return 
						BeginGroup()
							.Shape1()
							.Union()
							.Shape2()
						.EndGroup(op);
				}

			@since 1.1
		*/
		public PointyTriOp<TCell> BeginGroup()
		{
			return PointyTriGrid<TCell>.BeginShape();
		}
	}
	/**
		Class for making FlatTriGrids in different shapes.
		
		@link_constructing_grids
			
		@copyright Gamelogic.
		@author Herman Tulleken
		@since 1.0
		@see @ref AbstractOp
		@ingroup BuilderInterface
	*/
	public partial class FlatTriOp<TCell> : AbstractOp<ShapeStorageInfo<FlatTriPoint>>
	{
		public FlatTriOp(){}

		public FlatTriOp(
			ShapeStorageInfo<FlatTriPoint> leftShapeInfo,
			Func<ShapeStorageInfo<FlatTriPoint>, ShapeStorageInfo<FlatTriPoint>, ShapeStorageInfo<FlatTriPoint>> combineShapeInfo) :
			base(leftShapeInfo, combineShapeInfo)
		{}

		/**
			Use this function to create shapes to ensure they fit into memory.
		
			The test function can test shapes anywhere in space. If you specify the bottom corner 
			(in terms of the storage rectangle), the shape is automatically translated in memory 
			to fit, assuming memory width and height is big enough.

			Strategy for implementing new shapes:
				- First, determine the test function.
				- Next, draw a storage rectangle that contains the shape.
				- Determine the storgae rectangle width and height.
				- Finally, determine the grid-space coordinate of the left bottom corner of the storage rectangle.
		
			Then define your function as follows:

			\code{cs}
			public FlatTriShapeInfo<TCell> MyShape()
			{
				Shape(stargeRectangleWidth, storageRectangleHeight, isInsideMyShape, storageRectangleBottomleft);
			}
			\endcode

			\param width The widh of the storage rectangle
			\param height The height of the storage rectangle
			\param isInside A function that returns true if a passed point lies inside the shape being defined
			\param bottomLeftCorner The grid-space coordinate of the bottom left corner of the storage rect.

		*/
		public FlatTriShapeInfo<TCell> Shape(int width, int height, Func<FlatTriPoint, bool> isInside, PointyHexPoint bottomLeftCorner)
		{
			var shapeInfo = MakeShapeStorageInfo<FlatTriPoint>(width, height, x=>isInside(x + bottomLeftCorner));
			return new FlatTriShapeInfo<TCell>(shapeInfo).Translate(bottomLeftCorner);
		}

		/**
			The same as Shape with all parameters, but with bottomLeft Point set to  FlatTriPoint.Zero.
		*/
		public FlatTriShapeInfo<TCell> Shape(int width, int height, Func<FlatTriPoint, bool> isInside)
		{
			return Shape(width, height, isInside, PointyHexPoint.Zero);
		}

		/**
			Creates the grid in a shape that spans 
			the entire storage rectangle of the given width and height.
		*/
		[ShapeMethod]
		public FlatTriShapeInfo<TCell> Default(int width, int height)
		{
			var rawInfow = MakeShapeStorageInfo<FlatTriPoint>(
				width, 
				height,
				x => FlatTriGrid<TCell>.DefaultContains(x, width, height));

			return new FlatTriShapeInfo<TCell>(rawInfow);
		}

		/**
			Makes a grid with a single cell that corresponds to the origin.
		*/
		[ShapeMethod]
		public FlatTriShapeInfo<TCell> Single()
		{
			var rawInfow = MakeShapeStorageInfo<FlatTriPoint>(
				1, 
				1,
				x => x == FlatTriPoint.Zero);

			return new FlatTriShapeInfo<TCell>(rawInfow);
		}

		/**
			Starts a compound shape operation.

			Any shape that is defined in terms of other shape operations must use this method, and use Endgroup() to end the definition.

				public static FlatTriShapeInfo<TCell> MyCustomShape(this FlatTriOp<TCell> op)
				{
					return 
						BeginGroup()
							.Shape1()
							.Union()
							.Shape2()
						.EndGroup(op);
				}

			@since 1.1
		*/
		public FlatTriOp<TCell> BeginGroup()
		{
			return FlatTriGrid<TCell>.BeginShape();
		}
	}
	/**
		Class for making PointyRhombGrids in different shapes.
		
		@link_constructing_grids
			
		@copyright Gamelogic.
		@author Herman Tulleken
		@since 1.0
		@see @ref AbstractOp
		@ingroup BuilderInterface
	*/
	public partial class PointyRhombOp<TCell> : AbstractOp<ShapeStorageInfo<PointyRhombPoint>>
	{
		public PointyRhombOp(){}

		public PointyRhombOp(
			ShapeStorageInfo<PointyRhombPoint> leftShapeInfo,
			Func<ShapeStorageInfo<PointyRhombPoint>, ShapeStorageInfo<PointyRhombPoint>, ShapeStorageInfo<PointyRhombPoint>> combineShapeInfo) :
			base(leftShapeInfo, combineShapeInfo)
		{}

		/**
			Use this function to create shapes to ensure they fit into memory.
		
			The test function can test shapes anywhere in space. If you specify the bottom corner 
			(in terms of the storage rectangle), the shape is automatically translated in memory 
			to fit, assuming memory width and height is big enough.

			Strategy for implementing new shapes:
				- First, determine the test function.
				- Next, draw a storage rectangle that contains the shape.
				- Determine the storgae rectangle width and height.
				- Finally, determine the grid-space coordinate of the left bottom corner of the storage rectangle.
		
			Then define your function as follows:

			\code{cs}
			public PointyRhombShapeInfo<TCell> MyShape()
			{
				Shape(stargeRectangleWidth, storageRectangleHeight, isInsideMyShape, storageRectangleBottomleft);
			}
			\endcode

			\param width The widh of the storage rectangle
			\param height The height of the storage rectangle
			\param isInside A function that returns true if a passed point lies inside the shape being defined
			\param bottomLeftCorner The grid-space coordinate of the bottom left corner of the storage rect.

		*/
		public PointyRhombShapeInfo<TCell> Shape(int width, int height, Func<PointyRhombPoint, bool> isInside, PointyHexPoint bottomLeftCorner)
		{
			var shapeInfo = MakeShapeStorageInfo<PointyRhombPoint>(width, height, x=>isInside(x + bottomLeftCorner));
			return new PointyRhombShapeInfo<TCell>(shapeInfo).Translate(bottomLeftCorner);
		}

		/**
			The same as Shape with all parameters, but with bottomLeft Point set to  PointyRhombPoint.Zero.
		*/
		public PointyRhombShapeInfo<TCell> Shape(int width, int height, Func<PointyRhombPoint, bool> isInside)
		{
			return Shape(width, height, isInside, PointyHexPoint.Zero);
		}

		/**
			Creates the grid in a shape that spans 
			the entire storage rectangle of the given width and height.
		*/
		[ShapeMethod]
		public PointyRhombShapeInfo<TCell> Default(int width, int height)
		{
			var rawInfow = MakeShapeStorageInfo<PointyRhombPoint>(
				width, 
				height,
				x => PointyRhombGrid<TCell>.DefaultContains(x, width, height));

			return new PointyRhombShapeInfo<TCell>(rawInfow);
		}

		/**
			Makes a grid with a single cell that corresponds to the origin.
		*/
		[ShapeMethod]
		public PointyRhombShapeInfo<TCell> Single()
		{
			var rawInfow = MakeShapeStorageInfo<PointyRhombPoint>(
				1, 
				1,
				x => x == PointyRhombPoint.Zero);

			return new PointyRhombShapeInfo<TCell>(rawInfow);
		}

		/**
			Starts a compound shape operation.

			Any shape that is defined in terms of other shape operations must use this method, and use Endgroup() to end the definition.

				public static PointyRhombShapeInfo<TCell> MyCustomShape(this PointyRhombOp<TCell> op)
				{
					return 
						BeginGroup()
							.Shape1()
							.Union()
							.Shape2()
						.EndGroup(op);
				}

			@since 1.1
		*/
		public PointyRhombOp<TCell> BeginGroup()
		{
			return PointyRhombGrid<TCell>.BeginShape();
		}
	}
	/**
		Class for making FlatRhombGrids in different shapes.
		
		@link_constructing_grids
			
		@copyright Gamelogic.
		@author Herman Tulleken
		@since 1.0
		@see @ref AbstractOp
		@ingroup BuilderInterface
	*/
	public partial class FlatRhombOp<TCell> : AbstractOp<ShapeStorageInfo<FlatRhombPoint>>
	{
		public FlatRhombOp(){}

		public FlatRhombOp(
			ShapeStorageInfo<FlatRhombPoint> leftShapeInfo,
			Func<ShapeStorageInfo<FlatRhombPoint>, ShapeStorageInfo<FlatRhombPoint>, ShapeStorageInfo<FlatRhombPoint>> combineShapeInfo) :
			base(leftShapeInfo, combineShapeInfo)
		{}

		/**
			Use this function to create shapes to ensure they fit into memory.
		
			The test function can test shapes anywhere in space. If you specify the bottom corner 
			(in terms of the storage rectangle), the shape is automatically translated in memory 
			to fit, assuming memory width and height is big enough.

			Strategy for implementing new shapes:
				- First, determine the test function.
				- Next, draw a storage rectangle that contains the shape.
				- Determine the storgae rectangle width and height.
				- Finally, determine the grid-space coordinate of the left bottom corner of the storage rectangle.
		
			Then define your function as follows:

			\code{cs}
			public FlatRhombShapeInfo<TCell> MyShape()
			{
				Shape(stargeRectangleWidth, storageRectangleHeight, isInsideMyShape, storageRectangleBottomleft);
			}
			\endcode

			\param width The widh of the storage rectangle
			\param height The height of the storage rectangle
			\param isInside A function that returns true if a passed point lies inside the shape being defined
			\param bottomLeftCorner The grid-space coordinate of the bottom left corner of the storage rect.

		*/
		public FlatRhombShapeInfo<TCell> Shape(int width, int height, Func<FlatRhombPoint, bool> isInside, FlatHexPoint bottomLeftCorner)
		{
			var shapeInfo = MakeShapeStorageInfo<FlatRhombPoint>(width, height, x=>isInside(x + bottomLeftCorner));
			return new FlatRhombShapeInfo<TCell>(shapeInfo).Translate(bottomLeftCorner);
		}

		/**
			The same as Shape with all parameters, but with bottomLeft Point set to  FlatRhombPoint.Zero.
		*/
		public FlatRhombShapeInfo<TCell> Shape(int width, int height, Func<FlatRhombPoint, bool> isInside)
		{
			return Shape(width, height, isInside, FlatHexPoint.Zero);
		}

		/**
			Creates the grid in a shape that spans 
			the entire storage rectangle of the given width and height.
		*/
		[ShapeMethod]
		public FlatRhombShapeInfo<TCell> Default(int width, int height)
		{
			var rawInfow = MakeShapeStorageInfo<FlatRhombPoint>(
				width, 
				height,
				x => FlatRhombGrid<TCell>.DefaultContains(x, width, height));

			return new FlatRhombShapeInfo<TCell>(rawInfow);
		}

		/**
			Makes a grid with a single cell that corresponds to the origin.
		*/
		[ShapeMethod]
		public FlatRhombShapeInfo<TCell> Single()
		{
			var rawInfow = MakeShapeStorageInfo<FlatRhombPoint>(
				1, 
				1,
				x => x == FlatRhombPoint.Zero);

			return new FlatRhombShapeInfo<TCell>(rawInfow);
		}

		/**
			Starts a compound shape operation.

			Any shape that is defined in terms of other shape operations must use this method, and use Endgroup() to end the definition.

				public static FlatRhombShapeInfo<TCell> MyCustomShape(this FlatRhombOp<TCell> op)
				{
					return 
						BeginGroup()
							.Shape1()
							.Union()
							.Shape2()
						.EndGroup(op);
				}

			@since 1.1
		*/
		public FlatRhombOp<TCell> BeginGroup()
		{
			return FlatRhombGrid<TCell>.BeginShape();
		}
	}
	/**
		Class for making CairoGrids in different shapes.
		
		@link_constructing_grids
			
		@copyright Gamelogic.
		@author Herman Tulleken
		@since 1.0
		@see @ref AbstractOp
		@ingroup BuilderInterface
	*/
	public partial class CairoOp<TCell> : AbstractOp<ShapeStorageInfo<CairoPoint>>
	{
		public CairoOp(){}

		public CairoOp(
			ShapeStorageInfo<CairoPoint> leftShapeInfo,
			Func<ShapeStorageInfo<CairoPoint>, ShapeStorageInfo<CairoPoint>, ShapeStorageInfo<CairoPoint>> combineShapeInfo) :
			base(leftShapeInfo, combineShapeInfo)
		{}

		/**
			Use this function to create shapes to ensure they fit into memory.
		
			The test function can test shapes anywhere in space. If you specify the bottom corner 
			(in terms of the storage rectangle), the shape is automatically translated in memory 
			to fit, assuming memory width and height is big enough.

			Strategy for implementing new shapes:
				- First, determine the test function.
				- Next, draw a storage rectangle that contains the shape.
				- Determine the storgae rectangle width and height.
				- Finally, determine the grid-space coordinate of the left bottom corner of the storage rectangle.
		
			Then define your function as follows:

			\code{cs}
			public CairoShapeInfo<TCell> MyShape()
			{
				Shape(stargeRectangleWidth, storageRectangleHeight, isInsideMyShape, storageRectangleBottomleft);
			}
			\endcode

			\param width The widh of the storage rectangle
			\param height The height of the storage rectangle
			\param isInside A function that returns true if a passed point lies inside the shape being defined
			\param bottomLeftCorner The grid-space coordinate of the bottom left corner of the storage rect.

		*/
		public CairoShapeInfo<TCell> Shape(int width, int height, Func<CairoPoint, bool> isInside, PointyHexPoint bottomLeftCorner)
		{
			var shapeInfo = MakeShapeStorageInfo<CairoPoint>(width, height, x=>isInside(x + bottomLeftCorner));
			return new CairoShapeInfo<TCell>(shapeInfo).Translate(bottomLeftCorner);
		}

		/**
			The same as Shape with all parameters, but with bottomLeft Point set to  CairoPoint.Zero.
		*/
		public CairoShapeInfo<TCell> Shape(int width, int height, Func<CairoPoint, bool> isInside)
		{
			return Shape(width, height, isInside, PointyHexPoint.Zero);
		}

		/**
			Creates the grid in a shape that spans 
			the entire storage rectangle of the given width and height.
		*/
		[ShapeMethod]
		public CairoShapeInfo<TCell> Default(int width, int height)
		{
			var rawInfow = MakeShapeStorageInfo<CairoPoint>(
				width, 
				height,
				x => CairoGrid<TCell>.DefaultContains(x, width, height));

			return new CairoShapeInfo<TCell>(rawInfow);
		}

		/**
			Makes a grid with a single cell that corresponds to the origin.
		*/
		[ShapeMethod]
		public CairoShapeInfo<TCell> Single()
		{
			var rawInfow = MakeShapeStorageInfo<CairoPoint>(
				1, 
				1,
				x => x == CairoPoint.Zero);

			return new CairoShapeInfo<TCell>(rawInfow);
		}

		/**
			Starts a compound shape operation.

			Any shape that is defined in terms of other shape operations must use this method, and use Endgroup() to end the definition.

				public static CairoShapeInfo<TCell> MyCustomShape(this CairoOp<TCell> op)
				{
					return 
						BeginGroup()
							.Shape1()
							.Union()
							.Shape2()
						.EndGroup(op);
				}

			@since 1.1
		*/
		public CairoOp<TCell> BeginGroup()
		{
			return CairoGrid<TCell>.BeginShape();
		}
	}
}

