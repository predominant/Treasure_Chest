
using Gamelogic;
using Gamelogic.Grids;
using UnityEngine;
using UnityEngine.UI;

/**
		This component represents a cell that can be used with Unity's GUI system, using a 
		Text component to render the cell.

		It expects a Text component in the hierarchy (on the same game object or a child).

		To use it with grid builders:
			Make a new cell prefab with this component
			Add the grid builder to the canvas or to some child of the canvas
			Set the cell prefab proeprty oof the grid builder to the new prefab.

		Grids ships with a few example prefabs that uses UITextCells; check them out to 
		see how they work.

		@version1_10
	*/
public class UITextCell : TileCell
{
	private Text text;

	private Text UIText
	{
		get
		{
			if (text == null)
			{
				text = this.GetComponentInChildrenAlways<Text>();

				if (text == null)
				{
					Debug.LogError("Cannot retreive Text component from any child.");
				}
			}

			return text;
		}
	}

	public string Text
	{
		get { return UIText.text; }
		set { UIText.text = value; }
	}

	public override Color Color
	{
		get { return UIText.color; }
		set { UIText.color = value; }
	}

	public override Vector2 Dimensions
	{
		get { return UIText.rectTransform.rect.size; }
	}

	public override void __UpdatePresentation(bool forceUpdate)
	{
		//
	}

	public override void SetAngle(float angle)
	{
		//Always keep upright
	}

	public override void AddAngle(float angle)
	{
		//Always keep upright
	}
}
