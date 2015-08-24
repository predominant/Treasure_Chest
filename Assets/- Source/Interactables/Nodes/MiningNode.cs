using System;
using System.Collections;
using UnityEngine;
using GameSparks;
using GameSparks.Core;
using GameSparks.Api;
using GameSparks.Api.Requests;

public class MiningNode : MonoBehaviour, Interactable
{
    public Node.Type m_NodeType = Node.Type.MiningOre;
    public Node.MiningOreType m_NodeSubType = Node.MiningOreType.Coal;
    private bool m_AwaitingResponse = false;

    public void HandleInteraction()
    {
        if( GS.Available )
        {
            if (m_AwaitingResponse)
                return;

            m_AwaitingResponse = true;

            new LogEventRequest()
                .SetEventKey("NODE_COLLECT")
                .SetEventAttribute("Type", 0)
                .SetEventAttribute("SubType", 0)
                .Send( (response) =>
                {
                    if (response.HasErrors)
                        Debug.Log("Node collection net request failed");
                    else
                    {
                        int? yieldAmt = response.ScriptData.GetInt("Amount");
                        int? itemType = response.ScriptData.GetInt("Type");

                        Debug.Log("Item: " + ((Item.Type)(itemType)).ToString() + ", Yield " + yieldAmt);
                        GameObject.Destroy(this.gameObject);
                    }
                });
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}