using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Similar to a sprite cell, but with custom UV coordinates.
		This type of cell is useful when placing a single texture 
		across multiple cells.		

		@link_making_your_own_cells for guidelines on making your own cell.

		@version1_8
		@ingroup UnityComponents
	*/
	public class UVCell : TileCell
	{
		[SerializeField]
		public MapPlane plane = MapPlane.XY;

		[SerializeField]
		private Color color;

		[SerializeField]
		private Texture2D texture;

		[SerializeField]
		private Vector2 textureScale;

		[SerializeField]
		private Vector2 textureOffset;

		[SerializeField]
		[HideInInspector]
		private Material material;

		public override Color Color
		{
			get { return color; }

			set
			{
				color = value;
				__UpdatePresentation(true);
			}
		}

		public Material Material
		{
			get { return material; }
		}

		public override Vector2 Dimensions
		{
			get
			{
				switch (plane)
				{
					case MapPlane.XY:
					default:
						return GetComponent<MeshFilter>().sharedMesh.bounds.size.To2DXY();
					case MapPlane.XZ:
						return GetComponent<MeshFilter>().sharedMesh.bounds.size.To2DXZ();
				}
			}
		}

		public void SetTexture(Texture2D texture)
		{
			this.texture = texture;
			__UpdatePresentation(true);
		}

		public void SetUVs(Vector2 offset, Vector2 scale)
		{
			textureOffset = offset;
			textureScale = scale;
			__UpdatePresentation(true);
		}

		public override void __UpdatePresentation(bool forceUpdate)
		{
			if (material == null)
			{
				material = new Material(GetComponent<Renderer>().sharedMaterial); //only duplicate once
			}

			material.color = color;
			material.mainTexture = texture;
			material.mainTextureOffset = textureOffset;
			material.mainTextureScale = textureScale;

			GetComponent<Renderer>().material = material;
		}

		public override void SetAngle(float angle)
		{
			transform.SetLocalRotationZ(angle);
		}

		public override void AddAngle(float angle)
		{
			transform.RotateAroundZ(angle);
		}

		public void OnDestroy()
		{
			DestroyImmediate(material);
		}
	}
}
