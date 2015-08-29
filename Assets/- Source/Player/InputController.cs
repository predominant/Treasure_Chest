using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InputController : MonoBehaviour
{
    public Transform m_PathLocator = null;
	public LayerMask m_MoveMask;
	public LayerMask m_TouchMask;
    public float m_MoveLocThreshold = 0.2f;
    public float m_MaxInteractRange = 1f;
    protected Seeker m_Seeker = null;
    protected AIPath m_Path = null;
    protected GameObject m_InteractTarget;

    void Start()
    {
        m_Seeker = GetComponent<Seeker>();
        m_Seeker.pathCallback += OnPathComplete;
        m_Path = GetComponent<AIPath>();
        m_Path.MoveComplete += OnMoveComplete;
    }

    void Update()
    {
        Vector3 targetMoveLocation = Vector3.zero;

#if UNITY_EDITOR
        if( !TryInteractMouse() )
            if( TryGetGroundMoveMouse(out targetMoveLocation) )
            {
                //m_Path.endReachedDistance = m_MoveLocThreshold;
                MoveTo(targetMoveLocation);
            }
#else
        if( !TryInteractTouch() )
            if( TryGetGroundMoveTouch(out targetMoveLocation) )
            {
                //m_Path.endReachedDistance = m_MoveLocThreshold;
                MoveTo(targetMoveLocation);
            }
#endif
    }

    /// <summary>
    /// Resets interact target and navigates to position
    /// </summary>
    /// <param name="pos">Where the object will try to move</param>

    #region Movement 
	public void MoveTo(Vector3 pos)
	{
        m_InteractTarget = null;
        m_PathLocator.position = pos;
        m_Path.target = m_PathLocator;
        m_Path.SearchPath();
	}
    private bool TryGetGroundMoveMouse(out Vector3 targetMoveLocation)
    {
        targetMoveLocation = Vector3.zero;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, m_MoveMask))
            {
                targetMoveLocation = hitInfo.point;
                return true;
            }
        }

        return false;
    }
    #endregion

    #region Interaction
    public void SetInteractTarget(GameObject go)
    {
        Interactable i = go.GetComponent<Interactable>();

        if (null == i)
            return;

        Vector3 toTarget = go.transform.position - transform.position;
        float maxInteractRngSqr = m_MaxInteractRange * m_MaxInteractRange;

        if (toTarget.sqrMagnitude > maxInteractRngSqr)
        {
            //m_Path.endReachedDistance = m_MaxInteractRange;
            MoveTo(go.transform.position);
            m_InteractTarget = go;
        }
        else
            i.HandleInteraction();
    }
    private bool TryInteractMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, m_TouchMask))
            {
                SetInteractTarget(hitInfo.collider.gameObject);
                return true;
            }
        }

        return false;
    }
    private bool TryInteractTouch()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended && Input.GetTouch(0).deltaTime <= 0.1f)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, LayerMask.NameToLayer("Touchable")))
            {
                Interactable i = hitInfo.collider.gameObject.GetComponent<Interactable>();

                if (null != i)
                    i.HandleInteraction();

                return true;
            }
        }

        return false;
    }
    #endregion



    private bool TryGetGroundMoveTouch(out Vector3 targetMoveLocation)
    {
        targetMoveLocation = Vector3.zero;
        bool isSingleTouch = Input.touchCount == 1;
        bool isActiveTouch = false;

        if (isSingleTouch)
            isActiveTouch = Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved;

        if (isSingleTouch && isActiveTouch)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
                Debug.Log("Raycast location is valid");
                targetMoveLocation = hitInfo.point;
                return true;
            }
        }

        return false;
    }

    private void OnPathComplete(Pathfinding.Path p)
    {
        m_Path.target = null;
    }

    private void OnMoveComplete()
    {
        if (null == m_InteractTarget)
            return;

        SetInteractTarget(m_InteractTarget);
    }
}
