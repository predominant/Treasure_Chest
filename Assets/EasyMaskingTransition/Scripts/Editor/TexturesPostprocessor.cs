using UnityEngine;
using UnityEditor;
using System.Collections;

public class TexturesPostprocessor : AssetPostprocessor {
	
	void OnPreprocessTexture () {
		
		if (assetPath.Contains("EasyMaskingTransition/Textures/Gradations")) {
			
			TextureImporter textureImporter = assetImporter as TextureImporter;
			textureImporter.textureType = TextureImporterType.Advanced;
			textureImporter.grayscaleToAlpha = true;
			textureImporter.textureFormat = TextureImporterFormat.Alpha8;
		}
	}
}