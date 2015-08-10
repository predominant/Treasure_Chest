using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace UnityEngine.UI
{
	public class UIUnitFrame_Bar : MonoBehaviour {
		
		[SerializeField] private RectTransform m_TargetTransform;
		[SerializeField] private int m_MaxValue = 100;
		[SerializeField] private int m_CurValue = 100;
		[SerializeField] private float m_MinWidth = 0f;
		[SerializeField] private float m_MaxWidth = 100f;
		[SerializeField] private Text m_CurValueText;
		[SerializeField] private Text m_MaxValueText;
		[SerializeField] private bool m_TextInPct = false;
		[SerializeField] private UnityEvent m_OnChange = new UnityEvent();
		
		/// <summary>
		/// Gets or sets the bar's max value.
		/// </summary>
		/// <value>The max value.</value>
		public int maxValue {
			get { return this.m_MaxValue; }
			set { this.SetMaxValue(value); }
		}
		
		/// <summary>
		/// Gets or sets the bar's current value.
		/// </summary>
		/// <value>The value.</value>
		public int value {
			get { return this.m_CurValue; }
			set { this.SetValue(value); }
		}
		
#if UNITY_EDITOR
		protected void OnValidate()
		{
			// Validate the current value,
			// make sure it's not greater than the max value
			if (this.m_CurValue > this.m_MaxValue)
				this.m_CurValue = this.m_MaxValue;
			
			// Update the max value text
			if (this.m_MaxValueText != null)
				this.m_MaxValueText.text = this.m_MaxValue.ToString();
			
			// Update the current value text
			this.UpdateText();
			
			// Update the bar fill
			this.UpdateBarFill();
		}
		
		protected void Reset()
		{
			this.m_TargetTransform = this.transform as RectTransform;
			this.m_OnChange = new UnityEvent();
		}
#endif
		
		/// <summary>
		/// Sets the bar's max value.
		/// </summary>
		/// <param name="value">Value.</param>
		public void SetMaxValue(int value)
		{
			if (value < 0)
				value = 0;
			
			// Check if the value changes
			if (this.m_MaxValue != value)
			{
				this.m_MaxValue = value;
				
				// Validate the current value,
				// make sure it's not greater than the max value
				if (this.m_CurValue > this.m_MaxValue)
					this.m_CurValue = this.m_MaxValue;
				
				// Update the max value text
				if (this.m_MaxValueText != null)
					this.m_MaxValueText.text = this.m_MaxValue.ToString();
				
				// Update the bar fill
				this.UpdateBarFill();
			}
		}
		
		/// <summary>
		/// Sets the bar's current value.
		/// </summary>
		/// <param name="value">Value.</param>
		public void SetValue(int value)
		{
			if (value < 0)
				value = 0;
			
			// Check if the value changes
			if (this.m_CurValue != value)
			{
				this.m_CurValue = value;
				
				// Update the current value text
				this.UpdateText();
				
				// Update the bar fill
				this.UpdateBarFill();
				
				// Invoke the on change event
				if (this.m_OnChange != null)
					this.m_OnChange.Invoke();
			}
		}
		
		public void UpdateText()
		{
			// Update the current value text
			if (this.m_CurValueText != null)
			{
				if (this.m_TextInPct)
				{
					this.m_CurValueText.text = Mathf.RoundToInt(((float)this.m_CurValue / (float)this.m_MaxValue) * 100).ToString() + "%";
				}
				else
				{
					this.m_CurValueText.text = this.m_CurValue.ToString();
				}
			}
		}
		
		public void UpdateBarFill()
		{
			if (this.m_TargetTransform == null)
				return;
			
			// Calculate the fill value based on the current bar value
			float fillAmount = ((float)this.m_CurValue / (float)this.m_MaxValue);
			
			// Update the bar fill by changing it's width
			// we are doing it this way because we are using a mask on the bar and have it's fill inside with static width and position
			this.m_TargetTransform.SetSizeWithCurrentAnchors(
				RectTransform.Axis.Horizontal, 
				Mathf.Round(this.m_MinWidth + ((this.m_MaxWidth - this.m_MinWidth) * fillAmount))
			);
		}
	}
}