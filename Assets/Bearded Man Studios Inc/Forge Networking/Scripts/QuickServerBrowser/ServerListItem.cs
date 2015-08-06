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



using BeardedManStudios.Network;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ServerListItem : MonoBehaviour
{
	public Text ServerNameLabel;
	public Text PlayerCountLabel;
	public Text GameTypeLabel;
	public Toggle PasswordToggle;
	public Button JoinButton;
	private HostInfo hostInfo;

	public void SetupServerListItem(HostInfo host)
	{
		hostInfo = host;
		ServerNameLabel.text = host.name;
		PlayerCountLabel.text = host.connectedPlayers + "/" + host.maxPlayers;
		GameTypeLabel.text = host.gameType;
		PasswordToggle.isOn = !string.IsNullOrEmpty(host.password) && host.password.Length > 0;
	}

	public void JoinServer()
	{
		if (hostInfo != null && ServerListManager.Instance != null)
		{
			//Join the server
			ServerListManager.Instance.JoinServer(hostInfo);
		}
	}
}
