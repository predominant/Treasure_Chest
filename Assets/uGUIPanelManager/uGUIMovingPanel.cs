
namespace uGUIPanelManager
{

	[System.Serializable]
	public class uGUIMovingPanel
	{
		public uGUIManagedPanel panel;
		public bool isAnimating = false;
		public bool queued = false;
		public bool instant = false;
		public bool additional = false;
		public bool toggle = false;
		public PanelState targetState;
		public PanelState toggleState;
		
		
		public uGUIMovingPanel(uGUIManagedPanel panel, bool show, bool hide, bool queued, bool instant, bool additional)
		{
			this.panel = panel;
			this.queued = queued;
			this.instant = panel.duration == 0 ? true : instant;
			this.additional = additional;
			
			if (show)
				targetState = PanelState.Show;
			if (hide)
				targetState = PanelState.Hide;
		}
		
		public uGUIMovingPanel(uGUIManagedPanel panel, PanelState targetState, bool queued, bool instant, bool additional)
		{
			this.panel = panel;
			this.targetState = targetState;
			this.queued = queued;
			this.instant = panel.duration == 0 ? true : instant;
			this.additional = additional;
		}
		
		public uGUIMovingPanel(uGUIManagedPanel panel, PanelState targetState, PanelState toggleState, bool queued, bool instant, bool additional)
		{
			this.panel = panel;
			this.targetState = targetState;
			this.toggleState = toggleState;
			this.queued = queued;
			this.instant = panel.duration == 0 ? true : instant;
			this.additional = additional;
			this.toggle = true;
		}
	}
}