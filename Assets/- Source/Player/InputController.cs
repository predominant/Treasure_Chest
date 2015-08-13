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
        Vector3 targetMoveLocation = Vector3.zero;

#if UNITY_EDITOR
        bool setMoveLocation = TryGetGroundMoveMouse(out targetMoveLocation);
#else
        bool setMoveLocation = TryGetGroundMoveTouch(out targetMoveLocation);
#endif

        if ( setMoveLocation )
        {
            m_PathLocator.position = targetMoveLocation;
            m_Path.target = m_PathLocator;
            m_Path.SearchPath();
        }
    }

    private bool TryGetGroundMoveMouse(out Vector3 targetMoveLocation)
    {
        targetMoveLocation = Vector3.zero;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
                targetMoveLocation = hitInfo.point;
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
            Debug.Log("Touch is valid");
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
