using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using DG.Tweening;

public class GatherNodeUIBinding : MonoBehaviour
{
	public Node m_Node = null;
	public GameObject m_AvailablePanel = null;
	public GameObject m_UnavailablePanel = null;
	public TMPro.TextMeshProUGUI m_Countdown = null;
	public Image m_Fillbar = null;
	public Color m_FillStartColor = Color.red;
	public Color m_FillEndColor = Color.blue;

	private bool m_IsAvailable = false;

	void Start()
	{
		m_Node.AvailableTimeChanged += OnAvailableTimeChange;
	}

	void Update()
	{
		if(!m_IsAvailable)
		{
			TimeSpan toNextAvailable = m_Node.m_AvailableTime - DateTime.UtcNow;
			m_Countdown.text = Utility.TimeString( toNextAvailable );
			
			if( m_Node.IsAvailable )
			{
				m_IsAvailable = true;
				SetAvailablePanels( true );
			}
		}
	}

	void SetAvailablePanels(bool isAvailable)
	{
		if( m_AvailablePanel.activeSelf != isAvailable )
			m_AvailablePanel.SetActive(isAvailable);

		if( m_UnavailablePanel.activeSelf == isAvailable ) 
			m_UnavailablePanel.SetActive(!isAvailable);
	}

	void OnAvailableTimeChange()
	{
		m_IsAvailable = m_Node.IsAvailable;
		SetAvailablePanels(m_IsAvailable);

		if( !m_IsAvailable )
		{
			// Quickly animate to empty
			m_Fillbar.fillClockwise = false;

			m_Fillbar.DOColor( m_FillStartColor, 1f ).SetEase(Ease.Linear);
			m_Fillbar.DOFillAmount(0, 1f).SetEase(Ease.Linear)
				
				// Animate until full / ready
				.OnComplete( () =>
				{
					m_Fillbar.fillClockwise = true;
					m_Fillbar.DOFillAmount(1f, m_Node.SecondsToAvailable).SetEase(Ease.Linear);
					m_Fillbar.DOColor(m_FillEndColor, m_Node.SecondsToAvailable).SetEase(Ease.Linear);
				});
		}
	}
}