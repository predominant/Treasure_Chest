/*-----------------------------+------------------------------\
|                                                             |
|                        !!!NOTICE!!!                         |
|                                                             |
|  These libraries are under heavy development so they are    |
|  subject to make many changes as development continues.     |
|  For this reason, the libraries may not be well commented.  |
|  THANK YOU for supporting forge with all your feedback      |
|  suggestions, bug reports and comments!                     |
|                                                             |
|                               - The Forge Team              |
|                                 Bearded Man Studios, Inc.   |
|                                                             |
|  This source code, project files, and associated files are  |
|  copyrighted by Bearded Man Studios, Inc. (2012-2015) and   |
|  may not be redistributed without written permission.       |
|                                                             |
\------------------------------+-----------------------------*/



using UnityEngine;
using SimpleJSON;
using BeardedManStudios.Network;
using BeardedManStudios.Network.API;
using System.Collections.Generic;
using System;

namespace BeardedManStudios.Network.API
{
	public class ArbiterAPI : MonoBehaviour
	{
		public string apiVersion = "v1.0";
		public string applicationId = string.Empty;
		public string privateKey = string.Empty;
		public string accessKey = string.Empty;

		public void Awake() { DontDestroyOnLoad(gameObject); API.Initialize(apiVersion, applicationId, privateKey, accessKey); }
	}
}