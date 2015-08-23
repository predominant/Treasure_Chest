using UnityEngine;
using System.Collections;
using Gamelogic.Grids;

//[RequireComponent( typeof( ColliderBoundsEx ) )]
public class TileZone : MonoBehaviour
{
    //public Vector3 m_MinPoint = Vector3.zero;
    //public Vector3 m_MaxPoint = Vector3.zero;
    //public Vector3 m_MinCellPoint = Vector3.zero;
    //public Vector3 m_MaxCellPoint = Vector3.zero;
    //public RectPoint m_MinCell = RectPoint.Zero;
    //public RectPoint m_MaxCell = RectPoint.Zero;

    //public Vector3 m_Centroid = Vector3.zero;

    //public Vector2 m_Size = Vector2.zero;
    //public Vector2 m_GridSize = Vector2.zero;

    //protected bool m_SelectionOnGrid = false;
    //protected RectPoint m_SelectedPoint = RectPoint.Zero;
    //protected Vector3 m_SelectedPosition = Vector3.zero;
    ////protected ToolFramework m_ToolFramework = null;
    //protected GameObject m_ToolField = null;

    //protected virtual void Start()
    //{
    //    //m_ToolFramework = GameObject.Find( "Player" ).GetComponent<ToolFramework>();
		
    //    ColliderBoundsEx bounds = GetComponent<ColliderBoundsEx>();
		
    //    m_MinPoint = bounds.GetColliderVertex( ColliderBoundsEx.VertexType.MinBL );
    //    m_MaxPoint = bounds.GetColliderVertex( ColliderBoundsEx.VertexType.MaxBR );
		
    //    m_MinCell = WorldGridMap.Instance.GetClosestCell( m_MinPoint );
    //    m_MaxCell = WorldGridMap.Instance.GetClosestCell( m_MaxPoint );
		
    //    m_MinCellPoint = WorldGridMap.Instance.GetCellPosition( m_MinCell );
    //    m_MaxCellPoint = WorldGridMap.Instance.GetCellPosition( m_MaxCell );
    //    m_MinCellPoint.y = m_MaxCellPoint.y = transform.localScale.y * 0.5f * 0f;
		
    //    m_Centroid = (m_MaxCellPoint-m_MinCellPoint)*0.5f + m_MinCellPoint;
    //    m_Size.x = m_MaxCellPoint.x-m_MinCellPoint.x + WorldGridMap.Instance.CellWidth;
    //    m_Size.y = m_MaxCellPoint.z-m_MinCellPoint.z + WorldGridMap.Instance.CellHeight;
		
    //    transform.position = m_Centroid;
    //    transform.localScale = new Vector3( m_Size.x, transform.localScale.y, m_Size.y );	
		
    //    m_GridSize.x = Mathf.Abs( m_MaxCell.X - m_MinCell.X ) + 1;
    //    m_GridSize.y = Mathf.Abs( m_MaxCell.Y - m_MinCell.Y ) + 1;
    //}

    //// Update is called once per frame
    //protected virtual void Update()
    //{
    //    if( null == m_ToolField )
    //        return;

    //    Debug.DrawLine( m_MinCellPoint, m_MinCellPoint + Vector3.up * 10f, Color.green );
    //    Debug.DrawLine( m_MaxCellPoint, m_MaxCellPoint + Vector3.up * 10f, Color.red );
		
    //    //m_SelectedPoint = WorldGridMap.Instance.GetClosestCell( m_ToolFramework.m_ToolHighlight.transform.position );
    //    m_SelectionOnGrid = PointInZone( m_SelectedPoint );
    //    //m_SelectedPosition = WorldGridMap.Instance.GetCellPosition( m_SelectedPoint );
		
    //    // If the selection point is not on the grid, return
    //    //if( m_SelectionOnGrid )
    //        //m_ToolFramework.m_ToolHighlightActive = true;
    //    //else
    //    //{
    //        //m_ToolFramework.m_ToolHighlightActive = false;
    //        //return;
    //    //}
    //}
	
    //protected bool PointInZone( RectPoint point )
    //{
    //    if( point.X >= m_MinCell.X &&
    //       point.X <= m_MaxCell.X &&
    //       point.Y >= m_MinCell.Y &&
    //       point.Y <= m_MaxCell.Y )
    //        return true;
		
    //    return false;
    //}
	
    //public void SetToolField( GameObject toolField )
    //{
    //    m_ToolField = toolField;
    //}
	
    //public void ClearToolField()
    //{	
    //    m_ToolField = null;
    //    //m_ToolFramework.m_ToolHighlightActive = false;
    //}
}