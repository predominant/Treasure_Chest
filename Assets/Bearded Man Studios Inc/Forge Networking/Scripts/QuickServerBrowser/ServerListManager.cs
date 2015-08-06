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
using BeardedManStudios.Network.API;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ServerListManager : MonoBehaviour
{
	public static ServerListManager Instance;
	public GameObject ServerListItemPrefab;
	public GameObject PasswordPrompt;
	public InputField PasswordField;
	public Text ErrorResponseLabel;
	public Transform Grid;
	private HostInfo _lastHost;

#if NETFX_CORE && !UNITY_EDITOR
	private bool isWinRT = true;
#else
	private bool isWinRT = false;
#endif

	public string masterServerIp = string.Empty;

	public void Awake()
	{
		Instance = this;
		PasswordField.contentType = InputField.ContentType.Password;
	}

	public void Start()
	{
		RefreshServerList();
	}

	private void RefreshServerList()
	{
		ForgeMasterServer.GetHosts(masterServerIp, 0, (HostInfo[] hosts) =>
		{
			BeardedManStudios.Network.Unity.MainThreadManager.Run(delegate()
			{
				for (int i = Grid.childCount - 1; i >= 0; i--)
					Destroy(Grid.GetChild(0).gameObject);

				foreach (HostInfo host in hosts)
				{
					GameObject gO = Instantiate(ServerListItemPrefab) as GameObject;
					gO.transform.parent = Grid;
					gO.GetComponent<ServerListItem>().SetupServerListItem(host);
				}
			});
		});
	}

	public void JoinServer(HostInfo host)
	{
		if (!string.IsNullOrEmpty(host.password))
		{
			_lastHost = host;
			//Show password prompt
			PasswordPrompt.SetActive(true);
		}
		else
		{
			//Join server
			FinalizeJoin(host);
		}
	}

	private void FinalizeJoin(HostInfo host)
	{
		NetWorker socket = Networking.Connect(host.IpAddress, host.port,
			host.connectionType == "udp" ? Networking.TransportationProtocolType.UDP : Networking.TransportationProtocolType.TCP, isWinRT);

		if (socket.Connected)
		{
			Networking.SetPrimarySocket(socket);
			Application.LoadLevel(host.sceneName);
		}
		else
		{
			socket.connected += delegate()
			{
				Networking.SetPrimarySocket(socket);
				Application.LoadLevel(host.sceneName);
			};
		}
	}

	public void JoinServerWithPW()
	{
		if (_lastHost == null)
		{
			ClosePasswordPrompt();
			return;
		}

		if (PasswordField.text == _lastHost.password)
		{
			//Password succeeded
			ErrorResponseLabel.enabled = false;
			FinalizeJoin(_lastHost);
			ClosePasswordPrompt();
		}
		else
		{
			ErrorResponseLabel.enabled = true;
			ErrorResponseLabel.text = "Incorrect Password";
		}
	}

	public void ClosePasswordPrompt()
	{
		PasswordField.text = string.Empty;
		PasswordField.ActivateInputField();

		ErrorResponseLabel.enabled = false;
		PasswordPrompt.SetActive(false);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
			RefreshServerList();
	}
}