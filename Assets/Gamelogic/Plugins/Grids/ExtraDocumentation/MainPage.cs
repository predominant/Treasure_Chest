//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013-2015 Gamelogic (Pty) Ltd  //
//----------------------------------------------//

/**
	@mainpage Grids API Documentation
	
Grids is a library for Unity Game Engine that allows you to setup 
	a variety of grids rapidly. It supports a variety of grid types, 
	including hexagonal and triangular grids. It’s easy to set 
	your grid up in some standard shapes, to combine shapes, or to define your own. It 
	comes with functionality to easily map grid coordinates to game space: center your 
	grid in a rectangle or align it to an edge. It's easy to flip coordinates, or 
	rotate them so that they conform to your intuition. We designed the library with an
	eye on 2D games, but the grids work just as well in 3D. 

	This is the API documentation. For tutorials and other documentation, 
	see: http://gamelogic.co.za/documentation-contents/ .

	For a visual index of grids, see the <a href="http://gamelogic.co.za/grids/quick-start-tutorial/grid-index/">Grid Index</a>.

	@version 1.14

	<h3>Navigating the documentation</h3>
	Grids is a big library, and it can be hard to find functions. It's a good idea to 
    read this guide first: [Introduction and Terminology](http://gamelogic.co.za/grids/documentation-contents/quick-start-tutorial/introduction-and-terminology/) 
	which explains the core concepts. It's important to understand grids, maps, and 
	grid points.

	When browsing the documentation, a good place to start is to look at the methods 
	provided by your particular grid and grid point class (for example RectGrid and 
	RectPoint).
	
	Algorithms contain many of the grid algorithms such as path finding, getting 
	grid points in range, fidinging connected lines, rays and other shapes, and 
	transforming sets of points.
	
	Geometric funtions are usually provided by the point class, unless they rely and
	the particular mapping, in which case they are provided by the map.
	
	<center>
		<img src="GridIndex.png" />
	</center>

	<h3>Changes</h3>
 
	Version 1.14
		- Added classes to support building mesh grids in the editor.
		- Added a Filter method for grid shapes that makes it easier 
		to construct arbitrary shapes.

	Version 1.13
		- Removed GridUtils that contains methods for testing purposes.
		- Added custom maps for all tile grid builders that did not have them.
		- Added functions that were missing for the Basic license.

	Version 1.12
		- Fixed a bug with the color list drawer that prevented grid colors from being changed.

	Version 1.11
		- Fixed a bug in some overloads of PointsInRange and PointsInRangeCost that caused poor performance.
		- Removed a old AStar2 test method.
		- Added another constructor for LineGrids that allows neighbor setup.

	Version 1.10.1
		- Added a constructor to InspectableVectorPoint that takes a diamond point as a parameter.
		- Fixed a bug with neighbor constants for Diamond grids (which caused Sprial iterators from not working).
		- Fixed a bug with neighbors of wrapped grids.
		- Fixed a bug with PointyHexHorizontalRectangleWrapper wrapping calculations.
		
	Version 1.10		
		- Added GetPointsInRange and GetPointsInRangeCosts functions that returns all the points in a grid in a given range, or their distances from the start goal.
		- Added three new cell types that can be used with Unity's GUI system (with or without grid builders):
			- UITextCell 
			- UIImageCell			
			- UIImageTextCell
		- Added a GetLine extension methods for maps.
		- Added a spriral iterator overload for rect and hex grids.
		- Added a spiral iterator for diamond grids
		- Added the Perp method to rect, diamond, and hex points.		
		- Added a class PointStack which is a AOL compiler-safe Stack for points.
		- Added Fill methods to set the entire grid to a value or the result of a function.
		- Added CloneStructure methods that clones a grids structure, and use the given value or function to 
			fill the new grid.
		- Added an Apply method that calls an action for each cell in the grid.
		- Added a Transform method that replaces each cell with a transformed cell.
		- Added a overload to the Scale methods in Imap and AbstractMap that takes a Vector2.
		- Redesigned the way neighbors work slightly:
			- Added a property NeighborDirections to grids that can be used to get and set the grids neighbor-setup. 
			- Added default neighbor sets to point types
			- Removed InitNeighbors from abstract grid classes
			- CloneStructure and CastValues now correctly copy over the original grid's neighbor-setup
		- Changed all grid builders to take a Vector2 for the cell spacing factor, instead of a single factor.
		- Fixed dimension and anchor calculations for polar pointy brick maps
		- Fixed the type of the Grid property of polar grid builders.
		- Fixed a bug that prevented MeshTileCells' colors to be serialized.
		- Fixed a bug that could cause errors with input if the cell in a grid is null.
		- Fixed a bug that prevented polar grids from being used with GridBehaviours
		- Fixed a bug with polar grid maps and mesh creation.
		- Fixed a bug in the Poisson disk generation code that sometimes caused points to be generated outside the given rectangle
		- Removed the default OnClick method provided in SpriteCell and MeshTileCell
		- Made TextureCell obsolete (use UVCell instead)
		- Renamed the Select method defined on grids to SelectValuesAt
		- Renamed XXXUtil to XXXUtils for consistency
		- Made many documentation improvements.
		- Made compatible with Unity 5.0
		- Updated extensions package to version 1.1

	Version 1.9
		- Added custom shapes for all basic, non-polar grids in the editor.
		- Slightly changed the design of LayeredGrid and LayeredPoint to make them more re-usable and robust.
		- Fixed the position of gizmo labels for polar grids.
		- Fixed a bug that caused null pointer exceptions in interactive grids with null cells.
		- Fixed a bug with cell dimensions in hex grid builders that caused bugs when used with non-regular hexagons.
		- Fixed a bug that caused editor code to be called in builds.
		- Fixed a bug with Pointy hex shapes in the editor(fat and thin rectangles were swapped)
		- Removed the ToolTip and Description attributes (which implemented built-in functionality).
		- Made many documentation improvements.
	
	Version 1.8
		- Added grid builders that allows you to build grids in the editors, including the classes for supporting them, including
			- grid builder classes and their editors
			- classes for implementing custom maps and shapes that can be used with builders
			- classes that allows implementation of grid behaviours
			- cells and their editors
		- Added LineGrid and related classes for making 1D grids. 
		- Added ArchimedeanSpiralMap for use with a LineGrid to map cells in a spiral.
		- Added LayeredGrid and related classes for making 3D grids made from layers of 2D grids.
		- Added GenericSplicedGrids and related classes for making arbitrary semi-regular grids.
		- Added PointListMap that maps a sequence of LinePoints to an arbitrary set of world points.
		- Added VoronoiMap for mapping 2D space to an arbitrary point set.
		- Added IGridToWorldMap, an interface for one-way maps that can be used by maps that does the inverse mapping automatically.
		- Added method GetNeighborHood for RectGrid that returns a square region of points around a given point.
		- Added method Stretch to map building to make it easy to get a grid stretched over a rectangle.
		- Added class PoissonDisk for generating a PoissonDisk sample of points.
		- Added constructors to InspectableVectorPoint and InspectableSplicedVectorPoint that takes other points
			as parameters
		- Added ToString methods to InspectableVectorPoint and InspectableSplicedVectorPoint.
		- Made GetLongestConnected Obsolete (use GetLongestConnectedLine instead).
		- Removed ToString and GetHashCode from IGridPoint interface (these are redundant).
		- Made AnimatedMap also work with EditorApplication.timeSinceStartup.
		- Fixed a bug with the edge grids of tri grids.
		- Renamed "SuperRectGrid" to "NestedRectGrid".
		- Made enhancements to make Grids work better on iOS, including
			- rewriting certain methods to avoid problems with deep generics
			- adding more compiler hints for built-in cells	

	Version 1.7
		- Added Mod, Div and Mul methods for all uniform grid points.
		- Added Dot and PerpDot methods for all uniform grid points.
		- Added consistent coloring functions for all built-in grid points.
		- Added spiral iterator for hex grids.
		- Added some LINQ-like methods for grids, as well as GetValue and SetValue methods to make 
			LINQ constructions easier to write.
		- Removed Equals from the declaration of IGridPoint. It is unnecessary, but more importantly
			caused unwanted boxing of points.
		- Added wrapped versions of Rectangular, Diamond, Hexagonal and Triangular Grids. Use the static WrappedX 
			functions to use them.
		- Added a polar maps for Rectangular and Hex Grids that can be used with wrapped RectGrids and HexGrids
			to make polar grids.
		- Added some shapes for the Cairo grid.
		- Added a version of AStar that lets you specify the cost, not just the heuristic cost.
		- Fixed a bug with vertex and edge grids that plagued rectangular and diamond grids.
		- Fixed a bug with the canonical positions for hex-point shapes.
		- Simplified the underlying storage mechanism; this makes it easier to construct grids from scratch.
			It also had the result of default shapes changing from ragged rectangles to parallelograms (rect grids' 
			default shape remains a rectangle).
		- Changed some of the shape functions to make it more intuitive "where" a shape will be in relation to the 
			origin.
		- Made static helper functions in the Op classes that were accidentally exposed private. The public geometric functions provided
			by points should be used instead.
		- Improved the AStar algorithm's run speed and its garbage generation.
		- Removed VectorPoint as implementation base for uniform points, thereby making them slightly faster.
		- Made many improvements to the documentation.		

	Version 1.6
		- Fixed a bug with DiamondGrid's neighbors
		- Added NestedRectGrid
		- Added more point arithmetic for RectGrid
		- Added a grid shuffle method
		- Added an implementation of IList for IGridPoints that is safe to use with the AOT compiler. 
		- Replaced all internal collections with versions that are safe to use with the AOT compiler for iOS.
		- Included compiler hints that can be called to help the AOT compiler include all the necessary code.		

	Version 1.5
		- Fixed a bug in hex maps that caused hexagon grids with non-regular cells not work correctly.
		- Fixed spelling of some enums that contains the word North. This may break your code :/ but we could not
			deprecate the old enums safely.
		- Moved {edge and vertex} code in {rect and diamond} {grid and point} classes into separate files.

	Version 1.4
		- Reorganized the library slightly to make it possible to export different versions
		of it.

	Version 1.3
		- Added geometry calculations for hex points
		- Fixed wrong spelling of some methods that contains the word Middle. Old methods
		have been made obsolete, so this change should not break your code. 
		- Fixed a bug in one of the 3D to 2D extension methods provided form Transform.
		- Lots of small code quality improvements under the hood (including a few optimizations).

	Version 1.2
		- Added PolygonMap for handling general polygon-grids
		- Added CairoMap, CairoGrid and CairoPoint as an example of an "exotic" grid.
	
	Version 1.1
		- Added grouping for shape building
		- Added symmetric difference for shape building
		- Added more parallelograms for triangular grids
		- Changed the diamond grid / point coordinate system to be more intuitive
		- Fixed a bug with hexagonal shapes for triangular grids
		- Fixed a bug with composite shape building
		- Fixed a bug with vertex and edge grids 
*/

/**
	This namespace contains classes used commonly by sub-namespaces.
*/
namespace Gamelogic
{
	/**
		This namespace contains classes and methods for working with semi-regular grids.
	*/
	namespace Grids
	{
		/**
			Classes used for grids editors.
		*/
	// ReSharper disable once EmptyNamespace
		namespace Editor {}
	}

	/**
		Classes used for writing Unity Editor code that is used by all Gamelogic editor namespaces.
	*/
	// ReSharper disable once EmptyNamespace
	namespace Editor {}

	/**
		Code used for implementation that is not part of the API.
	*/
	// ReSharper disable once EmptyNamespace
	namespace Internal {}
}


/**
	@defgroup Grids
	Concrete Grids. These are the classes you use in your code to build grids. 
		* @link_constructing_grids to see how to construct grids in code.
		* @link_grid_index for a visual list of grids and corresponding classes.

	@defgroup Points
	Concrete Points. These are the structs that represent grid coordinates. Points are immutable. This makes them suitable for use as keys in dictionaries. 
	To display and configure points in the inspector, use InspectableVectorPoint or InspectableSplicedVectorPoint.
	
	@defgroup Maps
	Concrete Maps. These are the classes that you use to convert between grid and world coordinates
		* @link_working_with_maps to see how maps are used to position cells and to process user interaction.
		* @link_grid_index for a visual list of grids and which maps you should use.

	@defgroup Utilities
	Utilities and algorithms that can be used with grids.

	@defgroup Interface
	Interfaces and public classes that will typically occur in client code.

	@defgroup BuilderInterface Builder Interface
	Interfaces and classes used for the shape builder fluent interface. 
	@link_constructing_grids

	@defgroup UnityComponents Unity Components
	Classes that are designed to be used as components on Unity objects.

	@defgroup UnityEditorSupport Unity Editor Support
	Classes designed to support grids in the Unity editor, 

	@defgroup UnityUtilities Unity Utilities
	Utility classes specific to Unity.

	@defgroup Scaffolding
	"Behind-the-scene" classes used to implement grid features. 
	These classes will not likely (and should probably not) be used in client code.
	
	@defgroup Helpers
	"Behind-the-scene" helper classes used to implement grid features.
	These classes will not likely (and should probably not) be used in client code.
*/