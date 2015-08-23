using UnityEngine;
using System.Collections;

public class MiningNode : Interactable
{	
    //public string m_Harvest = "";
    //public int m_Yield = 1;
    //public int m_MaxYield = 1;
    //public int m_Durability = 1;

	public void HandleInteraction()
	{
        //if( tool.GetType() == typeof( HammerTool ) )
        //{
        //    --m_Durability;

        //    if( m_Durability > 0 )
        //        return;

        //    if( m_Harvest != "" && m_Yield > 0 )
        //    {
        //        /*Inventory inventory = GameObject.Find( "Player" ).GetComponent<Inventory>();
        //        inventory.Add( m_Harvest, m_Yield );*/

        //        ItemMap itemMap = GameObject.Find( "SceneMaster" ).GetComponent<ItemMap>();
        //        Item item = itemMap.m_Categories.FindItem( m_Harvest );

        //        if( item != null )
        //        {
        //            GameObject obj = (GameObject)GameObject.Instantiate( item.m_Prefab );
        //            obj.transform.position = transform.position;
        //            obj.transform.localPosition = transform.localPosition;
        //            obj.transform.rotation = transform.rotation;
        //        }
        //        else
        //            Debug.LogWarning( "Could not find mining node harvest '" + m_Harvest + "'" );
        //    }

        //    DestroyImmediate( gameObject );
        //}
	}
}