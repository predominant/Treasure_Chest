//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//
using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	/**
		This example shows how you can use a grid in 3D.
	*/
	[ExecuteInEditMode]
	public class RectWorld : GridBehaviour<RectPoint>
	{
		public Texture2D heightMap;
		public Gradient heightGradient = new Gradient()
		{
			alphaKeys = new GradientAlphaKey[0],
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(ExampleUtils.Colors[0], 0),
 				new GradientColorKey(ExampleUtils.Colors[2], 1),
			}
		};

		public float heightMultiplier = 4;

		override public void InitGrid()
		{
			var imageRect = new Rect(0, 0, heightMap.width, heightMap.height);
			var map = new RectMap(new Vector2(40, 40));
			var map2D = new ImageMap<RectPoint>(imageRect, Grid, map);

			foreach (var point in Grid)
			{
				int x = Mathf.FloorToInt(map2D[point].x);
				int y = Mathf.FloorToInt(map2D[point].y);
				float height = heightMap.GetPixel(x, y).r;

				if (height <= 0)
				{
					height = 0.01f;
				}

				var block = Grid[point];

				if (block == null) Debug.LogWarning("block is null " + point);
				else
				{
					block.Color = heightGradient.Evaluate(height);
					block.transform.localScale = new Vector3(1, height * heightMultiplier, 1);
				}
			}
		}
	}
}