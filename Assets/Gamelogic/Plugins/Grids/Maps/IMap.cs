//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System;

using UnityEngine;
namespace Gamelogic.Grids
{
	/**
		An IMap maps 2D world coordinates to Grid coordinates and vice versa. 
	
		Most of the methods of this class are meant to be chained to the constructor. The last command
		in the chain is suaully a conversion to IMap3D, which converts the 2D coordinates to 3D for use 
		in the game engine.

		
	@link_woking_with_maps.

		The order of chained calls sometimes make a difference.

		The standard order is this:
			- set grid point transforms
			- set cell anchoring
			- set world transforms (such as translate, rotate)
			- do layout (using WithWindow)
			- convert to 3D

		Transformations only apply to the world points returned and processed by the map, 
		the grid contents is not transformed.

		For example, applying scale to the map, will not scale the cells <emph>physically</emph>, in the sense 
		that if the grid contains GameObjects, cells will remain the same size. The cells will be logically bigger,
		so they will appear further apart from each other.

		Built-in 2D grids generally have associated built-in maps. See the [Grid Index](http://gamelogic.co.za/grids/quick-start-tutorial/grid-index/) 
		for the list.

		You can also provide your own maps, either as implementations of IMap, or IMap3D.
		
		@version1_0

		@ingroup Interface
	*/
	public interface IMap<TPoint> : IGridToWorldMap<TPoint>
		where TPoint : IGridPoint<TPoint>
	{
		#region Properties
		/**
			Gets a grid point given a world point.
		*/
		TPoint this[Vector2 point]
		{
			get;
		}

		/**
			The transformation applied to points before they are returned from the map.
		*/
		Func<TPoint, TPoint> GridPointTransform
		{
			get;
		}

		/**
			The transformation applied to points before they are processed by the map.
		*/
		Func<TPoint, TPoint> InverseGridPointTransform
		{
			get;
		}
		#endregion

		#region Helper methods for implementations
		/** 
			@name Helper Methods for implementers.
		*/
		/**@{*/

		/**
			This method maps a world point to a grid point
			without making any compensation based on the 
			anchoring.

			This method should be used for calculations in maps, 
			rather than the index methods. (The index methods do 
			the anchor compensation. If you use that method
			instead of this one internally, you may compensate 
			more than one time.
		*/
		TPoint RawWorldToGrid(Vector2 worldPoint);

		/**
			This method maps a grid point to a world point.
		*/
		Vector2 GridToWorld(TPoint worldPoint);

		Vector2 CalcGridDimensions(IGridSpace<TPoint> grid);
		Vector2 CalcBottomLeft(IGridSpace<TPoint> grid); 
		
		Vector2 CalcAnchorDimensions(IGridSpace<TPoint> grid);
		Vector2 CalcAnchorBottomLeft(IGridSpace<TPoint> grid);
		

		Vector2 GetAnchorTranslation();
		Vector2 GetCellDimensions();
		Vector2 GetCellDimensions(TPoint point);
		
		/** @}*/
		#endregion		

		#region Fluent Interface Members
		/** 
			@name Fluent Interface Members
			
			These are the methods you can chain together 
			to get a suitable map.
		*/
		/**@{*/

		/**
			Sets the point transforms used on the grid that this map
			will used with. 

			Normally, the grid and map should have the same point transforms.
		*/
		IMap<TPoint> SetGridPointTransforms(
			Func<TPoint, TPoint> coordinateTransformation,
			Func<TPoint, TPoint> inverseCoordinateTransformation);

		/**
			Returns an IMap that anchors the grid cells to the left. 
		*/
		IMap<TPoint> AnchorCellLeft();

		/**
			Returns an IMap that anchors the grid cells to the right. 
		*/
		IMap<TPoint> AnchorCellRight();

		/**
			Returns an IMap that anchors the grid cells horizontally to the center. 
		*/
		IMap<TPoint> AnchorCellCenter();

		/**
			Returns an IMap that anchors the grid cells to the top.
		*/
		IMap<TPoint> AnchorCellTop();

		/**
			Returns an IMap that anchors the grid cells to the bottom.
		*/
		IMap<TPoint> AnchorCellBottom();

		/**
			Returns an IMap that anchors the grid cells vertically to the middle.
		*/
		IMap<TPoint> AnchorCellMiddle();

		/**
			Returns an IMap that anchors the grid cells vertically to the top left.
		*/
		IMap<TPoint> AnchorCellTopLeft();

		/**
			Returns an IMap that anchors the grid cells vertically to the top center.
		*/
		IMap<TPoint> AnchorCellTopCenter();

		/**
			Returns an IMap that anchors the grid cells vertically to the top right.
		*/
		IMap<TPoint> AnchorCellTopRight();

		/**
			Returns an IMap that anchors the grid cells to the middle left.
		*/
		IMap<TPoint> AnchorCellMiddleLeft();

		/**
			Returns an IMap that anchors the grid cells to the middle center.
		*/
		IMap<TPoint> AnchorCellMiddleCenter();

		/**
			Returns an IMap that anchors the grid cells to the middle right.
		*/
		IMap<TPoint> AnchorCellMiddleRight();

		/**
			Returns an IMap that anchors the grid cells to the bottom left.
		*/
		IMap<TPoint> AnchorCellBottomLeft();

		/**
			Returns an IMap that anchors the grid cells to the bottom center.
		*/
		IMap<TPoint> AnchorCellBottomCenter();

		/**
			Returns an IMap that anchors the grid cells to the bottom right.
		*/
		IMap<TPoint> AnchorCellBottomRight();

		/**
			Returns an IMap where grid cells are translated by the give amount.
		*/
		IMap<TPoint> Translate(Vector2 offset);

		/**
			Returns an IMap where grid cells are translated by the give amounts.
		*/
		IMap<TPoint> Translate(float offsetX, float offsetY);

		/**
			Returns an IMap where grid cells are translated by the give amount.
		*/
		IMap<TPoint> TranslateX(float offsetX);

		/**
			Returns an IMap where grid cells are translated by the give amount.
		*/
		IMap<TPoint> TranslateY(float offsetY);

		/**
			Returns an IMap where grid cells are reflected about the Y-point in world space.
		*/
		IMap<TPoint> ReflectAboutY();

		/**
			Returns an IMap where grid cells are reflected about the X-point in world space.
		*/
		IMap<TPoint> ReflectAboutX();

		/**
			Returns an IMap where grid cells positioned with x-and y-coordinates (in world space) flipped.
		*/
		IMap<TPoint> FlipXY();

		/**
			Returns an IMap where grid cells are rotated about the origin by the given angle.
		*/
		IMap<TPoint> Rotate(float angle);

		/**
			Returns an IMap where grid cells are rotated about the given point by the given angle.
		*/
		IMap<TPoint> RotateAround(float angle, Vector2 point);

		/**
			Returns an IMap where grid cells are rotated about the origin by 90 degrees.
		*/
		IMap<TPoint> Rotate90();

		/**
			Returns an IMap where grid cells are scaled by factor.
		*/
		IMap<TPoint> Scale(float factor);

		IMap<TPoint> Scale(Vector2 factor);

		/**
			Returns an IMap where grid cells are scaled by the factors in each direction.
		*/
		IMap<TPoint> Scale(float factorX, float factorY);

		/**
			Returns an IMap where grid cells are scaled about the origin by 180 degrees.
		*/
		IMap<TPoint> Rotate180();		

		/**
			Returns a WindowedMap based on this map that can be used to 
			lay the grid out in a window.

			For example:
				
			@code
			var map = new RectMap(cellDimensions)
				.WithWindow(screenRect)
				.AlignMiddleCenter();
			@endcode
		*/
		WindowedMap<TPoint> WithWindow(Rect window);

		/**
			Returns a IMap3D which maps a grid point to Vector3 instead of Vector2.
			The vector3 is the same as the Vector2 that this map would return, with the 
			z set to the given value.
		*/
		IMap3D<TPoint> To3DXY(float z);

		/**
			The same as To3DX(float z), but with z set to 0.
		*/
		IMap3D<TPoint> To3DXY();

		/**
			Returns a IMap3D which maps a grid point to Vector3 instead of Vector2.
			The vector3 is the same as the Vector2 that this map would return, 
			with z of the Vector3 corresponding to y of the Vector2, and with the 
			y set to the given value.
		*/
		IMap3D<TPoint> To3DXZ(float y);

		/**
			The same as To3DZ(float y), but with y set to 0.
		*/
		IMap3D<TPoint> To3DXZ();
		/** @}*/
		#endregion
		
	}
}