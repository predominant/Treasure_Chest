//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2014 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

namespace Gamelogic.Grids
{

	/**
		A tile that is represented by a simple 3D object (such as a cube), 
		typically an object with a single mesh and material.

		@link_making_your_own_cells for guidelines on making your own cell.
		
		@version1_8
		@link_making_your_own_cells for guidelines on making your own cell.
	*/
	public class Block : TileCell
	{
		[SerializeField]
		[Tooltip("The color of the block.")]
		private Color color;

		[SerializeField]
		private Material materialCopy; 

		public override Color Color
		{
			get { return color; }

			set
			{
				color = value;
				__UpdatePresentation(true);
			}
		}

		public override Vector2 Dimensions
		{
			get
			{
				var size = GetComponent<MeshFilter>().sharedMesh.bounds.size;

				return new Vector2(size.x, size.z);
			}
		}

		public override void __UpdatePresentation(bool forceUpdate)
		{
			if (materialCopy == null)
			{
				materialCopy = new Material(GetComponent<Renderer>().sharedMaterial);
			}

			materialCopy.color = color;
			GetComponent<Renderer>().material = materialCopy;
		}

		public override void SetAngle(float angle)
		{
			transform.localRotation = Quaternion.Euler(0, angle, 0);
		}

		public override void AddAngle(float angle)
		{
			transform.localRotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y + angle, 0);
		}

		public void OnDisabel()
		{
			Destroy(materialCopy); 
			materialCopy = null;
		}
	}
}
