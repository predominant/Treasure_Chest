using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace uGUIPanelManager
{

	[RequireComponent(typeof(Button))]
	public class uGUIPanelSwitcher : MonoBehaviour
	{



		public string panelName;
		public PanelState targetState;
		public PanelState toggleState;

		public bool toggle = false;
		public bool additional = false;
		public bool queued = true;


		private Button cachedButton;
		public Button CachedButton
		{
			get
			{
				if (cachedButton == null)
					cachedButton = this.GetComponent<Button>();
				return cachedButton;
			}
		}

		// Use this for initialization
		void Awake()
		{
			CachedButton.onClick.AddListener(this.OnClick);
		}

		void OnClick()
		{
			if (this.panelName == "")
			{
				return;
			}

			if (toggle)
			{
				uGUIManager.instance.TogglePanelStateInst(this.panelName, this.targetState, this.toggleState, this.additional, this.queued);
			} else
			{
				uGUIManager.instance.SetPanelStateInst(this.panelName, this.targetState, this.additional, this.queued);
			}

		}

	}
}
