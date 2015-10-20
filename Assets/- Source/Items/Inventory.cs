using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameSparks;
using GameSparks.Core;
using GameSparks.Api;
using GameSparks.Api.Requests;
using Devdog.InventorySystem;

public class Inventory
{
	public InventoryManager m_InventoryManager = null;

	public Inventory( InventoryManager _inventoryManager )
	{
		m_InventoryManager = _inventoryManager;
		Net_SyncData();
	}

	#region Server Actions
	public void Net_SyncData()
	{
		new AccountDetailsRequest()
			.Send((response) =>
			{
				if (response.HasErrors)
				{
					Debug.Log("[GS] ACCOUNT_DETAILS_REQUEST failed: " + response.Errors.JSON);
				}
				else
				{
					Dictionary<string, int> items = new Dictionary<string,int>();
					foreach( string item in response.VirtualGoods.BaseData.Keys )
					{
						int amt = (int)(double)response.VirtualGoods.BaseData[item];
						items.Add( item, amt );
					}

					//m_InventoryManager.inventory.AddItem( )	
					//m_InventoryManager.inventory.
				}
			});
	}

	#endregion
}