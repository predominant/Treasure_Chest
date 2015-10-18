using System;
using System.Collections;
using UnityEngine;

public class MiningNode : Node
{
	public enum MiningNodeType
	{
		Coal = 0,
		Diamond
	}

	public MiningNodeType m_NodeType = MiningNodeType.Coal;

	public override Job.Type JobType { get { return Job.Type.Mining; } }
	public override string NodeTypeName { get { return m_NodeType.ToString(); } }
}