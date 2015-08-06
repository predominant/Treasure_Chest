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


using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

using BeardedManStudios.Network;

[CustomEditor(typeof(NetworkedMonoBehavior), true), CanEditMultipleObjects]
public class NetworkedMonoBehavior_Editor : Editor
{
	private NetworkedMonoBehavior Target { get { return (NetworkedMonoBehavior)target; } }
	private AnimBool networkingFoldout;
	//private AnimBool lerpFoldout;
	//private AnimBool authoritativeFoldout;
	private AnimBool easyControls;
	private List<ForgeEditorDisplayButton> _editorButtons = new List<ForgeEditorDisplayButton>();
	private GUIStyle _editorStyle;

	void OnEnable()
	{
		//lerpFoldout = new AnimBool(true);
		//authoritativeFoldout = new AnimBool(true);
		networkingFoldout = new AnimBool(true);
		easyControls = new AnimBool(true);
		easyControls.target = true;

		_editorStyle = new GUIStyle();
		_editorStyle.normal.textColor = Color.white;
		_editorStyle.fontStyle = FontStyle.Bold;
		_editorStyle.alignment = TextAnchor.UpperCenter;

		Texture2D btnUp = Resources.Load<Texture2D>("HoverIdle");

		// TODO:  Implement
		//Texture2D btnDwn = Resources.Load<Texture2D>("HoverOver");
		//Texture2D btnIcon = Resources.Load<Texture2D>("ForgeAnvil");

		_editorButtons = new List<ForgeEditorDisplayButton>();

		ForgeButtonStyle buttonStyle = new ForgeButtonStyle("Server Is Authority", btnUp, btnUp, null, 75);
		//ForgeSubmitButtonStyle submitButtonStyle = new ForgeSubmitButtonStyle(() =>
		//{
		//	Debug.Log("Submit success");
		//}, "Toggle");

		ServerAuthorityButton serverAuthorityButton = new ServerAuthorityButton(buttonStyle);
		serverAuthorityButton.Initialize(Target.serverIsAuthority, Target.clientSidePrediction, string.Empty,
		(authorityValue)=>
		{
			Target.serverIsAuthority = authorityValue;
		},
		(clientSidePredictionValue)=>
		{
			Target.clientSidePrediction = clientSidePredictionValue;
		});

		_editorButtons.Add(serverAuthorityButton);

		buttonStyle = new ForgeButtonStyle("Networking Throttle", btnUp, btnUp, null, 100);

		NetworkThrottleButton networkingThrottleButton = new NetworkThrottleButton(buttonStyle);
		networkingThrottleButton.Initialize(Target.networkTimeDelay, "This controls how long (in seconds) to wait before updating across the network (priority can be emulated here), this includes [NetSync] vars.",
		(throttle)=>
		{
			Target.networkTimeDelay = throttle;
		});

		_editorButtons.Add(networkingThrottleButton);

		buttonStyle = new ForgeButtonStyle("Interpolation", btnUp, btnUp, null, 175);

		InterpolationButton interpolationButton = new InterpolationButton(buttonStyle);
		interpolationButton.Initialize(Target.interpolateFloatingValues, Target.lerpT, Target.lerpStopOffset, Target.lerpAngleStopOffset, "",
		(interpolation)=>
		{
			Target.interpolateFloatingValues = interpolation;
		},
		(lerpSpeed)=>
		{
			Target.lerpT = lerpSpeed;
		},
		(distanceStop)=>
		{
			Target.lerpStopOffset = distanceStop;
		},
		(angleStop)=>
		{
			Target.lerpAngleStopOffset = angleStop;
		});

		_editorButtons.Add(interpolationButton);

		buttonStyle = new ForgeButtonStyle("Easy Controls", btnUp, btnUp, null, 150);

		EasyControlsButton easyControlsButton = new EasyControlsButton(buttonStyle);
		easyControlsButton.Initialize(Target.lerpPosition,
			Target.serializePosition,
			Target.lerpRotation, 
			Target.serializeRotation,
			Target.lerpScale,
			Target.serializeScale,
		(lerpPositionChanged) =>
		{
			Target.lerpPosition = lerpPositionChanged;
		},
		(serializePositionChanged) =>
		{
			Target.serializePosition = serializePositionChanged;
		},
		(lerpRotationChanged) =>
		{
			Target.lerpRotation = lerpRotationChanged;
		},
		(serializeRotationChanged) =>
		{
			Target.serializeRotation = serializeRotationChanged;
		},
		(lerpScaleChanged) =>
		{
			Target.lerpScale = lerpScaleChanged;
		},
		(serializeScaleChanged) =>
		{
			Target.serializeScale = serializeScaleChanged;
		});

		_editorButtons.Add(easyControlsButton);

		buttonStyle = new ForgeButtonStyle("Miscellanous", btnUp, btnUp, null, 200);

		MiscellaneousButton miscButton = new MiscellaneousButton(buttonStyle);
		miscButton.Initialize("For things that constantly update like position and rotation you would normally not turn on reliable.",
			Target.isReliable,
			Target.isPlayer,
			Target.destroyOnDisconnect,
			Target.teleportToInitialPositions,
		(udpReliable)=>
		{
			Target.isReliable = udpReliable;
		},
		(isPlayer)=>
		{
			Target.isPlayer = isPlayer;
		},
		(destroyOnDisconnect)=>
		{
			Target.destroyOnDisconnect = destroyOnDisconnect;
		},
		(teleportToInitialPosition)=>
		{
			Target.teleportToInitialPositions = teleportToInitialPosition;
		});

		_editorButtons.Add(miscButton);

		//lerpFoldout.valueChanged.AddListener(Repaint);
		//networkingFoldout.valueChanged.AddListener(Repaint);
		//easyControls.valueChanged.AddListener(Repaint);
	}

	public override void OnInspectorGUI()
	{
		networkingFoldout.target = EditorGUILayout.Foldout(networkingFoldout.target, "Network Controls");

		if (!networkingFoldout.target)
		{
			DrawDefaultInspector();
			return;
		}

		if (EditorGUILayout.BeginFadeGroup(networkingFoldout.faded))
		{
			foreach (ForgeEditorDisplayButton button in _editorButtons)
			{
				button.Update(Event.current, _editorStyle, this, Screen.width, Screen.height);
				GUILayout.Space(10);
			}

			//GUI.color = Color.yellow;
			//EditorGUILayout.HelpBox("This controls how long (in seconds) to wait before updating across the network (priority can be emulated here), this includes [NetSync] vars.", MessageType.None, true);
			//GUI.color = Color.white;
			//if (Target.networkTimeDelay >= 0.03f)
			//	GUI.color = Color.green;
			//else if (Target.networkTimeDelay > 0.00f)
			//	GUI.color = Color.yellow;
			//else
			//	GUI.color = Color.red;
			//Target.networkTimeDelay = EditorGUILayout.FloatField("Network Throttle", Target.networkTimeDelay);
			//GUI.color = Color.white;

			//EditorGUILayout.Space();

			//Target.serverIsAuthority = EditorGUILayout.Toggle("Server Is Authority", Target.serverIsAuthority);
			//authoritativeFoldout.target = Target.serverIsAuthority;
			//if (EditorGUILayout.BeginFadeGroup(authoritativeFoldout.faded))
			//{
			//	Target.clientSidePrediction = EditorGUILayout.Toggle("Client Side Prediction", Target.clientSidePrediction);
			//}

			//EditorGUILayout.EndFadeGroup();

			//EditorGUILayout.Space();

			//GUI.color = Color.yellow;
			//EditorGUILayout.HelpBox("For things that constantly update like position and rotation you would normally not turn on reliable", MessageType.None, true);
			//GUI.color = Color.white;
			//Target.isReliable = EditorGUILayout.Toggle("UDP Reliable", Target.isReliable);

			//EditorGUILayout.Space();

			//Target.interpolateFloatingValues = EditorGUILayout.Toggle("Enable Interpolation", Target.interpolateFloatingValues);
			//lerpFoldout.target = Target.interpolateFloatingValues;

			//EditorGUILayout.Space();

			//Target.isPlayer = EditorGUILayout.Toggle("Is Player", Target.isPlayer);
			//Target.destroyOnDisconnect = EditorGUILayout.Toggle("Destroy on Disconnect", Target.destroyOnDisconnect);
			//Target.teleportToInitialPositions = EditorGUILayout.Toggle("Teleport to Initial Positions", Target.teleportToInitialPositions);

			//EditorGUILayout.Space();

			//if (EditorGUILayout.BeginFadeGroup(lerpFoldout.faded))
			//{
				//GUI.enabled = Target.interpolateFloatingValues;
				//Target.lerpT = EditorGUILayout.Slider("Lerp Speed", Target.lerpT, 0.001f, 1.0f);
				//Target.lerpStopOffset = EditorGUILayout.Slider("Distance Stop", Target.lerpStopOffset, 0.001f, 1.0f);
				//Target.lerpAngleStopOffset = EditorGUILayout.Slider("Angle Stop", Target.lerpAngleStopOffset, 1.0f, 359.0f);
				//GUI.enabled = true;
			//}

			//EditorGUILayout.EndFadeGroup();

			//GUI.color = Color.green;
			//easyControls.target = EditorGUILayout.Foldout(easyControls.target, "Easy Controls");
			//GUI.color = Color.white;
			//if (EditorGUILayout.BeginFadeGroup(easyControls.faded))
			//{
			//	Target.lerpPosition = EditorGUILayout.Toggle("Lerp Position", Target.lerpPosition);
			//	Target.serializePosition = (NetworkedMonoBehavior.SerializeVector3Properties)EditorGUILayout.EnumPopup("Serialize Position", Target.serializePosition);
			//	Target.lerpRotation = EditorGUILayout.Toggle("Lerp Rotation", Target.lerpRotation);
			//	Target.serializeRotation = (NetworkedMonoBehavior.SerializeVector3Properties)EditorGUILayout.EnumPopup("Serialize Rotation", Target.serializeRotation);
			//	Target.lerpScale = EditorGUILayout.Toggle("Lerp Scale", Target.lerpScale);
			//	Target.serializeScale = (NetworkedMonoBehavior.SerializeVector3Properties)EditorGUILayout.EnumPopup("Serialize Scale", Target.serializeScale);
			//}

			//EditorGUILayout.EndFadeGroup();
		}

		EditorGUILayout.EndFadeGroup();

		EditorGUILayout.Space();
		EditorGUILayout.Separator();
		EditorGUILayout.Space();

		// Needed because the enum's keep getting reset
		EditorUtility.SetDirty(Target);

		DrawDefaultInspector();

		Repaint();
	}
}