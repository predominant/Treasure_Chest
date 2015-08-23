//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		A cell represented by a mesh, such as used by the polar grids.

		@link_making_your_own_cells for guidelines on making your own cell.
		
		@version1_8
		@ingroup UnityComponents
	*/
	[AddComponentMenu("Gamelogic/Cells/MeshTileCell")]
	public class MeshTileCell : TileCell
	{
		private bool on;

		[SerializeField]
		private Color color;
		
		[SerializeField]
		private Color highlightColor;

		override public Color Color
		{
			get { return color; }

			set
			{
				color = value;
				highlightColor = Color.Lerp(value, Color.white, 0.5f);

				__UpdatePresentation();
			}
		}

		public override Vector2 Dimensions
		{
			get { return GetComponent<MeshFilter>().sharedMesh.bounds.size.To2DXY(); }
		}

		public override void __UpdatePresentation(bool forceUpdate)
		{
			if (forceUpdate) __UpdatePresentation();
		}

		public override void SetAngle(float angle)
		{
			transform.RotateAroundZ(angle);
		}

		public override void AddAngle(float angle)
		{
			transform.RotateAroundZ(transform.localEulerAngles.z + angle);
		}

		private void __UpdatePresentation()
		{
			var mesh = GetComponent<MeshFilter>().sharedMesh;
			var colors = new Color[mesh.vertexCount];

			for (int i = 0; i < colors.Length; i++)
			{
				colors[i] = HighlightOn ? highlightColor : color;
			}

			mesh.colors = colors;
		}		

		public bool HighlightOn
		{
			get { return on; }

			set
			{
				on = value;

				__UpdatePresentation();
			}
		}
	}
}