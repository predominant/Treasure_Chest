using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour
{
	public float m_Speed = 25.0f;
    void Update()
    {
		this.transform.eulerAngles += new Vector3(0, Time.deltaTime * m_Speed, 0);
///		this.transform.rotation +
    }
}
