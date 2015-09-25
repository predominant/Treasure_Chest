using UnityEngine;
using System.Collections;

public class PlayerInputEventProxy : MonoBehaviour
{
	protected InputController m_PlayerInputController;

	void Start()
	{
		m_PlayerInputController = GameObject.FindGameObjectWithTag( "Player" ).GetComponent<InputController>();
	}
	
	public void OnPointerDown()
	{
		m_PlayerInputController.OnInteract();
	}
}