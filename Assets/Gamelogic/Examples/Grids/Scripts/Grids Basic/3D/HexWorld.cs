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
	public class HexWorld : GridBehaviour<FlatHexPoint>
	{
		public Texture2D heightMap;

		override public void InitGrid()
		{
			var imageRect = new Rect(0, 0, heightMap.width, heightMap.height);
			var map = new FlatHexMap(new Vector2(80, 69));
			var map2D = new ImageMap<FlatHexPoint>(imageRect, Grid, map);

			foreach (var point in Grid)
			{
				int x = Mathf.FloorToInt(map2D[point].x);
				int y = Mathf.FloorToInt(map2D[point].y);
				float height = heightMap.GetPixel(x, y).r * 4;

				if (height <= 0)
				{
					height = 0.01f;
				}

				var block = Grid[point];

				if (block == null)
				{
					Debug.LogWarning("block is null " + point);
				}
				else
				{
					//block.Color = ExampleUtils.Blend(height, ExampleUtils.DefaultColors[0], ExampleUtils.DefaultColors[1]);
					block.transform.localScale = new Vector3(1, height, 1);
				}
			}
		}
	}
}