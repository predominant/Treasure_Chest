// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2013-08-05</date>

using UnityEngine;
using System.Collections;

public class KGFCutscene : MonoBehaviour,KGFIValidator
{
	#region cutscene properties
	public float itsSize = 0.2f;
	public float itsFadeTime = 0.5f;
	public Color itsColor = Color.black;
	public bool itsUseAlphaFade = true;
	
	public Camera itsCamera;
	
	public Shader itsShader;
	#endregion
	
	#region internal variables
	
	float itsCurrentPercent = 0;
	float itsCurrentPercentDest = 0;
	float itsCurrentVelocity = 0;
	
	Mesh itsMesh;
	Material itsMaterial;
	GameObject itsGameObject;
	Transform itsGameObjectTransform;
	
	bool itsUpdate = false;
	#endregion
	
	#region public interface
	/// <summary>
	/// Start cutscene
	/// </summary>
	public void StartCutscene()
	{
		itsCurrentPercentDest = 1;
		itsUpdate = true;
	}
	
	/// <summary>
	/// Stop cutscene
	/// </summary>
	public void StopCutscene()
	{
		itsCurrentPercentDest = 0;
		itsUpdate = true;
	}
	
	/// <summary>
	/// Change color of the cutscene bars
	/// </summary>
	public void SetColor(Color theColor)
	{
		itsColor = theColor;
		itsMaterial.color = new Color(itsColor.r,itsColor.g,itsColor.b,itsUseAlphaFade?itsCurrentPercent:1);
	}
	#endregion
	
	#region unity stuff
	void Awake()
	{
		InitStuff();
	}
	
	void LateUpdate()
	{
		if(itsUpdate)
		{
			UpdatePlanes();
		}
	}
	#endregion
	
	#region internal
	void InitStuff()
	{
		itsMaterial = new Material(itsShader);
		itsMaterial.color = new Color(itsColor.r,itsColor.g,itsColor.b,itsUseAlphaFade?itsCurrentPercent:1);
		
		itsGameObject = new GameObject("CutSceneBlackBars");
		itsGameObjectTransform = itsGameObject.transform;
		itsGameObjectTransform.parent = itsCamera.transform;
		itsGameObjectTransform.localPosition = Vector3.zero;
		itsGameObjectTransform.localRotation = Quaternion.identity;
		
		itsGameObject.AddComponent<MeshRenderer>().sharedMaterial = itsMaterial;
		itsGameObject.AddComponent<MeshFilter>().mesh = GeneratePlaneMeshXZ();
		itsMesh = itsGameObject.GetComponent<MeshFilter>().mesh;
	}
	
	void UpdatePlanes()
	{
		itsCurrentPercent = Mathf.SmoothDamp(itsCurrentPercent,itsCurrentPercentDest,ref itsCurrentVelocity,itsFadeTime);
		if (Mathf.Abs(itsCurrentPercent - itsCurrentPercentDest) < 0.01f)
		{
			itsCurrentPercent = itsCurrentPercentDest;
			itsUpdate = false;
		}
		
		if(itsCamera != null)
		{
			float aHeight = itsCurrentPercent * itsSize * Screen.height;
			
			Vector3[] aVertices = itsMesh.vertices;
			aVertices[0] = itsCamera.ScreenToWorldPoint(new Vector3(-1,-1,itsCamera.nearClipPlane+0.01f));
			aVertices[0] = itsGameObjectTransform.InverseTransformPoint(aVertices[0]);
			aVertices[1] = itsCamera.ScreenToWorldPoint(new Vector3(Screen.width+1,-1,itsCamera.nearClipPlane+0.01f));
			aVertices[1] = itsGameObjectTransform.InverseTransformPoint(aVertices[1]);
			aVertices[2] = itsCamera.ScreenToWorldPoint(new Vector3(Screen.width+1,aHeight,itsCamera.nearClipPlane+0.01f));
			aVertices[2] = itsGameObjectTransform.InverseTransformPoint(aVertices[2]);
			aVertices[3] = itsCamera.ScreenToWorldPoint(new Vector3(-1,aHeight,itsCamera.nearClipPlane+0.01f));
			aVertices[3] = itsGameObjectTransform.InverseTransformPoint(aVertices[3]);
			
			aVertices[4] = itsCamera.ScreenToWorldPoint(new Vector3(-1,Screen.height - aHeight,itsCamera.nearClipPlane+0.01f));
			aVertices[4] = itsGameObjectTransform.InverseTransformPoint(aVertices[4]);
			aVertices[5] = itsCamera.ScreenToWorldPoint(new Vector3(Screen.width+1,Screen.height - aHeight,itsCamera.nearClipPlane+0.01f));
			aVertices[5] = itsGameObjectTransform.InverseTransformPoint(aVertices[5]);
			aVertices[6] = itsCamera.ScreenToWorldPoint(new Vector3(Screen.width+1,Screen.height+1,itsCamera.nearClipPlane+0.01f));
			aVertices[6] = itsGameObjectTransform.InverseTransformPoint(aVertices[6]);
			aVertices[7] = itsCamera.ScreenToWorldPoint(new Vector3(-1,Screen.height+1,itsCamera.nearClipPlane+0.01f));
			aVertices[7] = itsGameObjectTransform.InverseTransformPoint(aVertices[7]);
			itsMesh.vertices = aVertices;
			
			itsMesh.RecalculateBounds();
			
			Vector3 aNormal = itsCamera.transform.forward;
			Vector3[] aNormals = itsMesh.normals;
			aNormals[0] = aNormal;
			aNormals[1] = aNormal;
			aNormals[2] = aNormal;
			aNormals[3] = aNormal;
			aNormals[4] = aNormal;
			aNormals[5] = aNormal;
			aNormals[6] = aNormal;
			aNormals[7] = aNormal;
			itsMesh.normals = aNormals;
			
			if (itsUseAlphaFade)
			{
				itsMaterial.color = new Color(itsColor.r,itsColor.g,itsColor.b,itsCurrentPercent);
			}
		}
	}
	
	/// <summary>
	/// Generate a new mesh that has 2 planes.
	/// </summary>
	private Mesh GeneratePlaneMeshXZ()
	{
		Mesh aMesh = new Mesh();
		
		Vector3[] aVertices = new Vector3[8];
		aVertices[0] = new Vector3(0.0f,0.0f,0.0f);
		aVertices[1] = new Vector3(1.0f,0.0f,0.0f);
		aVertices[2] = new Vector3(1.0f,0.0f,1.0f);
		aVertices[3] = new Vector3(0.0f,0.0f,1.0f);
		
		aVertices[4] = new Vector3(0.0f,0.0f,0.0f);
		aVertices[5] = new Vector3(1.0f,0.0f,0.0f);
		aVertices[6] = new Vector3(1.0f,0.0f,1.0f);
		aVertices[7] = new Vector3(0.0f,0.0f,1.0f);
		
		Vector3[] aNormals = new Vector3[8];
		aNormals[0] = new Vector3(0.0f,1.0f,0.0f);
		aNormals[1] = new Vector3(0.0f,1.0f,0.0f);
		aNormals[2] = new Vector3(0.0f,1.0f,0.0f);
		aNormals[3] = new Vector3(0.0f,1.0f,0.0f);
		
		aNormals[4] = new Vector3(0.0f,1.0f,0.0f);
		aNormals[5] = new Vector3(0.0f,1.0f,0.0f);
		aNormals[6] = new Vector3(0.0f,1.0f,0.0f);
		aNormals[7] = new Vector3(0.0f,1.0f,0.0f);
		
		Vector2[] anUVs = new Vector2[8];
		anUVs[0] = new Vector2(0.0f,0.0f);
		anUVs[1] = new Vector2(1.0f,0.0f);
		anUVs[2] = new Vector2(1.0f,1.0f);
		anUVs[3] = new Vector2(0.0f,1.0f);
		
		anUVs[4] = new Vector2(0.0f,0.0f);
		anUVs[5] = new Vector2(1.0f,0.0f);
		anUVs[6] = new Vector2(1.0f,1.0f);
		anUVs[7] = new Vector2(0.0f,1.0f);
		
		
		int[] aTriangles = new int[12];
		aTriangles[0] = 0;
		aTriangles[1] = 3;
		aTriangles[2] = 2;
		aTriangles[3] = 0;
		aTriangles[4] = 2;
		aTriangles[5] = 1;
		
		aTriangles[6] = 4;
		aTriangles[7] = 7;
		aTriangles[8] = 6;
		aTriangles[9] = 4;
		aTriangles[10] = 6;
		aTriangles[11] = 5;
		
		aMesh.vertices = aVertices;
		aMesh.normals = aNormals;
		aMesh.uv = anUVs;
		aMesh.triangles = aTriangles;
		
		return aMesh;
	}
	#endregion
	
	#region KGFIValidator
	public KGFMessageList Validate()
	{
		KGFMessageList aList = new KGFMessageList();
		
		if (itsCamera == null)
		{
			aList.AddError("KGFCutscene can only be used in combination with a camera");
		}
		
		return aList;
	}
	#endregion
}