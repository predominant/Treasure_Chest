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



using System;
using System.Collections.Generic;
using System.Threading;
using BeardedManStudios.Network;
using UnityEditor;
using UnityEngine;
using System.Collections;

public class ForgeNetworkingEditor : EditorWindow
{
	private static ForgeNetworkingEditor Instance;
	private Texture2D ForgeIcon;
	private Vector2 ScrollPos;
	private GUIStyle boldWhite = new GUIStyle();
	private GUIStyle regularWhite = new GUIStyle();
	private List<ForgeEditorDisplayObject> News = new List<ForgeEditorDisplayObject>();
	private List<ForgeEditorDisplayObject> Videos = new List<ForgeEditorDisplayObject>();
	private List<ForgeEditorDisplayObject> Store = new List<ForgeEditorDisplayObject>();
	private List<ForgeEditorDisplayObject> Documentation = new List<ForgeEditorDisplayObject>();
	private List<ForgeEditorDisplayObject> About = new List<ForgeEditorDisplayObject>();
	private ForgeEditorNav currentNav = ForgeEditorNav.Home;

	private const string NEWS_ENDPOINT = "http://www.beardedmanstudios.com/forge/news.php";
	private const string VIDEOS_ENDPOINT = "http://www.beardedmanstudios.com/forge/videos.php";
	private const string STORE_ENDPOINT = "http://www.beardedmanstudios.com/forge/store.php";
	private const string YOUTUBE_THUMBNAIL_ENDPOINT = "http://img.youtube.com/vi/<id>/0.jpg";
	private const string YOUTUBE_URL = "https://www.youtube.com/watch?v=<id>";

	private enum ForgeEditorNav
	{
		Home = 0,
		Store,
		Videos,
		Documentation,
		About
	};

	protected class ForgeEditorDisplayObject
	{
		public string DisplayInfo;
		public DisplayType DisplayInfoType;
		public Texture2D DisplayImage;
		private readonly GUIStyle boldWhite;
		private readonly GUIStyle regularWhite;
		private string imageID;
		private string imageUrl;
		public byte[] storedBytes;

		public enum DisplayType
		{
			Header = 0,
			Paragraph,
			Video,
			AssetImage
		};

		public ForgeEditorDisplayObject(string info, DisplayType type, GUIStyle bold, GUIStyle regular)
		{
			DisplayInfoType = type;
			boldWhite = bold;
			regularWhite = regular;

			switch (DisplayInfoType)
			{
				case DisplayType.Header:
					DisplayInfo = info.Replace("<h1>", string.Empty).Replace("</h1>", string.Empty);
					break;
				case DisplayType.Paragraph:
					DisplayInfo = info.Replace("<p>", string.Empty).Replace("</p>", string.Empty);
					break;
				case DisplayType.Video:
					string videoTitle = info.Remove(0, "<video title=\"".Length);
					videoTitle = videoTitle.Remove(videoTitle.IndexOf("\">"), videoTitle.Length - videoTitle.IndexOf("\">"));
					string videoID = info.Remove(0, info.IndexOf("\">") + "\">".Length);
					videoID = videoID.Remove(videoID.IndexOf("</video>"), videoID.Length - videoID.IndexOf("</video>"));
					DisplayInfo = videoTitle;
					imageID = videoID;
					imageUrl = YOUTUBE_URL.Replace("<id>", videoID);
					HTTP imageRequest = new HTTP(YOUTUBE_THUMBNAIL_ENDPOINT.Replace("<id>", imageID));
					imageRequest.GetImage(RetreiveImageResponse);
					break;
				case DisplayType.AssetImage:
					string assetImage = info.Remove(0, "<asset image=\"".Length);
					assetImage = assetImage.Remove(assetImage.IndexOf("\">"), assetImage.Length - assetImage.IndexOf("\">"));
					string assetURL = info.Remove(0, info.IndexOf("\">") + "\">".Length);
					assetURL = assetURL.Remove(assetURL.IndexOf("</asset>"), assetURL.Length - assetURL.IndexOf("</asset>"));
					imageUrl = assetURL;
					//Debug.Log("Attempting to load: " + assetImage);
					HTTP assetRequest = new HTTP(assetImage);
					assetRequest.GetImage(RetreiveImageAssetResponse);
					break;
			}
		}

		private void RetreiveImageResponse(object data)
		{
			storedBytes = (byte[])data;
		}

		private void RetreiveImageAssetResponse(object data)
		{
			storedBytes = (byte[])data;
		}

		public bool ImageReady()
		{
			return storedBytes != null && storedBytes.Length > 0;
		}

		public void LoadImage()
		{
			try
			{
				DisplayImage = new Texture2D(1, 1);
				DisplayImage.LoadImage(storedBytes);
				DisplayImage.Apply();
				storedBytes = null;
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
			}
		}

		public void Display(Rect position)
		{
			switch (DisplayInfoType)
			{
				case DisplayType.Header:
					GetHeader();
					break;
				case DisplayType.Paragraph:
					GetParagraph();
					break;
				case DisplayType.Video:
					GetVideo(position);
					break;
				case DisplayType.AssetImage:
					GetAssetstore(position);
					break;
			}
		}

		public void GetHeader()
		{
			GUILayout.Space(Screen.height * 0.03f);
			GUILayout.TextArea(DisplayInfo, boldWhite);
		}

		public void GetParagraph()
		{
			GUILayout.TextArea(DisplayInfo, regularWhite);
		}

		public void GetVideo(Rect position)
		{
			GUILayout.TextArea(DisplayInfo, regularWhite);
			if (DisplayImage != null)
			{
				if (GUILayout.Button(DisplayImage))
				{
					Application.OpenURL(imageUrl);
				}
			}
			else
				GUILayout.TextArea("[Failed to load video]", regularWhite);
		}

		public void GetAssetstore(Rect position)
		{
			if (DisplayImage != null)
			{
				if (GUILayout.Button(DisplayImage, GUILayout.Width(position.width * 0.482f)))
				{
					Application.OpenURL(imageUrl);
				}
			}
			else
				GUILayout.TextArea("[Failed to load image]", regularWhite);
		}
	}

	[MenuItem("Window/Forge Networking/Forge Editor %G")]
	public static void Init()
	{
		if (Instance != null)
		{
			Instance.Close();
			Instance = null;
		}
		else
		{
			Instance = (ForgeNetworkingEditor) GetWindow(typeof (ForgeNetworkingEditor));
			Instance.Initialize();
		}
	}

	public void Initialize()
	{
#if UNITY_5_1
		titleContent.text = "Forge Editor";
#else
		title = "Forge Editor";
#endif

		minSize = new Vector2(400, 400);
		ForgeIcon = Resources.Load<Texture2D>("BMSLogo");
		ScrollPos = Vector2.zero;
		
		regularWhite.fontSize = 14;
		regularWhite.wordWrap = true;

		if (UnityEditorInternal.InternalEditorUtility.HasPro())
		{
			regularWhite.normal.textColor = Color.white;
			boldWhite.normal.textColor = Color.white;
		}

		boldWhite.fontSize = 14;
		boldWhite.wordWrap = true;
		boldWhite.fontStyle = FontStyle.Bold;
		News.Add(new ForgeEditorDisplayObject("Loading news...", ForgeEditorDisplayObject.DisplayType.Paragraph, boldWhite, regularWhite));
		Videos.Add(new ForgeEditorDisplayObject("Loading videos...", ForgeEditorDisplayObject.DisplayType.Paragraph, boldWhite, regularWhite));
		//Videos.Add(new ForgeEditorDisplayObject("Under Construction!", ForgeEditorDisplayObject.DisplayType.Header, boldWhite, regularWhite));		
		//Store.Add(new ForgeEditorDisplayObject("Loading store...", ForgeEditorDisplayObject.DisplayType.Paragraph, boldWhite, regularWhite));
		Documentation.Add(new ForgeEditorDisplayObject("Coming soon!", ForgeEditorDisplayObject.DisplayType.Header, boldWhite, regularWhite));
		//About.Add(new ForgeEditorDisplayObject("Bearded Man Studios, Inc.", ForgeEditorDisplayObject.DisplayType.Header, boldWhite, regularWhite));
		//About.Add(new ForgeEditorDisplayObject("Brent Farris, Brett Faulds", ForgeEditorDisplayObject.DisplayType.Paragraph, boldWhite, regularWhite));
		HTTP newsHttp = new HTTP(NEWS_ENDPOINT);
		newsHttp.Get(LayoutNews);
		HTTP videosHttp = new HTTP(VIDEOS_ENDPOINT);
		videosHttp.Get(LayoutVideos);
		//HTTP storeHttp = new HTTP(STORE_ENDPOINT);
		//storeHttp.Get(LayoutStore);
	}

	private void OnGUI()
	{
		GUILayout.BeginArea(new Rect(0, 0, position.width, position.height * 0.2f));
		//EditorGUI.DrawTextureAlpha(new Rect(0, 0, Screen.width, Screen.height * 0.2f), ForgeIcon);
		if (ForgeIcon != null)
		{
			EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height * 0.2f), Color.white);
			GUI.DrawTexture(new Rect(0, 0, position.width * 0.7f, position.height * 0.2f), ForgeIcon);
		}

		GUILayout.EndArea();
		GUILayout.BeginArea(new Rect(0, position.height * 0.2f, position.width, position.height * 0.05f));
		GUILayout.BeginHorizontal();

		if (GUILayout.Button("Home", GUILayout.Height(position.height * 0.05f)))
		{
			currentNav = ForgeEditorNav.Home;
		}

		//if (GUILayout.Button("Add-ons", GUILayout.Height(position.height * 0.05f)))
		//{
		//	currentNav = ForgeEditorNav.Store;
		//}

		if (GUILayout.Button("Videos", GUILayout.Height(position.height * 0.05f)))
		{
			currentNav = ForgeEditorNav.Videos;
		}

		if (GUILayout.Button("Documentation", GUILayout.Height(position.height * 0.05f)))
		{
			//Open url of the documentation
			Application.OpenURL("http://developers.forgearcade.com/Documentation");
			//currentNav = ForgeEditorNav.Documentation;
		}

		//if (GUILayout.Button("About", GUILayout.Height(position.height * 0.05f)))
		//{
		//	currentNav = ForgeEditorNav.About;
		//}

		GUILayout.EndHorizontal();
		GUILayout.EndArea();

		GUILayout.BeginArea(new Rect(0, position.height * 0.25f, position.width, position.height * 0.75f));
		ScrollPos = GUILayout.BeginScrollView(ScrollPos);

		switch (currentNav)
		{
			case ForgeEditorNav.Home:
				NewsNav();
				break;
			case ForgeEditorNav.Store:
				StoreNav();
				break;
			case ForgeEditorNav.Videos:
				VideosNav();
				break;
			case ForgeEditorNav.Documentation:
				DocumentationNav();
				break;
			case ForgeEditorNav.About:
				AboutNav();
				break;
		}

		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	private void NewsNav()
	{
		foreach (ForgeEditorDisplayObject nObj in News)
		{
			nObj.Display(position);
		}
	}

	private void StoreNav()
	{
		int counter = 0;

		GUILayout.BeginVertical();
		foreach (ForgeEditorDisplayObject nObj in Store)
		{
			if (nObj.DisplayInfoType == ForgeEditorDisplayObject.DisplayType.AssetImage)
				counter++;
			else
			{
				if (counter != 0)
					GUILayout.EndHorizontal();

				counter = 0;
			}

			if (counter % 2 == 1)
			{
				if (counter != 1)
					GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
			}

			if (nObj.ImageReady())
				nObj.LoadImage();

			nObj.Display(position);
		}

		if (counter != 0)
			GUILayout.EndHorizontal();

		GUILayout.EndVertical();
	}

	private void VideosNav()
	{
		foreach (ForgeEditorDisplayObject nObj in Videos)
		{
			if (nObj.ImageReady())
				nObj.LoadImage();
			nObj.Display(position);
		}
	}

	private void DocumentationNav()
	{
		foreach (ForgeEditorDisplayObject nObj in Documentation)
		{
			nObj.Display(position);
		}
	}

	private void AboutNav()
	{
		foreach (ForgeEditorDisplayObject nObj in About)
		{
			nObj.Display(position);
		}
	}

	private void LayoutNews(object data)
	{
		//string testString = "<h1>News:</h1><p>This is some news that is just going to be populated here in the whats it</p><h1>Other Stuff</h1><p>I don't know, I'm just typeing stuff now...</p><p>So, whatever!</p>";
		//data = testString;
		News.Clear();
		string[] headers = ((string)data).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

		foreach (string entry in headers)
		{
			if (entry.Contains("<h1>"))
			{
				News.Add(new ForgeEditorDisplayObject(entry, ForgeEditorDisplayObject.DisplayType.Header, boldWhite, regularWhite));
			}
			else if (entry.Contains("<p>"))
			{
				//.Replace("\n", string.Empty) at the end of entry if you want to remove newlines
				News.Add(new ForgeEditorDisplayObject(entry, ForgeEditorDisplayObject.DisplayType.Paragraph, boldWhite, regularWhite));
			}
		}
	}

	private void LayoutVideos(object data)
	{
		Videos.Clear();

		string[] headers = ((string)data).Split(new char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);

		foreach (string entry in headers)
		{
			//Debug.Log("entry: " + entry);
			if (entry.Contains("<video title=\"")) //Video
			{
				//.Replace("\n", string.Empty) at the end of entry if you want to remove newlines
				Videos.Add(new ForgeEditorDisplayObject(entry, ForgeEditorDisplayObject.DisplayType.Video, boldWhite, regularWhite));
			}
			else if (entry.Contains("<h1>")) //Header
			{
				Videos.Add(new ForgeEditorDisplayObject(entry, ForgeEditorDisplayObject.DisplayType.Header, boldWhite, regularWhite));
			}
		}
	}

	private void LayoutStore(object data)
	{
		Store.Clear();
		string[] headers = ((string)data).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

		foreach (string entry in headers)
		{
			//Debug.Log("entry: " + entry);
			if (entry.Contains("<asset image=\"")) //Video
			{
				//.Replace("\n", string.Empty) at the end of entry if you want to remove newlines
				Store.Add(new ForgeEditorDisplayObject(entry, ForgeEditorDisplayObject.DisplayType.AssetImage, boldWhite, regularWhite));
			}
			else if (entry.Contains("<h1>")) //Header
			{
				Store.Add(new ForgeEditorDisplayObject(entry, ForgeEditorDisplayObject.DisplayType.Header, boldWhite, regularWhite));
			}
		}
	}
}
