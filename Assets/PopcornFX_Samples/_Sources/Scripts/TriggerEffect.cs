using UnityEngine;
using System.Collections;

public class TriggerEffect : PKFxPackDependent
{
	public float m_LoopDelay = 1.25f;
	
	IEnumerator Start ()
	{
		yield return WaitForPack(true);
		StartCoroutine(WaitAndRestartEffect(m_LoopDelay));	
	}
	
	IEnumerator  WaitAndRestartEffect(float waitTime)
	{
		var effect = this.GetComponent<PKFxFX>();

	    for (; ; )
        {
			// suspend execution for waitTime seconds
			yield return new WaitForSeconds(waitTime);
			
		    effect.StopEffect();
			effect.StartEffect();
		}
	}
	
}
