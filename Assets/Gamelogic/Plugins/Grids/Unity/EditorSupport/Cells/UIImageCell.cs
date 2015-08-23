using UnityEngine;
using UnityEngine.UI;

namespace Gamelogic.Grids
{
	/**
		This component represents a cell that can be used with Unity's GUI system, using an 
		Image compoenent to render the cell.

		It expects an Image component in the hierarchy (on the same game object or a child).

		To use it with grid builders:
			Make a new cell prefab with this component
			Add the grid builder to the canvas or to some child of the canvas
			Set the cell prefab proeprty oof the grid builder to the new prefab.

		Grids ships with a few example prefabs that uses UIImageCells; check them out to 
		see how they work.

		@version1_10
	*/
	public class UIImageCell : TileCell
	{
		private Image image;

		public Image Image
		{
			get
			{
				if (image == null)
				{
					image = this.GetComponentInChildrenAlways<Image>();

					if (image == null)
					{
						Debug.LogError("Cannot retreive Image component from any child.");
					}
				}

				return image;
			}
		}

		public override Color Color
		{
			get { return Image.color; }
			set { Image.color = value; }
		}

		public override Vector2 Dimensions
		{
			get { return Image.rectTransform.rect.size; }
		}

		public override void __UpdatePresentation(bool forceUpdate)
		{
			//Changes are made directly; no update necessary.
		}

		public override void SetAngle(float angle)
		{
			transform.SetLocalRotationZ(angle);
		}

		public override void AddAngle(float angle)
		{
			transform.RotateAroundZ(angle);
		}
	}
}