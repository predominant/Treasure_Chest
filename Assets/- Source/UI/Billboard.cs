using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
    public bool m_YOnly = false;

    void Update()
    {
        Vector3 targetPos = Camera.main.transform.position;
        if( m_YOnly )
            targetPos.y = transform.position.y;

        Quaternion targetRot = Quaternion.LookRotation( transform.position - targetPos, Vector3.up );
        transform.rotation = targetRot;
    }
}
