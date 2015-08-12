using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour
{
    public Transform m_PathLocator = null;
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
        bool isSingleTouch = Input.touchCount == 1;
        bool isActiveTouch = false;
        bool setMoveLocation = false;
        Vector3 targetMoveLocation = Vector3.zero;
        //m_Path.target = null;

        
#if UNITY_EDITOR
        if( Input.GetMouseButtonDown(0) )
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
                setMoveLocation = true;
                targetMoveLocation = hitInfo.point;
            }
        }
#else
        if (isSingleTouch)
            isActiveTouch = Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved;

        if (isSingleTouch && isActiveTouch)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, LayerMask.NameToLayer("Ground")))
            {
                setMoveLocation = true;
                targetMoveLocation = hitInfo.point;
            }
        }
#endif

        if ( setMoveLocation )
        {
            m_PathLocator.position = targetMoveLocation;
            m_Path.target = m_PathLocator;
            m_Path.TrySearchPath();
        }
    }

    private void OnPathComplete(Pathfinding.Path p)
    {
        m_Path.target = null;
    }
}
