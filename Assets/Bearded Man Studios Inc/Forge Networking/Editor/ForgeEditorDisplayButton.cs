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

using System.Security.AccessControl;
using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;
using BeardedManStudios.Network;

public class ForgeSubmitButtonStyle
{
	public System.Action SubmitSuccess;
	public string SubmitTitle;
	public int SubmitButtonWidth;
	public int SubmitButtonHeight;

	public ForgeSubmitButtonStyle(System.Action submitSuccess = null, string title = "", int width = 0, int height = 0)
	{
		SubmitSuccess = submitSuccess;
		SubmitTitle = title;
		SubmitButtonWidth = width;
		SubmitButtonHeight = height;
	}
}

public class ForgeButtonStyle
{
	public string ButtonTitle;
	public Texture2D ButtonIcon;
	public Texture2D ButtonUp;
	public Texture2D ButtonDown;
	public System.Action ButtonSubmitAction;
	public int ButtonHeight;
	public GUIStyle ButtonStyle;

	public ForgeButtonStyle(string title, Texture2D up, Texture2D down, System.Action submitAction = null, int height = 50, Texture2D icon = null, GUIStyle style = null)
	{
		ButtonTitle = title;
		ButtonIcon = icon;
		ButtonUp = up;
		ButtonDown = down;
		ButtonSubmitAction = submitAction;
		ButtonHeight = height;
		ButtonStyle = style;
	}
}

public class ForgeEditorDisplayButton
{
	//public string TextFieldReturn
	//{
	//	get { return _buttonTextField; }
	//}

	protected System.Action _submitCallback;
	protected Texture2D _buttonUp;
	protected Texture2D _buttonDown;
	protected Texture2D _buttonIcon;
	protected Texture2D _activeTex;
	protected Texture2D _prevTex;
	protected GUIContent _buttonContent;
	protected GUIStyle _buttonStyle;
	protected Rect _buttonRect;
	
	//protected int _buttonWidth;
	protected int _buttonHeight;

	protected string _buttonTitle;
	protected string _buttonText;

	//protected bool TextFieldEnabled;
	//protected string _buttonTextField;

	//Submit button
	protected bool _submitButtonVisibile = false;
	protected string _submitButtonTitle;
	protected System.Action _submitButtonAction;
	protected int _submitButtonWidth;
	protected int _submitButtonHeight;

	public ForgeEditorDisplayButton(ForgeButtonStyle buttonStyle, ForgeSubmitButtonStyle submitStyle = null)
	{
		_buttonUp = buttonStyle.ButtonUp;
		_buttonDown = buttonStyle.ButtonDown;
		_buttonIcon = buttonStyle.ButtonIcon;
		_submitCallback = buttonStyle.ButtonSubmitAction;
		_buttonUp.wrapMode = TextureWrapMode.Repeat;
		_buttonContent = new GUIContent(_buttonUp);
		_activeTex = _buttonUp;

		//_buttonTextField = string.Empty;
		_buttonTitle = buttonStyle.ButtonTitle;

		if (submitStyle != null)
		{
			_submitButtonVisibile = true;
			_submitButtonTitle = submitStyle.SubmitTitle;
			_submitButtonAction = submitStyle.SubmitSuccess;
			_submitButtonWidth = submitStyle.SubmitButtonWidth;
			_submitButtonHeight = submitStyle.SubmitButtonHeight;
		}

		_buttonHeight = buttonStyle.ButtonHeight;

		if (buttonStyle.ButtonStyle != null)
			_buttonStyle = buttonStyle.ButtonStyle;
		else
		{
			_buttonStyle = new GUIStyle();

		}
	}

	public virtual void Update(Event windowEvent, GUIStyle fontStyle, Editor window, int width, int height)
	{
		//if (_activeTex == null)
		//	return;

		//_buttonRect = GUILayoutUtility.GetRect(_buttonContent, _buttonStyle, GUILayout.Height(_buttonHeight));
		//Rect textField = new Rect(_buttonRect);
		//textField.xMin += 10;
		//textField.xMax -= width * 0.32f;
		//textField.yMin += 20;
		//textField.yMax -= 10;
		//Rect iconField = new Rect(_buttonRect);
		//iconField.xMin += 10;
		//iconField.xMax -= (width - 60);
		//iconField.yMin += 5;
		//iconField.yMax -= 35;

		//if (windowEvent.isMouse && _buttonRect.Contains(windowEvent.mousePosition))
		//{
		//	if (windowEvent.type == EventType.MouseDown)
		//	{
		//		if (_buttonDown != null)
		//		{
		//			_activeTex = _buttonDown;
		//			_buttonContent.image = _buttonUp;
		//		}
		//	}
		//	else if (windowEvent.type == EventType.MouseUp)
		//	{
		//		if (_buttonUp != null)
		//		{
		//			_activeTex = _buttonUp;
		//			_buttonContent.image = _buttonUp;
		//		}

		//		if (_submitCallback != null)
		//			_submitCallback();
		//	}
		//}
		//else if (windowEvent.isMouse)
		//{
		//	if (_buttonUp != null)
		//	{
		//		_activeTex = _buttonUp;
		//		_buttonContent.image = _buttonUp;
		//	}
		//}

		//GUI.DrawTexture(_buttonRect, _activeTex, ScaleMode.StretchToFill);

		//if (_buttonIcon != null)
		//	GUI.DrawTexture(iconField, _buttonIcon, ScaleMode.StretchToFill);

		//if (fontStyle != null)
		//	GUI.Label(_buttonRect, _buttonTitle, fontStyle);

		//_buttonTextField = GUI.TextField(textField, _buttonTextField, 32);

		//if (_submitButtonVisibile)
		//{
		//	Rect submitField = new Rect(_buttonRect);
		//	submitField.xMin += (width * 0.68f) + _submitButtonWidth;
		//	submitField.xMax -= 10 - _submitButtonWidth;
		//	submitField.yMin += 20 + _submitButtonHeight;
		//	submitField.yMax -= 10 - _submitButtonHeight;

		//	if (GUI.Button(submitField, _submitButtonTitle))
		//	{
		//		if (_submitButtonAction != null)
		//			_submitButtonAction();
		//	}
		//}

		//if (_prevTex != _activeTex)
		//{
		//	_prevTex = _activeTex;
		//	window.Repaint();
		//}
		//GUI.Button(_buttonRect, _buttonContent);
	}
}

public class ServerAuthorityButton : ForgeEditorDisplayButton
{
	#region Server Is Authority
	private bool _serverIsAuthorityPrev = false;
	private bool _serverIsAuthority = false;
	public bool ServerIsAuthority
	{
		get { return _serverIsAuthority; }
		set
		{
			if (value != _serverIsAuthorityPrev)
			{
				_serverIsAuthority = value;
				_serverIsAuthorityPrev = value;

				if (_authorityChanged != null)
					_authorityChanged(value);
			}
		}
	}
	private System.Action<bool> _authorityChanged;
	#endregion

	#region Client Side Prediciton
	private bool _clientSidePredictionPrev = false;
	private bool _clientSidePrediction = false;
	public bool ClientSidePrediction
	{
		get { return _clientSidePrediction; }
		set
		{
			if (value != _clientSidePredictionPrev)
			{
				_clientSidePrediction = value;
				_clientSidePredictionPrev = value;

				if (_clientSidePredictionChanged != null)
					_clientSidePredictionChanged(value);
			}
		}
	}
	private System.Action<bool> _clientSidePredictionChanged;
	#endregion

	private string _toolTiptext;
	private bool _clientSidePredictionAllowed = false;
	private bool _selfButton = false;

	public ServerAuthorityButton(ForgeButtonStyle buttonStyle, ForgeSubmitButtonStyle submitStyle = null) : base(buttonStyle, submitStyle)
	{
	}

	public void Initialize(bool authority, bool clientSidePrediction, string tooltipText, System.Action<bool> authorityChanged, System.Action<bool> clientSidePredictionChanged = null)
	{
		ServerIsAuthority = authority;
		ClientSidePrediction = clientSidePrediction;
		_toolTiptext = tooltipText;

		_authorityChanged = authorityChanged;
		_clientSidePredictionChanged = clientSidePredictionChanged;
		_clientSidePredictionAllowed = _clientSidePredictionChanged != null;

		if (!_submitButtonVisibile)
		{
			_selfButton = true;
			_submitButtonVisibile = true;
			_submitButtonTitle = ServerIsAuthority ? "Disable" : "Enable";
		}
	}

	public override void Update(Event windowEvent, GUIStyle fontStyle, Editor window, int width, int height)
	{
		if (_activeTex == null)
			return;

		Rect original = GUILayoutUtility.GetRect(_buttonContent, _buttonStyle, GUILayout.Height(_buttonHeight));
		_buttonRect = original;
		//if (!ServerIsAuthority)
		//	_buttonRect.height -= 25;

		Rect textField = new Rect(original);
		textField.xMin += 10;
		textField.xMax -= width * 0.32f;
		textField.yMin += 20;
		if (_clientSidePredictionAllowed)
			textField.yMax -= 20;
		else
			textField.yMax -= 10;
		Rect iconField = new Rect(original);
		iconField.xMin += 10;
		iconField.xMax -= (width - 60);
		iconField.yMin += 5;
		iconField.yMax -= 35;

		if (windowEvent.isMouse && _buttonRect.Contains(windowEvent.mousePosition))
		{
			if (windowEvent.type == EventType.MouseDown)
			{
				if (_buttonDown != null)
				{
					_activeTex = _buttonDown;
					_buttonContent.image = _buttonUp;
				}
			}
			else if (windowEvent.type == EventType.MouseUp)
			{
				if (_buttonUp != null)
				{
					_activeTex = _buttonUp;
					_buttonContent.image = _buttonUp;
				}

				if (_submitCallback != null)
					_submitCallback();
			}
		}
		else if (windowEvent.isMouse)
		{
			if (_buttonUp != null)
			{
				_activeTex = _buttonUp;
				_buttonContent.image = _buttonUp;
			}
		}

		GUI.DrawTexture(_buttonRect, _activeTex, ScaleMode.StretchToFill);

		if (_buttonIcon != null)
			GUI.DrawTexture(iconField, _buttonIcon, ScaleMode.StretchToFill);

		if (fontStyle != null)
			GUI.Label(_buttonRect, _buttonTitle, fontStyle);

		if (!string.IsNullOrEmpty(_toolTiptext))
		{
			GUI.color = Color.yellow;
			GUI.TextArea(textField, _toolTiptext);
			GUI.color = Color.white;
		}

		if (_submitButtonVisibile)
		{
			bool fullScreenButton = string.IsNullOrEmpty(_toolTiptext);

			Rect submitField = new Rect(original);
			submitField.xMin += !fullScreenButton ? (width*0.68f) + _submitButtonWidth : 10 + _submitButtonWidth;
			submitField.xMax -= 10 - _submitButtonWidth;
			submitField.yMin += 20 + _submitButtonHeight;
			submitField.yMax -= _clientSidePredictionAllowed ? 35 - _submitButtonHeight : 10 - _submitButtonHeight;

			Rect clientsidePredField = new Rect(original);
			clientsidePredField.xMin += !fullScreenButton ? (width * 0.68f) + _submitButtonWidth : 10 + _submitButtonWidth;
			clientsidePredField.xMax -= 10 - _submitButtonWidth;
			clientsidePredField.yMin += 45 + _submitButtonHeight;
			clientsidePredField.yMax -= _clientSidePredictionAllowed ? 15 - _submitButtonHeight : 10 - _submitButtonHeight;

			if (_selfButton)
			{
				_submitButtonTitle = ServerIsAuthority ? "Disable" : "Enable";
			}

			GUI.color = ServerIsAuthority ? Color.green : Color.gray;

			if (GUI.Button(submitField, _submitButtonTitle))
			{
				if (_selfButton)
				{
					ServerIsAuthority = !ServerIsAuthority;
					_submitButtonTitle = ServerIsAuthority ? "Disable" : "Enable";
				}

				if (_submitButtonAction != null)
					_submitButtonAction();
			}
			GUI.color = Color.white;

			GUI.enabled = ServerIsAuthority;

			GUI.color = ClientSidePrediction ? Color.green : Color.gray;
			string clientSideText = ClientSidePrediction ? "Disable Client side Prediction" : "Enable Client side Prediction";
			if (GUI.Button(clientsidePredField, clientSideText))
			{
				if (_selfButton)
				{
					ClientSidePrediction = !ClientSidePrediction;
				}
			}
			GUI.color = Color.white;

			GUI.enabled = true;
		}

		if (_prevTex != _activeTex)
		{
			_prevTex = _activeTex;
			window.Repaint();
		}
	}
}

public class NetworkThrottleButton : ForgeEditorDisplayButton
{
	#region Networking Throttle
	private float _networkingThrottle = 0.5f;
	private float _networkingThrottlePrev = 0.5f;

	public float NetworkThrottle
	{
		get { return _networkingThrottle; }
		set
		{
			if (value != _networkingThrottlePrev)
			{
				_networkingThrottle = value;
				_networkingThrottlePrev = value;

				if (_networkingThrottleChanged != null)
					_networkingThrottleChanged(_networkingThrottle);
			}
		}
	}
	private System.Action<float> _networkingThrottleChanged = null;
	#endregion

	private string _toolTiptext;

	public NetworkThrottleButton(ForgeButtonStyle buttonStyle, ForgeSubmitButtonStyle submitStyle = null) : base(buttonStyle, submitStyle)
	{
	}

	public void Initialize(float initialThrottle, string tooltipText, System.Action<float> networkingThrottleChanged)
	{
		_toolTiptext = tooltipText;
		_networkingThrottle = initialThrottle;
		_networkingThrottlePrev = initialThrottle;
		_networkingThrottleChanged = networkingThrottleChanged;
	}

	public override void Update(Event windowEvent, GUIStyle fontStyle, Editor window, int width, int height)
	{
		if (_activeTex == null)
			return;

		_buttonRect = GUILayoutUtility.GetRect(_buttonContent, _buttonStyle, GUILayout.Height(_buttonHeight));
		Rect iconField = new Rect(_buttonRect);
		iconField.xMin += 10;
		iconField.xMax -= (width - 60);
		iconField.yMin += 5;
		iconField.yMax -= 35;

		Rect throttleField = new Rect(_buttonRect);
		throttleField.xMin += 100;
		throttleField.xMax -= 10;
		throttleField.yMin += 75;
		throttleField.yMax -= 5;

		Rect throttleLabelField = new Rect(_buttonRect);
		throttleLabelField.xMin += 10;
		throttleLabelField.xMax -= (width - 125);
		throttleLabelField.yMin += 75;
		throttleLabelField.yMax -= 8;

		if (string.IsNullOrEmpty(_toolTiptext))
			_buttonRect.yMax -= 50;

		if (windowEvent.isMouse && _buttonRect.Contains(windowEvent.mousePosition))
		{
			if (windowEvent.type == EventType.MouseDown)
			{
				if (_buttonDown != null)
				{
					_activeTex = _buttonDown;
					_buttonContent.image = _buttonUp;
				}
			}
			else if (windowEvent.type == EventType.MouseUp)
			{
				if (_buttonUp != null)
				{
					_activeTex = _buttonUp;
					_buttonContent.image = _buttonUp;
				}

				if (_submitCallback != null)
					_submitCallback();
			}
		}
		else if (windowEvent.isMouse)
		{
			if (_buttonUp != null)
			{
				_activeTex = _buttonUp;
				_buttonContent.image = _buttonUp;
			}
		}

		GUI.DrawTexture(_buttonRect, _activeTex, ScaleMode.StretchToFill);

		if (_buttonIcon != null)
			GUI.DrawTexture(iconField, _buttonIcon, ScaleMode.StretchToFill);

		if (fontStyle != null)
			GUI.Label(_buttonRect, _buttonTitle, fontStyle);

		if (!string.IsNullOrEmpty(_toolTiptext))
		{
			Rect textField = new Rect(_buttonRect);
			textField.xMin += 10;
			textField.xMax -= 10;
			textField.yMin += 20;
			textField.yMax -= 35;

			GUI.color = Color.yellow;
			GUI.TextArea(textField, _toolTiptext);
			GUI.color = Color.white;
		}
		else
		{
			throttleField.yMin -= 60;
			throttleField.yMax -= 60;
			throttleLabelField.yMin -= 60;
			throttleLabelField.yMax -= 60;
		}

		NetworkThrottle = GUI.HorizontalSlider(throttleField, NetworkThrottle, 0f, 100f);

		if (NetworkThrottle >= 0.03f)
			GUI.color = Color.green;
		else if (NetworkThrottle > 0.00f)
			GUI.color = Color.yellow;
		else
			GUI.color = Color.red;

		float tempThrottle = NetworkThrottle;
		if (float.TryParse(GUI.TextField(throttleLabelField, NetworkThrottle.ToString()), out tempThrottle))
		{
			NetworkThrottle = tempThrottle;
		}

		GUI.color = Color.white;

		if (_prevTex != _activeTex)
		{
			_prevTex = _activeTex;
			window.Repaint();
		}
	}
}

public class InterpolationButton : ForgeEditorDisplayButton
{
	#region Interpolation
	private bool _interpolationPrev = false;
	private bool _interpolation = false;
	public bool Interpolation
	{
		get { return _interpolation; }
		set
		{
			if (value != _interpolationPrev)
			{
				_interpolation = value;
				_interpolationPrev = value;

				if (_interpolationChanged != null)
					_interpolationChanged(value);
			}
		}
	}
	private System.Action<bool> _interpolationChanged;
	#endregion

	#region Lerp Speed
	private float _lerpSpeedPrev = 0;
	private float _lerpSpeed = 0;
	public float LerpSpeed
	{
		get { return _lerpSpeed; }
		set
		{
			if (value != _lerpSpeedPrev)
			{
				_lerpSpeed = value;
				_lerpSpeedPrev = value;

				if (_lerpSpeedChanged != null)
					_lerpSpeedChanged(value);
			}
		}
	}
	private System.Action<float> _lerpSpeedChanged;
	#endregion

	#region Distance Stop
	private float _distanceStopPrev = 0;
	private float _distanceStop = 0;
	public float DistanceStop
	{
		get { return _distanceStop; }
		set
		{
			if (value != _distanceStopPrev)
			{
				_distanceStop = value;
				_distanceStopPrev = value;

				if (_distanceStopChanged != null)
					_distanceStopChanged(value);
			}
		}
	}
	private System.Action<float> _distanceStopChanged;
	#endregion

	#region Angle Stop
	private float _angleStopPrev = 0;
	private float _angleStop = 0;
	public float AngleStop
	{
		get { return _angleStop; }
		set
		{
			if (value != _angleStopPrev)
			{
				_angleStop = value;
				_angleStopPrev = value;

				if (_angleStopChanged != null)
					_angleStopChanged(value);
			}
		}
	}
	private System.Action<float> _angleStopChanged;
	#endregion

	private string _toolTiptext;
	private bool _interpolationButtonVisible;

	public InterpolationButton(ForgeButtonStyle buttonStyle, ForgeSubmitButtonStyle submitStyle = null) : base(buttonStyle, submitStyle)
	{
	}

	public void Initialize(bool interpolation, float lerpSpeed, float distanceStop, float angleStop, string tooltipText, System.Action<bool> interpolationChanged, System.Action<float> lerpSpeedChanged, System.Action<float> distanceStopChanged, System.Action<float> angleStopChanged)
	{
		Interpolation = interpolation;
		LerpSpeed = lerpSpeed;
		DistanceStop = distanceStop;
		AngleStop = angleStop;
		_toolTiptext = tooltipText;

		_interpolationChanged = interpolationChanged;
		_lerpSpeedChanged = lerpSpeedChanged;
		_distanceStopChanged = distanceStopChanged;
		_angleStopChanged = angleStopChanged;

		if (!_submitButtonVisibile)
		{
			_submitButtonVisibile = true;
			_submitButtonTitle = Interpolation ? "Disable" : "Enable";
		}
	}

	public override void Update(Event windowEvent, GUIStyle fontStyle, Editor window, int width, int height)
	{
		if (_activeTex == null)
			return;

		Rect original = GUILayoutUtility.GetRect(_buttonContent, _buttonStyle, GUILayout.Height(_buttonHeight));
		_buttonRect = original;

		Rect textField = new Rect(original);
		textField.xMin += 10;
		textField.xMax -= width * 0.32f;
		textField.yMin += 20;
		textField.yMax -= 10;

		Rect iconField = new Rect(original);
		iconField.xMin += 10;
		iconField.xMax -= (width - 60);
		iconField.yMin += 5;
		iconField.yMax -= 35;

		Rect lerpSpeedLabelField = new Rect(original);
		lerpSpeedLabelField.xMin += 10;
		lerpSpeedLabelField.xMax -= (width - 100);
		lerpSpeedLabelField.yMin += 75;
		lerpSpeedLabelField.yMax -= 82;

		Rect distanceStopLabelField = new Rect(original);
		distanceStopLabelField.xMin += 10;
		distanceStopLabelField.xMax -= (width - 100);
		distanceStopLabelField.yMin += 107;
		distanceStopLabelField.yMax -= 50;

		Rect angleStopLabelField = new Rect(original);
		angleStopLabelField.xMin += 10;
		angleStopLabelField.xMax -= (width - 100);
		angleStopLabelField.yMin += 140;
		angleStopLabelField.yMax -= 18;

		if (windowEvent.isMouse && _buttonRect.Contains(windowEvent.mousePosition))
		{
			if (windowEvent.type == EventType.MouseDown)
			{
				if (_buttonDown != null)
				{
					_activeTex = _buttonDown;
					_buttonContent.image = _buttonUp;
				}
			}
			else if (windowEvent.type == EventType.MouseUp)
			{
				if (_buttonUp != null)
				{
					_activeTex = _buttonUp;
					_buttonContent.image = _buttonUp;
				}

				if (_submitCallback != null)
					_submitCallback();
			}
		}
		else if (windowEvent.isMouse)
		{
			if (_buttonUp != null)
			{
				_activeTex = _buttonUp;
				_buttonContent.image = _buttonUp;
			}
		}

		GUI.DrawTexture(_buttonRect, _activeTex, ScaleMode.StretchToFill);

		if (_buttonIcon != null)
			GUI.DrawTexture(iconField, _buttonIcon, ScaleMode.StretchToFill);

		if (fontStyle != null)
			GUI.Label(_buttonRect, _buttonTitle, fontStyle);

		if (!string.IsNullOrEmpty(_toolTiptext))
		{
			GUI.color = Color.yellow;
			GUI.TextArea(textField, _toolTiptext);
			GUI.color = Color.white;
		}

		if (_submitButtonVisibile)
		{
			bool fullScreenButton = string.IsNullOrEmpty(_toolTiptext);

			Rect submitField = new Rect(original);
			submitField.xMin += !fullScreenButton ? (width * 0.68f) + _submitButtonWidth : 10 + _submitButtonWidth;
			submitField.xMax -= 10 - _submitButtonWidth;
			submitField.yMin += 20 + _submitButtonHeight;
			submitField.yMax -= 125 - _submitButtonHeight;

			Rect lerpSpeedLabel = new Rect(original);
			lerpSpeedLabel.xMin += 10;
			lerpSpeedLabel.xMax -= 10;
			lerpSpeedLabel.yMin += 62;
			lerpSpeedLabel.yMax -= 55;

			Rect distanceStopLabel = new Rect(original);
			distanceStopLabel.xMin += 10;
			distanceStopLabel.xMax -= 10;
			distanceStopLabel.yMin += 93;
			distanceStopLabel.yMax -= 25;

			Rect angleStopLabel = new Rect(original);
			angleStopLabel.xMin += 10;
			angleStopLabel.xMax -= 10;
			angleStopLabel.yMin += 125;
			angleStopLabel.yMax -= 25;

			Rect lerpSpeedField = new Rect(original);
			lerpSpeedField.xMin += (width * 0.25f) + _submitButtonWidth;
			lerpSpeedField.xMax -= 10 - _submitButtonWidth;
			lerpSpeedField.yMin += 75 + _submitButtonHeight;
			lerpSpeedField.yMax -= 75;

			Rect distanceStopField = new Rect(original);
			distanceStopField.xMin += (width * 0.25f) + _submitButtonWidth;
			distanceStopField.xMax -= 10 - _submitButtonWidth;
			distanceStopField.yMin += 107 + _submitButtonHeight;
			distanceStopField.yMax -= 45;

			Rect angleStopField = new Rect(original);
			angleStopField.xMin += (width * 0.25f) + _submitButtonWidth;
			angleStopField.xMax -= 10 - _submitButtonWidth;
			angleStopField.yMin += 140 + _submitButtonHeight;
			angleStopField.yMax -= 10;

			_submitButtonTitle = Interpolation ? "Disable" : "Enable";

			GUI.color = Interpolation ? Color.green : Color.gray;

			if (GUI.Button(submitField, _submitButtonTitle))
			{
				Interpolation = !Interpolation;
				_submitButtonTitle = Interpolation ? "Disable" : "Enable";

				if (_submitButtonAction != null)
					_submitButtonAction();
			}
			GUI.color = Color.white;

			GUI.Label(lerpSpeedLabel, "Lerp Speed", fontStyle);

			GUI.Label(distanceStopLabel, "Distance Stop", fontStyle);

			GUI.Label(angleStopLabel, "Angle Stop", fontStyle);

			GUI.enabled = Interpolation;

			LerpSpeed = GUI.HorizontalSlider(lerpSpeedField, LerpSpeed, 0f, 1f);

			float tempLerpSpeed = LerpSpeed;
			if (float.TryParse(GUI.TextField(lerpSpeedLabelField, LerpSpeed.ToString()), out tempLerpSpeed))
			{
				LerpSpeed = tempLerpSpeed;
			}

			DistanceStop = GUI.HorizontalSlider(distanceStopField, DistanceStop, 0f, 1f);

			float tempDistanceStop = DistanceStop;
			if (float.TryParse(GUI.TextField(distanceStopLabelField, DistanceStop.ToString()), out tempDistanceStop))
			{
				DistanceStop = tempDistanceStop;
			}

			AngleStop = GUI.HorizontalSlider(angleStopField, AngleStop, 1f, 359.0f);

			float tempAngleStop = AngleStop;
			if (float.TryParse(GUI.TextField(angleStopLabelField, AngleStop.ToString()), out tempAngleStop))
			{
				AngleStop = tempAngleStop;
			}

			GUI.enabled = true;
		}

		if (_prevTex != _activeTex)
		{
			_prevTex = _activeTex;
			window.Repaint();
		}
	}
}

public class EasyControlsButton : ForgeEditorDisplayButton
{
	#region Lerp Position
	private bool _lerpPositionPrev = false;
	private bool _lerpPosition = false;
	public bool LerpPosition
	{
		get { return _lerpPosition; }
		set
		{
			if (value != _lerpPositionPrev)
			{
				_lerpPosition = value;
				_lerpPositionPrev = value;

				if (_lerpPositionChanged != null)
					_lerpPositionChanged(value);
			}
		}
	}
	private System.Action<bool> _lerpPositionChanged;
	#endregion

	#region SerializePosition
	private NetworkedMonoBehavior.SerializeVector3Properties _serializePositionPrev;
	private NetworkedMonoBehavior.SerializeVector3Properties _serializePosition;
	public NetworkedMonoBehavior.SerializeVector3Properties SerializePosition
	{
		get { return _serializePosition; }
		set
		{
			if (value != _serializePositionPrev)
			{
				_serializePosition = value;
				_serializePositionPrev = value;

				if (_serializePositionChanged != null)
					_serializePositionChanged(value);
			}
		}
	}
	private System.Action<NetworkedMonoBehavior.SerializeVector3Properties> _serializePositionChanged;
	#endregion

	#region Lerp Rotation
	private bool _lerpRotationPrev = false;
	private bool __lerpRotation = false;
	public bool LerpRotation
	{
		get { return __lerpRotation; }
		set
		{
			if (value != _lerpRotationPrev)
			{
				__lerpRotation = value;
				_lerpRotationPrev = value;

				if (_lerpRotationChanged != null)
					_lerpRotationChanged(value);
			}
		}
	}
	private System.Action<bool> _lerpRotationChanged;
	#endregion

	#region Serialize Rotation
	private NetworkedMonoBehavior.SerializeVector3Properties _serializeRotationPrev;
	private NetworkedMonoBehavior.SerializeVector3Properties _serializeRotation;
	public NetworkedMonoBehavior.SerializeVector3Properties SerializeRotation
	{
		get { return _serializeRotation; }
		set
		{
			if (value != _serializeRotationPrev)
			{
				_serializeRotation = value;
				_serializeRotationPrev = value;

				if (_serializeRotationChanged != null)
					_serializeRotationChanged(value);
			}
		}
	}
	private System.Action<NetworkedMonoBehavior.SerializeVector3Properties> _serializeRotationChanged;
	#endregion

	#region Lerp Scale
	private bool _lerpScalePrev = false;
	private bool _lerpScale = false;
	public bool LerpScale
	{
		get { return _lerpScale; }
		set
		{
			if (value != _lerpScalePrev)
			{
				_lerpScale = value;
				_lerpScalePrev = value;

				if (_lerpScaleChanged != null)
					_lerpScaleChanged(value);
			}
		}
	}
	private System.Action<bool> _lerpScaleChanged;
	#endregion

	#region Serialize Scale
	private NetworkedMonoBehavior.SerializeVector3Properties _serializeScalePrev;
	private NetworkedMonoBehavior.SerializeVector3Properties _serializeScale;
	public NetworkedMonoBehavior.SerializeVector3Properties SerializeScale
	{
		get { return _serializeScale; }
		set
		{
			if (value != _serializeScalePrev)
			{
				_serializeScale = value;
				_serializeScalePrev = value;

				if (_serializeScaleChanged != null)
					_serializeScaleChanged(value);
			}
		}
	}
	private System.Action<NetworkedMonoBehavior.SerializeVector3Properties> _serializeScaleChanged;
	#endregion

	private bool _easyControlsButtonVisible;
	private bool _lerpPosX, _lerpPosY , _lerpPosZ;
	private bool _lerpRotX, _lerpRotY, _lerpRotZ;
	private bool _lerpScaleX, _lerpScaleY, _lerpScaleZ;

	private const int ADJUSTABLE_HEIGHT_MIN = -10;
	private const int ADJUSTABLE_HEIGHT_MAX = -5;

	public EasyControlsButton(ForgeButtonStyle buttonStyle, ForgeSubmitButtonStyle submitStyle = null)
		: base(buttonStyle, submitStyle)
	{
	}

	public void Initialize(bool lerpPosition, 
		NetworkedMonoBehavior.SerializeVector3Properties serializePosition, 
		bool lerpRotation, 
		NetworkedMonoBehavior.SerializeVector3Properties serializeRotation, 
		bool lerpScale, 
		NetworkedMonoBehavior.SerializeVector3Properties serializeScale,
		System.Action<bool> lerpPositionChanged,
		System.Action<NetworkedMonoBehavior.SerializeVector3Properties> serializePositionChanged,
		System.Action<bool> lerpRotationChanged,
		System.Action<NetworkedMonoBehavior.SerializeVector3Properties> serializeRotationChanged,
		System.Action<bool> lerpScaleChanged,
		System.Action<NetworkedMonoBehavior.SerializeVector3Properties> serializeScaleChanged)
	{
		LerpPosition = lerpPosition;
		SerializePosition = serializePosition;
		LerpRotation = lerpRotation;
		SerializeRotation = serializeRotation;
		LerpScale = lerpScale;
		SerializeScale = serializeScale;

		SetSerializedVector3ToBools(SerializePosition, ref _lerpPosX, ref _lerpPosY, ref _lerpPosZ);
		SetSerializedVector3ToBools(SerializeRotation, ref _lerpRotX, ref _lerpRotY, ref _lerpRotZ);
		SetSerializedVector3ToBools(SerializeScale, ref _lerpScaleX, ref _lerpScaleY, ref _lerpScaleZ);

		_easyControlsButtonVisible = true;
		_submitButtonVisibile = true;

		_lerpPositionChanged = lerpPositionChanged;
		_serializePositionChanged = serializePositionChanged;
		_lerpRotationChanged = lerpRotationChanged;
		_serializeRotationChanged = serializeRotationChanged;
		_lerpScaleChanged = lerpScaleChanged;
		_serializeScaleChanged = serializeScaleChanged;
	}

	public override void Update(Event windowEvent, GUIStyle fontStyle, Editor window, int width, int height)
	{
		if (_activeTex == null)
			return;

		Rect original = GUILayoutUtility.GetRect(_buttonContent, _buttonStyle, GUILayout.Height(_buttonHeight));
		_buttonRect = original;

		Rect lerpPositionField = new Rect(original);
		lerpPositionField.xMin += 10;
		lerpPositionField.xMax -= (width - (width * 0.35f));
		lerpPositionField.yMin += 55 + ADJUSTABLE_HEIGHT_MIN;
		lerpPositionField.yMax -= 90 + ADJUSTABLE_HEIGHT_MAX;

		Rect lerpPositionXField = new Rect(original);
		lerpPositionXField.xMin += (width - (width * 0.68f));
		lerpPositionXField.xMax -= (width - (width * 0.55f));
		lerpPositionXField.yMin += 55 + ADJUSTABLE_HEIGHT_MIN;
		lerpPositionXField.yMax -= 90 + ADJUSTABLE_HEIGHT_MAX;

		Rect lerpPositionYField = new Rect(original);
		lerpPositionYField.xMin += (width - (width * 0.48f));
		lerpPositionYField.xMax -= (width - (width * 0.75f));
		lerpPositionYField.yMin += 55 + ADJUSTABLE_HEIGHT_MIN;
		lerpPositionYField.yMax -= 90 + ADJUSTABLE_HEIGHT_MAX;

		Rect lerpPositionZField = new Rect(original);
		lerpPositionZField.xMin += (width - (width * 0.275f));
		lerpPositionZField.xMax -= (width - (width * 0.96f));
		lerpPositionZField.yMin += 55 + ADJUSTABLE_HEIGHT_MIN;
		lerpPositionZField.yMax -= 90 + ADJUSTABLE_HEIGHT_MAX;

		Rect lerpRotationField = new Rect(original);
		lerpRotationField.xMin += 10;
		lerpRotationField.xMax -= (width - (width * 0.35f));
		lerpRotationField.yMin += 95 + ADJUSTABLE_HEIGHT_MIN - 10;
		lerpRotationField.yMax -= 50 + ADJUSTABLE_HEIGHT_MAX + 10;

		Rect lerpRotationXField = new Rect(original);
		lerpRotationXField.xMin += (width - (width * 0.68f));
		lerpRotationXField.xMax -= (width - (width * 0.55f));
		lerpRotationXField.yMin += 95 + ADJUSTABLE_HEIGHT_MIN - 10;
		lerpRotationXField.yMax -= 50 + ADJUSTABLE_HEIGHT_MAX + 10;

		Rect lerpRotationYField = new Rect(original);
		lerpRotationYField.xMin += (width - (width * 0.48f));
		lerpRotationYField.xMax -= (width - (width * 0.75f));
		lerpRotationYField.yMin += 95 + ADJUSTABLE_HEIGHT_MIN - 10;
		lerpRotationYField.yMax -= 50 + ADJUSTABLE_HEIGHT_MAX + 10;

		Rect lerpRotationZField = new Rect(original);
		lerpRotationZField.xMin += (width - (width * 0.275f));
		lerpRotationZField.xMax -= (width - (width * 0.96f));
		lerpRotationZField.yMin += 95 + ADJUSTABLE_HEIGHT_MIN - 10;
		lerpRotationZField.yMax -= 50 + ADJUSTABLE_HEIGHT_MAX + 10;

		Rect lerpScaleField = new Rect(original);
		lerpScaleField.xMin += 10;
		lerpScaleField.xMax -= (width - (width * 0.35f));
		lerpScaleField.yMin += 135 + ADJUSTABLE_HEIGHT_MIN - 20;
		lerpScaleField.yMax -= 10 + ADJUSTABLE_HEIGHT_MAX + 20;

		Rect lerpScaleXField = new Rect(original);
		lerpScaleXField.xMin += (width - (width * 0.68f));
		lerpScaleXField.xMax -= (width - (width * 0.55f));
		lerpScaleXField.yMin += 135 + ADJUSTABLE_HEIGHT_MIN - 20;
		lerpScaleXField.yMax -= 10 + ADJUSTABLE_HEIGHT_MAX + 20;

		Rect lerpScaleYField = new Rect(original);
		lerpScaleYField.xMin += (width - (width * 0.48f));
		lerpScaleYField.xMax -= (width - (width * 0.75f));
		lerpScaleYField.yMin += 135 + ADJUSTABLE_HEIGHT_MIN - 20;
		lerpScaleYField.yMax -= 10 + ADJUSTABLE_HEIGHT_MAX + 20;

		Rect lerpScaleZField = new Rect(original);
		lerpScaleZField.xMin += (width - (width * 0.275f));
		lerpScaleZField.xMax -= (width - (width * 0.96f));
		lerpScaleZField.yMin += 135 + ADJUSTABLE_HEIGHT_MIN - 20;
		lerpScaleZField.yMax -= 10 + ADJUSTABLE_HEIGHT_MAX + 20;

		Rect iconField = new Rect(original);
		iconField.xMin += 10;
		iconField.xMax -= (width - 60);
		iconField.yMin += 5;
		iconField.yMax -= 35;

		if (windowEvent.isMouse && _buttonRect.Contains(windowEvent.mousePosition))
		{
			if (windowEvent.type == EventType.MouseDown)
			{
				if (_buttonDown != null)
				{
					_activeTex = _buttonDown;
					_buttonContent.image = _buttonUp;
				}
			}
			else if (windowEvent.type == EventType.MouseUp)
			{
				if (_buttonUp != null)
				{
					_activeTex = _buttonUp;
					_buttonContent.image = _buttonUp;
				}

				if (_submitCallback != null)
					_submitCallback();
			}
		}
		else if (windowEvent.isMouse)
		{
			if (_buttonUp != null)
			{
				_activeTex = _buttonUp;
				_buttonContent.image = _buttonUp;
			}
		}

		GUI.DrawTexture(_buttonRect, _activeTex, ScaleMode.StretchToFill);

		if (_buttonIcon != null)
			GUI.DrawTexture(iconField, _buttonIcon, ScaleMode.StretchToFill);

		if (fontStyle != null)
			GUI.Label(_buttonRect, _buttonTitle, fontStyle);

		if (_submitButtonVisibile)
		{
			Rect submitField = new Rect(original);
			submitField.xMin += 10 + _submitButtonWidth;
			submitField.xMax -= 10 - _submitButtonWidth;
			submitField.yMin += 20 + _submitButtonHeight;
			submitField.yMax -= 125 - _submitButtonHeight - 15;

			_submitButtonTitle = _easyControlsButtonVisible ? "Lock" : "Modify";

			GUI.color = _easyControlsButtonVisible ? Color.green : Color.gray;

			if (GUI.Button(submitField, _submitButtonTitle))
			{
				_easyControlsButtonVisible = !_easyControlsButtonVisible;
				_submitButtonTitle = _easyControlsButtonVisible ? "Lock" : "Modify";

				if (_submitButtonAction != null)
					_submitButtonAction();
			}
			GUI.color = Color.white;

			GUI.enabled = _easyControlsButtonVisible;

			#region Lerp Position GUI
			GUI.color = LerpPosition ? Color.white : Color.gray;

			if (GUI.Button(lerpPositionField, "Lerp Position"))
			{
				LerpPosition = !LerpPosition;
			}

			//GUI.enabled = LerpPosition && _easyControlsButtonVisible;

			GUI.color = _lerpPosX ? Color.white : Color.gray;
			if (GUI.Button(lerpPositionXField, "X"))
			{
				_lerpPosX = !_lerpPosX;
			}
			GUI.color = Color.white;

			GUI.color = _lerpPosY ? Color.white : Color.gray;
			if (GUI.Button(lerpPositionYField, "Y"))
			{
				_lerpPosY = !_lerpPosY;
			}
			GUI.color = Color.white;

			GUI.color = _lerpPosZ ? Color.white : Color.gray;
			if (GUI.Button(lerpPositionZField, "Z"))
			{
				_lerpPosZ = !_lerpPosZ;
			}

			SerializePosition = GetSerializedVector3FromBools(_lerpPosX, _lerpPosY, _lerpPosZ);

			GUI.enabled = _easyControlsButtonVisible;
			#endregion

			#region Lerp Rotation GUI
			GUI.color = LerpRotation ? Color.white : Color.gray;

			if (GUI.Button(lerpRotationField, "Lerp Rotation"))
			{
				LerpRotation = !LerpRotation;
			}

			//GUI.enabled = LerpRotation && _easyControlsButtonVisible;

			GUI.color = _lerpRotX ? Color.white : Color.gray;
			if (GUI.Button(lerpRotationXField, "X"))
			{
				_lerpRotX = !_lerpRotX;
			}
			GUI.color = Color.white;

			GUI.color = _lerpRotY ? Color.white : Color.gray;
			if (GUI.Button(lerpRotationYField, "Y"))
			{
				_lerpRotY = !_lerpRotY;
			}
			GUI.color = Color.white;

			GUI.color = _lerpRotZ ? Color.white : Color.gray;
			if (GUI.Button(lerpRotationZField, "Z"))
			{
				_lerpRotZ = !_lerpRotZ;
			}

			SerializeRotation = GetSerializedVector3FromBools(_lerpRotX, _lerpRotY, _lerpRotZ);

			GUI.enabled = _easyControlsButtonVisible;
			#endregion

			#region Lerp Scale GUI
			GUI.color = LerpScale ? Color.white : Color.gray;

			if (GUI.Button(lerpScaleField, "Lerp Scale"))
			{
				LerpScale = !LerpScale;
			}

			//GUI.enabled = LerpScale && _easyControlsButtonVisible;

			GUI.color = _lerpScaleX ? Color.white : Color.gray;
			if (GUI.Button(lerpScaleXField, "X"))
			{
				_lerpScaleX = !_lerpScaleX;
			}
			GUI.color = Color.white;

			GUI.color = _lerpScaleY ? Color.white : Color.gray;
			if (GUI.Button(lerpScaleYField, "Y"))
			{
				_lerpScaleY = !_lerpScaleY;
			}
			GUI.color = Color.white;

			GUI.color = _lerpScaleZ ? Color.white : Color.gray;
			if (GUI.Button(lerpScaleZField, "Z"))
			{
				_lerpScaleZ = !_lerpScaleZ;
			}

			SerializeScale = GetSerializedVector3FromBools(_lerpScaleX, _lerpScaleY, _lerpScaleZ);

			GUI.enabled = _easyControlsButtonVisible;
			#endregion

			GUI.color = Color.white;

			GUI.enabled = true;
		}

		if (_prevTex != _activeTex)
		{
			_prevTex = _activeTex;
			window.Repaint();
		}
	}

	private NetworkedMonoBehavior.SerializeVector3Properties GetSerializedVector3FromBools(bool x, bool y, bool z)
	{
		if (x && y && z)
			return NetworkedMonoBehavior.SerializeVector3Properties.XYZ;
		if (x && y)
			return NetworkedMonoBehavior.SerializeVector3Properties.XY;
		if (x && z)
			return NetworkedMonoBehavior.SerializeVector3Properties.XZ;
		if (y && z)
			return NetworkedMonoBehavior.SerializeVector3Properties.YZ;
		if (x)
			return NetworkedMonoBehavior.SerializeVector3Properties.X;
		if (y)
			return NetworkedMonoBehavior.SerializeVector3Properties.Y;
		if (z)
			return NetworkedMonoBehavior.SerializeVector3Properties.Z;

		return NetworkedMonoBehavior.SerializeVector3Properties.None;
	}

	private void SetSerializedVector3ToBools(NetworkedMonoBehavior.SerializeVector3Properties vec3Prop, ref bool x, ref bool y, ref bool z)
	{
		switch (vec3Prop)
		{
			case NetworkedMonoBehavior.SerializeVector3Properties.XYZ:
				x = true;
				y = true;
				z = true;
				break;
			case NetworkedMonoBehavior.SerializeVector3Properties.XY:
				x = true;
				y = true;
				break;
			case NetworkedMonoBehavior.SerializeVector3Properties.XZ:
				x = true;
				z = true;
				break;
			case NetworkedMonoBehavior.SerializeVector3Properties.YZ:
				y = true;
				z = true;
				break;
			case NetworkedMonoBehavior.SerializeVector3Properties.X:
				x = true;
				break;
			case NetworkedMonoBehavior.SerializeVector3Properties.Y:
				y = true;
				break;
			case NetworkedMonoBehavior.SerializeVector3Properties.Z:
				z = true;
				break;
		}
	}
}

public class MiscellaneousButton : ForgeEditorDisplayButton
{
	#region UDP Reliable
	private bool _udpReliable = false;
	private bool _udpReliablePrev = false;

	public bool UDPReliable
	{
		get { return _udpReliable; }
		set
		{
			if (value != _udpReliablePrev)
			{
				_udpReliable = value;
				_udpReliablePrev = value;

				if (_udpReliableChanged != null)
					_udpReliableChanged(value);
			}
		}
	}
	private System.Action<bool> _udpReliableChanged = null;
	#endregion

	#region Is Player
	private bool _isPlayer = false;
	private bool _isPlayerPrev = false;

	public bool IsPlayer
	{
		get { return _isPlayer; }
		set
		{
			if (value != _isPlayerPrev)
			{
				_isPlayer = value;
				_isPlayerPrev = value;

				if (_isPlayerChanged != null)
					_isPlayerChanged(value);
			}
		}
	}
	private System.Action<bool> _isPlayerChanged = null;
	#endregion

	#region Destroy On Disconnect
	private bool _destroyOnDisconnect = false;
	private bool _destroyOnDisconnectPrev = false;

	public bool DestroyOnDisconnect
	{
		get { return _destroyOnDisconnect; }
		set
		{
			if (value != _destroyOnDisconnectPrev)
			{
				_destroyOnDisconnect = value;
				_destroyOnDisconnectPrev = value;

				if (_destroyOnDisconnectChanged != null)
					_destroyOnDisconnectChanged(value);
			}
		}
	}
	private System.Action<bool> _destroyOnDisconnectChanged = null;
	#endregion

	#region Teleport to Initial Position
	private bool _teleportToInitialPositions = false;
	private bool _teleportToInitialPositionsPrev = false;

	public bool TeleportToInitialPositions
	{
		get { return _teleportToInitialPositions; }
		set
		{
			if (value != _teleportToInitialPositionsPrev)
			{
				_teleportToInitialPositions = value;
				_teleportToInitialPositionsPrev = value;

				if (_teleportToInitialPositionsChanged != null)
					_teleportToInitialPositionsChanged(value);
			}
		}
	}
	private System.Action<bool> _teleportToInitialPositionsChanged = null;
	#endregion

	private string _toolTiptext;

	public MiscellaneousButton(ForgeButtonStyle buttonStyle, ForgeSubmitButtonStyle submitStyle = null)
		: base(buttonStyle, submitStyle)
	{
	}

	public void Initialize(string tooltipText, 
		bool udpReliable, 
		bool isPlayer, 
		bool destroyOnDisconnect, 
		bool teleportToInitialPosition,
		System.Action<bool> udpReliableChanged,
		System.Action<bool> isPlayerChanged,
		System.Action<bool> destroyOnDisconnectChanged,
		System.Action<bool> teleportToInitialPositionsChanged)
	{
		_toolTiptext = tooltipText;
		UDPReliable = udpReliable;
		IsPlayer = isPlayer;
		DestroyOnDisconnect = destroyOnDisconnect;
		TeleportToInitialPositions = teleportToInitialPosition;

		_udpReliableChanged = udpReliableChanged;
		_isPlayerChanged = isPlayerChanged;
		_destroyOnDisconnectChanged = destroyOnDisconnectChanged;
		_teleportToInitialPositionsChanged = teleportToInitialPositionsChanged;
	}

	public override void Update(Event windowEvent, GUIStyle fontStyle, Editor window, int width, int height)
	{
		if (_activeTex == null)
			return;

		_buttonRect = GUILayoutUtility.GetRect(_buttonContent, _buttonStyle, GUILayout.Height(_buttonHeight));
		Rect iconField = new Rect(_buttonRect);
		iconField.xMin += 10;
		iconField.xMax -= (width - 60);
		iconField.yMin += 5;
		iconField.yMax -= 35;

		Rect udpReliableField = new Rect(_buttonRect);
		udpReliableField.xMin += 10;
		udpReliableField.xMax -= 10;
		udpReliableField.yMin += 60;
		udpReliableField.yMax -= 115;

		Rect isPlayerField = new Rect(_buttonRect);
		isPlayerField.xMin += 10;
		isPlayerField.xMax -= 10;
		isPlayerField.yMin += 92;
		isPlayerField.yMax -= 85;

		Rect destroyOnDisconnectField = new Rect(_buttonRect);
		destroyOnDisconnectField.xMin += 10;
		destroyOnDisconnectField.xMax -= 10;
		destroyOnDisconnectField.yMin += 125;
		destroyOnDisconnectField.yMax -= 50;

		Rect teleportToInitialPositionField = new Rect(_buttonRect);
		teleportToInitialPositionField.xMin += 10;
		teleportToInitialPositionField.xMax -= 10;
		teleportToInitialPositionField.yMin += 160;
		teleportToInitialPositionField.yMax -= 15;

		if (string.IsNullOrEmpty(_toolTiptext))
			_buttonRect.yMax -= 50;

		if (windowEvent.isMouse && _buttonRect.Contains(windowEvent.mousePosition))
		{
			if (windowEvent.type == EventType.MouseDown)
			{
				if (_buttonDown != null)
				{
					_activeTex = _buttonDown;
					_buttonContent.image = _buttonUp;
				}
			}
			else if (windowEvent.type == EventType.MouseUp)
			{
				if (_buttonUp != null)
				{
					_activeTex = _buttonUp;
					_buttonContent.image = _buttonUp;
				}

				if (_submitCallback != null)
					_submitCallback();
			}
		}
		else if (windowEvent.isMouse)
		{
			if (_buttonUp != null)
			{
				_activeTex = _buttonUp;
				_buttonContent.image = _buttonUp;
			}
		}

		GUI.DrawTexture(_buttonRect, _activeTex, ScaleMode.StretchToFill);

		if (_buttonIcon != null)
			GUI.DrawTexture(iconField, _buttonIcon, ScaleMode.StretchToFill);

		if (fontStyle != null)
			GUI.Label(_buttonRect, _buttonTitle, fontStyle);

		if (!string.IsNullOrEmpty(_toolTiptext))
		{
			Rect textField = new Rect(_buttonRect);
			textField.xMin += 10;
			textField.xMax -= 10;
			textField.yMin += 20;
			textField.yMax -= 150;

			GUI.color = Color.yellow;
			GUI.TextArea(textField, _toolTiptext);
			GUI.color = Color.white;
		}

		GUI.color = UDPReliable ? Color.white : Color.gray;

		if (GUI.Button(udpReliableField, "UDP Reliable"))
		{
			UDPReliable = !UDPReliable;
		}

		GUI.color = Color.white;

		GUI.color = IsPlayer ? Color.white : Color.gray;

		if (GUI.Button(isPlayerField, "Is Player"))
		{
			IsPlayer = !IsPlayer;
		}

		GUI.color = Color.white;

		GUI.color = DestroyOnDisconnect ? Color.white : Color.gray;

		if (GUI.Button(destroyOnDisconnectField, "Destroy On Disconnect"))
		{
			DestroyOnDisconnect = !DestroyOnDisconnect;
		}

		GUI.color = Color.white;

		GUI.color = TeleportToInitialPositions ? Color.white : Color.gray;

		if (GUI.Button(teleportToInitialPositionField, "Teleport To Initial Position"))
		{
			TeleportToInitialPositions = !TeleportToInitialPositions;
		}

		GUI.color = Color.white;

		if (_prevTex != _activeTex)
		{
			_prevTex = _activeTex;
			window.Repaint();
		}
	}
}