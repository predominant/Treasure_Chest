using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Integration.SimpleJson;
using Devdog.InventorySystem.Models;
using UnityEngine;

namespace Devdog.InventorySystem
{
    public class JsonCollectionSerializer : ICollectionSerializer
    {
        /// <summary>
        /// Serialize to JSON and return as byte array with UTF8 encoding.
        /// </summary>
        /// <param name="toSerialize"></param>
        /// <returns>UTF-8 encoded JSON</returns>
        public byte[] SerializeItems(IList<Models.InventoryItemSaveLookup> toSerialize)
        {
            var jsonArr = new JsonObject();
            jsonArr.Add("data", toSerialize);

            return Encoding.UTF8.GetBytes(jsonArr.ToString());
        }

        public byte[] SerializeItemReferences(IList<Models.InventoryItemReferenceSaveLookup> toSerialize)
        {
            var jsonArr = new JsonObject();
            jsonArr.Add("data", toSerialize);

            return Encoding.UTF8.GetBytes(jsonArr.ToString());
        }

        public IList<Models.InventoryItemSaveLookup> DeserializeItems(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            if (string.IsNullOrEmpty(json))
                return new List<InventoryItemSaveLookup>();

            var dict = (IDictionary<string, object>)Devdog.InventorySystem.Integration.SimpleJson.SimpleJson.DeserializeObject(json); // Full namespace required, unity 5.1 has some internal lib?

            //return SimpleJson.DeserializeObject<InventoryItemSaveLookup[]>(json);
            var list = new List<InventoryItemSaveLookup>(64);
            foreach (var obj in (List<object>)dict["data"])
            {
                var o = (IDictionary<string, object>) obj;
                list.Add(new InventoryItemSaveLookup(int.Parse(o["itemID"].ToString()), uint.Parse(o["amount"].ToString())));
            }

            return list;
        }

        public IList<Models.InventoryItemReferenceSaveLookup> DeserializeItemReferences(byte[] data)
        {

            return null;
        }
    }
}
