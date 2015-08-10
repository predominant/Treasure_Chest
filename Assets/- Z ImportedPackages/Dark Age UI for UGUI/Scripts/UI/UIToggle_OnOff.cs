using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	[ExecuteInEditMode, RequireComponent(typeof(UnityEngine.UI.Toggle))]
	public class UIToggle_OnOff : MonoBehaviour, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler {
		
		[SerializeField] private Image m_Target;
		[SerializeField] private Sprite m_ActiveSprite;
		[SerializeField] private Image m_InactivePress;
		[SerializeField] private Image m_ActivePress;
		[SerializeField] private bool m_InstaOut = true;
		
		public Toggle toggle {
			get { return this.gameObject.GetComponent<Toggle>(); }
		}
		
		protected void OnEnable()
		{
			this.toggle.onValueChanged.AddListener(OnValueChanged);
			this.DoTransition(false, true);
			this.OnValueChanged(this.toggle.isOn);
		}
		
		protected void OnDisable()
		{
			this.toggle.onValueChanged.RemoveListener(OnValueChanged);
		}
		
#if UNITY_EDITOR
		protected void OnValidate()
		{
			this.DoTransition(false, true);
		}
#endif
		
		public void OnValueChanged(bool state)
		{
			if (this.m_Target == null || !this.isActiveAndEnabled)
				return;
			
			this.m_Target.overrideSprite = (state) ? this.m_ActiveSprite : null;
		}
		
		private void DoTransition(bool pressed, bool instant)
		{
			if (!this.isActiveAndEnabled)
				return;
			
			// Transition the active pressed
			if (this.toggle.isOn)
			{
				// If the active pressed was set
				if (this.m_ActivePress != null)
				{
					if (instant || (!pressed && this.m_InstaOut))
						this.m_ActivePress.canvasRenderer.SetAlpha((pressed ? 1f : 0f));
					else
						this.m_ActivePress.CrossFadeAlpha((pressed ? 1f : 0f), 0.1f, true);
				}
				
				// Transition the opposite pressed state to not pressed
				// If the inactive pressed was set
				if (this.m_InactivePress != null)
				{
					if (instant || this.m_InstaOut)
						this.m_InactivePress.canvasRenderer.SetAlpha(0f);
					else
						this.m_InactivePress.CrossFadeAlpha(0f, 0.1f, true);
				}
			}
			else // Transition to inactive pressed
			{
				// If the inactive pressed was set
				if (this.m_InactivePress != null)
				{
					if (instant || (!pressed && this.m_InstaOut))
						this.m_InactivePress.canvasRenderer.SetAlpha((pressed ? 1f : 0f));
					else
						this.m_InactivePress.CrossFadeAlpha((pressed ? 1f : 0f), 0.1f, true);
				}
				
				// Transition the opposite pressed state to not pressed
				// If the active pressed was set
				if (this.m_ActivePress != null)
				{
					if (instant || this.m_InstaOut)
						this.m_ActivePress.canvasRenderer.SetAlpha(0f);
					else
						this.m_ActivePress.CrossFadeAlpha(0f, 0.1f, true);
				}
			}
		}
		
		public virtual void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;
			
			this.DoTransition(true, false);
		}
		
		public virtual void OnPointerUp(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;
			
			this.DoTransition(false, false);
		}
	}
}