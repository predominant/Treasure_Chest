using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputController : MonoBehaviour
{
    public Transform m_PathLocator = null;
	public LayerMask m_MoveMask;
	public LayerMask m_TouchMask;
    protected Seeker m_Seeker = null;
    protected AIPath m_Path = null;


    void Start()
    {
        m_Seeker = GetComponent<Seeker>();
        m_Seeker.pathCallback += OnPathComplete;
        m_Path = GetComponent<AIPath>();
    }

    void Update()
    {
        Vector3 targetMoveLocation = Vector3.zero;

#if UNITY_EDITOR
        bool interacted = TryInteractMouse();
        bool setMoveLocation = false;
        if( !interacted )
            setMoveLocation = TryGetGroundMoveMouse(out targetMoveLocation);
#else
        bool interacted = TryInteractMouse();
        bool setMoveLocation = false;
        if( !interacted )
            setMoveLocation = TryGetGroundMoveTouch(out targetMoveLocation);
#endif

        if ( setMoveLocation )
        {
            m_PathLocator.position = targetMoveLocation;
            m_Path.target = m_PathLocator;
            m_Path.SearchPath();
        }
    }

    private bool TryInteractMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
            RaycastHit hitInfo;

            if ( Physics.Raycast(ray, out hitInfo, Mathf.Infinity, m_TouchMask))
            {
                Interactable i = hitInfo.collider.gameObject.GetComponent<Interactable>();

                if (null != i)
                    i.HandleInteraction();

                return true;
            }
        }

        return false;
    }

    private bool TryGetGroundMoveMouse(out Vector3 targetMoveLocation)
    {
        targetMoveLocation = Vector3.zero;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if( Physics.Raycast( ray, out hitInfo, Mathf.Infinity, m_MoveMask ) )
            {
                targetMoveLocation = hitInfo.point;
                return true;
            }
        }

        return false;
    }

    private bool TryInteractTouch()
    {
        if( Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended && Input.GetTouch(0).deltaTime <= 0.1f )
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
}
