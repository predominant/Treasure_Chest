using UnityEngine;
using System.Collections;

public class CameraLocatorController : MonoBehaviour
{
    public Vector3 m_HeadOffset = Vector3.zero;
    public Quaternion m_TargetOrientation = Quaternion.identity;
    public Vector3 m_TargetPosition = Vector3.zero;
	
	void Update()
    {
        transform.rotation = m_TargetOrientation;
        transform.localPosition = m_HeadOffset + m_TargetPosition;
	}
}
