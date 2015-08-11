using UnityEngine;
using System.Collections;

public class Rotate2D : MonoBehaviour {

    public float m_RotateSpeed = 90f;

	void OnEnable () {
        transform.localRotation = Quaternion.identity;
	}
	
	void Update () {
        transform.localRotation *= Quaternion.EulerRotation(0f, 0f, m_RotateSpeed * Time.deltaTime);
	}
}
