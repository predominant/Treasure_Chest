//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) Gamelogic (Pty) Ltd            //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		A TileCell is a cell that is represented by a single Unity object. 

		They can be anything, including Sprites, Models, or compound objects, 
		as long as they can be instantiated, and each corresponds to exactly one 
		cell in a grid. They are used with TileGrid builders.

		@link_making_your_own_cells for guidelines on making your own cell.

		@ingroup UnityComponents
		@version1_8
	*/

	public abstract class TileCell : GLMonoBehaviour, IColorableCell
	{
		[SerializeField]
		private Vector3 centerOffset = Vector3.zero;
		/**
			The visual center of a cell.
			
			This is useful for cells that do not have their actual pivot 
			at the "expected" place, such as mesh tile cells in polar grids.			
		*/
		public Vector3 Center
		{
			get {return transform.TransformPoint(__CenterOffset); }
		}

		/**
			Should be set by the grid creator to a value that can serve as the 
			center of the cell (if the cell is at the origin).

			@usedbyeditor
		*/

		public Vector3 __CenterOffset
		{
			get { return centerOffset; }
			set { centerOffset = value; }
		}

		public abstract Color Color { get; set; }

		/**
			The 2D dimensions of the tile. For 3D objects, this is the 
			dimensions along the same plane as the 2D grid.
		*/
		public abstract Vector2 Dimensions { get; }

		/**
			This methods is called by the editor, and is to update the 
			cell representation to it's current state.

			(Typically, this is necessary for serialized fields that 
			affect the presentation. For example, the color may be saved,
			but the actual object may reset it's color).

			@usedbyeditor
		*/
		public abstract void __UpdatePresentation(bool forceUpdate);

		/**
			Sets the angle of the tile. If the tile is in the XY plane, the angle 
			will be the Z angle.

			@param angle in degrees.
		*/
		public abstract void SetAngle(float angle);

		/**
			Adds the given angle to the current angle of the tile. If the tile is 
			in the XY plane, it will be added to the Z angle.
		*/
		public abstract void AddAngle(float angle);


		#region Gizmos

		/**
			Draws the Gizmos for this tile: a label with the tile name.

			It is often convenient, for this reason, to make the tile name
			the coordinate of the tile.
		*/
		virtual public void OnDrawGizmos()
		{
			GLGizmos.Label(Center, name);
		}

		#endregion

	}
}