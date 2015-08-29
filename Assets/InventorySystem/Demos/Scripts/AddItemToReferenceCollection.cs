using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Devdog.InventorySystem.Demo
{
	public class AddItemToReferenceCollection : MonoBehaviour {
	
		public InventoryItemBase item; // The item we wish to store
		public ItemCollectionBase referenceCollection; // The reference collection to move it to
		
		public void Start ()
		{
			var i = GameObject.Instantiate<InventoryItemBase> (item); // Create a copy, remember, only instance objects are allowed to be stored in collections.
		    var storedItems = new List<InventoryItemBase>();
            bool added = InventoryManager.AddItem(i, storedItems); // Add the item to an inventory (loot to collection)
			if (added && storedItems.First().itemCollection != null)
				storedItems.First().itemCollection.MoveItem (i, i.index, referenceCollection, 0, false); // Move the item to a reference collection, a reference doesn't destroy the item from it's original location.
			
			// And remove it
			referenceCollection.SetItem(0, null, true);
		}
	}
}