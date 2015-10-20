using UnityEngine;
using System.Collections;

public class PlayerUIBinding : MonoBehaviour
{
	public ProgressBarControl m_ExperienceBar = null;
	public ProgressBarControl m_EnergyBar = null;
	public TMPro.TextMeshProUGUI m_LevelText = null;

	protected Player m_Player = null;

	void Start()
	{
		m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		m_Player.LevelChanged += OnLevelChanged;
		m_Player.ExperienceChanged += OnExperienceChanged;
		m_Player.EnergyChanged += OnEnergyChanged;
		m_Player.MaxEnergyChanged += OnMaxEnergyChanged;
	}

	private void OnLevelChanged(uint level)
	{
		m_LevelText.text = level.ToString();

		uint totalLevelXP = Experience.Table[level];
		if( level > 0 )
			totalLevelXP -= Experience.Table[level-1];

		m_ExperienceBar.SetValueRange(0, totalLevelXP);
	}

	private void OnExperienceChanged(float exp)
	{
		float baseExp = 0f;
		if( m_Player.Level > 0 )
			baseExp = Experience.Table[m_Player.Level-1];

		m_ExperienceBar.SetValue( exp - baseExp );
	}


	private void OnEnergyChanged(float energy)
	{
		m_EnergyBar.SetValue( energy );
	}

	private void OnMaxEnergyChanged(float maxEnergy)
	{
		m_EnergyBar.SetValueRange( 0f, maxEnergy );
	}
}