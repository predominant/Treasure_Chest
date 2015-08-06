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



//using System;

//namespace UnityEngine
//{
//	public class HideInInspector : Attribute { }

//	public class MonoBehaviour
//	{
//		public Transform transform = new Transform();
//		public static object Instantiate(GameObject a) { return null; }
//		public static object Instantiate(GameObject a, Vector3 pos, Quaternion rot) { return null; }
//		public GameObject gameObject = null;
//		public static void Destroy(GameObject obj) { }
//	}

//	public class Vector2
//	{
//		public float x, y;
//		public Vector2(float x, float y) { this.x = x; this.y = y; }
//		public static Vector2 Lerp(Vector2 a, Vector2 b, float t) { return new Vector2(0, 0); }
//		public static float Distance(Vector2 a, Vector2 b) { return 0.0f; }
//	}

//	public class Vector3
//	{
//		public float x, y, z;
//		public Vector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
//		public static Vector3 Lerp(Vector3 a, Vector3 b, float t) { return new Vector3(0, 0, 0); }
//		public static float Distance(Vector3 a, Vector3 b) { return 0.0f; }
//	}

//	public class Vector4
//	{
//		public float x, y, z, w;
//		public Vector4(float x, float y, float z, float w) { this.x = x; this.y = y; this.z = z; this.w = w; }
//		public static Vector4 Lerp(Vector4 a, Vector4 b, float t) { return new Vector4(0, 0, 0, 0); }
//		public static float Distance(Vector4 a, Vector4 b) { return 0.0f; }
//	}

//	public class Color
//	{
//		public float r, g, b, a;
//		public Color(float r, float g, float b, float a) { this.r = r; this.g = g; this.b = b; this.a = a; }
//	}

//	public class Quaternion
//	{
//		public float x, y, z, w;
//		public Quaternion(float x, float y, float z, float w) { this.x = x; this.y = y; this.z = z; this.w = w; }
//		public static Quaternion Slerp(Quaternion a, Quaternion b, float t) { return new Quaternion(0, 0, 0, 0); }
//		public static float Angle(Quaternion a, Quaternion b) { return 0.0f; }
//		public static Quaternion Euler(Vector3 e) { return new Quaternion(0, 0, 0, 0); }
//	}

//	public class GameObject
//	{
//		public string name = string.Empty;
//		public static object Instantiate(GameObject a) { return null; }
//		public static object Instantiate(GameObject a, Vector3 pos, Quaternion rot) { return null; }
//		public T GetComponent<T>() { return (T)new Object(); }
//		public static void Destroy(GameObject obj) { }
//	}

//	public class Transform
//	{
//		public Vector3 position { get; set; }
//		public Quaternion rotation { get; set; }
//		public Vector3 localScale { get; set; }
//	}

//	public class Time
//	{
//		public static float deltaTime = 0.0f;
//	}

//	public class Resources
//	{
//		public static T Load<T>(string name) { return (T)new Object(); }
//	}

//	public class Debug
//	{
//		public static void Log(string message) { }
//		public static void LogError(string message) { }
//	}

//	public class Application
//	{
//		public static void LoadLevel(int i) { }
//		public static void LoadLevel(string name) { }
//		public static int loadedLevel = 0;
//		public static bool isLoadingLevel = false;
//		public static void Quit() { }
//	}

//	public class Ping
//	{
//		public int time = 0;
//		public Ping(string host) { }
//	}
//}