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
                .SetEventAttribute("JobType", "Mining")
                .SetEventAttribute("SubType", "Coal")
                .Send( (response) =>
                {
                    if (response.HasErrors)
                        Debug.Log("Node collection net request failed");
                    else
                    {
                        int? itemAmt = response.ScriptData.GetInt("Amount");
                        int? itemType = response.ScriptData.GetInt("Type");
                        int? expAmt = response.ScriptData.GetInt("Experience");

                        Debug.Log("Item: " + ((Item.Type)(itemType)).ToString() + ", Yield: " + itemAmt + ", Experience: " + expAmt );
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