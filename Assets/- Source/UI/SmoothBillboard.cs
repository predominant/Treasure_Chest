using UnityEngine;
using System.Collections;

public class SmoothBillboard : MonoBehaviour
{
    [Range(0.1f, 0.99f)]
    public float m_Delay = 0.1f;

    public bool m_YOnly = false;
    public bool m_Spin = false;

    [Range(0.1f, 10f)]
    public float m_SpinSpeed = 1.0f;

    [HideInInspector]
    public float m_SpinOffset = 0f;

    protected Vector3 m_CachedFrom = Vector3.zero;
    protected Vector3 m_CachedTo = Vector3.zero;

	void Update()
    {
        Vector3 targetPos = Camera.main.transform.position;
        if( m_YOnly )
            targetPos.y = transform.position.y;

        // Early out if our locations haven't changed from the previous completed LookAt
        if( targetPos == m_CachedTo && transform.position == m_CachedFrom )
            return;

        Quaternion targetRot = Quaternion.LookRotation( transform.position - targetPos, Vector3.up );

        if( Quaternion.Angle( targetRot, transform.rotation ) < 1f )
        {
            m_CachedFrom = transform.position;
            m_CachedTo = targetPos;
            return;
        }

        if( m_Spin )
        {
            m_SpinOffset += m_SpinSpeed * Time.deltaTime * 360f;
            targetRot *= Quaternion.AngleAxis( m_SpinOffset, Vector3.up );
        }
        else
            m_SpinOffset = 0f;

        if( m_Spin )
            transform.rotation = targetRot;
        else
            transform.rotation = Quaternion.Lerp( transform.rotation, targetRot, m_Delay );
	}
}
