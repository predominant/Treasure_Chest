/*
 * Copyright (C) 2012-2015 Soomla Inc. - All Rights Reserved
 *
 *   Unauthorized copying of this file, via any medium is strictly prohibited
 *   Proprietary and confidential
 *
 *   Written by Refael Dakar <refael@soom.la>
 */
 
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Soomla.Highway
{
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public class HighwayManifestTools : ISoomlaManifestTools
    {
#if UNITY_EDITOR
		static HighwayManifestTools instance = new HighwayManifestTools();
		static HighwayManifestTools()
		{
			SoomlaManifestTools.ManTools.Add(instance);
		}

		public void UpdateManifest() {
			SoomlaManifestTools.SetPermission("android.permission.INTERNET");
			SoomlaManifestTools.SetPermission("android.permission.ACCESS_NETWORK_STATE");

			//google-play-services.jar version
			SoomlaManifestTools.AddMetaDataTag("com.google.android.gms.version", "@integer/google_play_services_version");
		}
#endif
	}
}
