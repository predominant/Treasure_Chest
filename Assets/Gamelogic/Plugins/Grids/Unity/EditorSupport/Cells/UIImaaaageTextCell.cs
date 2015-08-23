using UnityEngine;
using UnityEngine.UI;

namespace Gamelogic.Grids
{
	/**
		This is the same as UIImageCell, except that it supports a text component as well.
		
		This class is also suitable to use with Unity's Buttons  (since they are rendered with 
		Image and Text components).

		In addition to the Image component that is expected down the hierarchy, this component 
		also expects a Text component down the hierarchy (on this object or a child).

		@version1_10
	*/
	public class UIImageTextCell : UIImageCell
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

		/**
			@version1_10_1
		*/
		public Color TextColor
		{
			get { return UIText.color; }
			set { UIText.color = value; }
		}
	}
}
