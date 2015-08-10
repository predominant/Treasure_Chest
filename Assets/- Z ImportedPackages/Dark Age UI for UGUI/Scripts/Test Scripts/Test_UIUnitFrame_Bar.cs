using System;
using UnityEngine;
using UnityEngine.UI.Tweens;
using System.Collections;

namespace UnityEngine.UI
{
	public class Test_UIUnitFrame_Bar : MonoBehaviour {
		
		public UIUnitFrame_Bar bar;
		public float duration = 4f;
		
		// Tween controls
		[NonSerialized] private readonly TweenRunner<FloatTween> m_FloatTweenRunner;
		
		// Called by Unity prior to deserialization, 
		// should not be called by users
		protected Test_UIUnitFrame_Bar()
		{
			if (this.m_FloatTweenRunner == null)
				this.m_FloatTweenRunner = new TweenRunner<FloatTween>();
			
			this.m_FloatTweenRunner.Init(this);
		}
		
		protected void OnEnable()
		{
			this.StartDemoTween();
		}
		
		protected void SetFillAmount(float amount)
		{
			if (this.bar != null)
				this.bar.value = Mathf.RoundToInt((float)this.bar.maxValue * amount);
		}
		
		public void StartDemoTween()
		{
			if (this.bar == null)
				return;
			
			float pct = (float)this.bar.value / (float)this.bar.maxValue;
			float targetAmount = (pct > 0.5f) ? 0f : 1f;
			
			FloatTween floatTween = new FloatTween { duration = this.duration, startFloat = pct, targetFloat = targetAmount };
			floatTween.AddOnChangedCallback(SetFillAmount);
			floatTween.AddOnFinishCallback(OnTweenFinished);
			floatTween.ignoreTimeScale = true;
			this.m_FloatTweenRunner.StartTween(floatTween);
		}
		
		protected void OnTweenFinished()
		{
			this.StartDemoTween();
		}
	}
}