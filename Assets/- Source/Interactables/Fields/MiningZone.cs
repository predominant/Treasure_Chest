using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Gamelogic.Grids;

public class MiningZone : TileZone
{
    //public List<GameObject> m_MiningNodePrefabs = null;

    //public GameObject[,] m_Nodes = null;
    //public Texture2D m_CurrentGenMask = null;
    //public List<Texture2D> m_GenerationMasks = null;
    ////public Texture2D m_NoiseMap = null;

    //protected FarmPlot m_FarmPlot = null;
	
    //public bool m_UseNoiseGeneration = true;
    //public bool m_UseRandomGeneration = true;
    //public bool m_DbgGenerateTimePaused = true;
    //public float m_DbgGenerateDuration = 0.5f;
    //protected float m_DbgGenerateTimer = 0f;
	
    //// Use this for initializationss
    //protected override void Start()
    //{
    //    base.Start();

    //    ResizeNodesArray();
    //}

    //protected override void Update ()
    //{
    //    base.Update();

    //    if( !m_DbgGenerateTimePaused )
    //        return;

    //    m_DbgGenerateTimer += Time.deltaTime;
    //    if( m_DbgGenerateTimer < m_DbgGenerateDuration )
    //        return;

    //    m_DbgGenerateTimer = 0f;
    //    ResizeNodesArray();
    //}

    //void ResizeNodesArray()
    //{
    //    if( null != m_Nodes )
    //        foreach( GameObject node in m_Nodes )
    //            if( null != node )
    //                DestroyImmediate( node );

    //    m_Nodes = new GameObject[ (int)m_GridSize.x, (int)m_GridSize.y ];
		
    //    for( int x = 0; x < m_GridSize.x; ++x )
    //    {
    //        for( int y = 0; y < m_GridSize.y; ++y )
    //        {
    //            /*int ix = x % 2;
    //            int iy = y % 2;

    //            if( ix == 0 && iy == 0 || ix == 1 && iy == 1 )
    //            {
    //                GameObject node = (GameObject)GameObject.Instantiate( m_MiningNodePrefab );
    //                node.transform.localPosition = Vector3.zero;
    //                node.transform.position = new Vector3( m_MinCellPoint.x + x, m_MinCellPoint.y, m_MinCellPoint.z + y );
    //                m_Nodes[x,y] = node;
    //            }
    //            else
    //                m_Nodes[x,y] = null;*/


    //            Color pixel = Color.white;

    //            if( m_GenerationMasks.Count > 0 )
    //            {
    //                m_CurrentGenMask = m_GenerationMasks[ Random.Range(0, m_GenerationMasks.Count) ];

    //                if( null != m_CurrentGenMask )
    //                    pixel = m_CurrentGenMask.GetPixel( x, y );
    //            }

    //            float noise = 1f;

    //            if( m_UseNoiseGeneration )
    //                noise = Mathf.PerlinNoise( m_MinCell.X + x, m_MinCell.Y + y );

    //            float nodeChanceValue = pixel.b * noise;
    //            float nodeRandChance = 0f;

    //            if( m_UseRandomGeneration )
    //                nodeRandChance = Random.value;

    //            if( nodeChanceValue > nodeRandChance )
    //                CreateMiningNode( x, y, pixel.g, pixel.r );
    //            else
    //                m_Nodes[x,y] = null;
    //        }
    //    }
    //}

    //public void CreateMiningNode( int x, int y, float rarityP = 1f, float yieldP = 1f )
    //{
    //    if( null != m_Nodes[x,y] )
    //        DestroyImmediate( m_Nodes[x,y] );

    //    int prefabIdx = 0;

    //    if( m_UseRandomGeneration )
    //        prefabIdx = Random.Range(0, Mathf.RoundToInt(m_MiningNodePrefabs.Count * rarityP));
    //    else
    //        prefabIdx = Mathf.Clamp( Mathf.RoundToInt(m_MiningNodePrefabs.Count * rarityP), 0, m_MiningNodePrefabs.Count-1 );

    //    GameObject node = (GameObject)GameObject.Instantiate( m_MiningNodePrefabs[prefabIdx] );
    //    node.transform.localPosition = Vector3.zero;
    //    node.transform.position = new Vector3( m_MinCellPoint.x + x, m_MinCellPoint.y, m_MinCellPoint.z + y );

    //    MiningNode nodeScript = node.GetComponent<MiningNode>();
    //    nodeScript.m_Yield = nodeScript.m_MaxYield;//Mathf.RoundToInt(nodeScript.m_MaxYield * yieldP * Random.value);
    //    m_Nodes[x,y] = node;
    //}
}