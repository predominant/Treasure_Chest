//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System;
using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	public static class ExampleUtils
	{
		public static Color ColorFromInt(int r, int g, int b)
		{
			return new Color(r/255.0f, g/255.0f, b/255.0f);
		}

		public static Color ColorFromInt(int r, int g, int b, int a)
		{
			return new Color(r/255.0f, g/255.0f, b/255.0f, a/255.0f);
		}

		public static Color[] Colors =
		{
			ColorFromInt(133, 219, 233),
			ColorFromInt(198, 224, 34),
			ColorFromInt(255, 215, 87),
			ColorFromInt(228, 120, 129),

			ColorFromInt(42, 192, 217),
			ColorFromInt(114, 197, 29),
			ColorFromInt(247, 188, 0),
			ColorFromInt(215, 55, 82),

			ColorFromInt(205, 240, 246),
			ColorFromInt(229, 242, 154),
			ColorFromInt(255, 241, 153),
			ColorFromInt(240, 182, 187),

			ColorFromInt(235, 249, 252),
			ColorFromInt(241, 249, 204),
			ColorFromInt(255, 252, 193),
			ColorFromInt(247, 222, 217),

			Color.black
		};

		public static readonly int ImageWidth = 1024;
		public static readonly int ImageHeight = 768;

		public static Rect ScreenRect
		{
			get
			{
				return new Rect(-Screen.width/2f, -Screen.height/2f, Screen.width, Screen.height);
			}
		}

		public static Vector2 ImagePointToWorldPoint(Vector2 imagePoint)
		{
			var x = -(imagePoint.x - ImageWidth/2.0f);
			var y = -(imagePoint.y - ImageHeight/2.0f);

			return new Vector2(x, y);
		}

		/**
		This function only works if a main camera has been set.
	*/

		[Obsolete("Use GridBuilder.ScreenToWorld instead")]
		public static Vector3 ScreenToWorld(Vector3 screenPosition)
		{
			return GridBuilderUtils.ScreenToWorld(screenPosition);
		}

		/**
		This function only works if a main camera has been set.
	*/

		[Obsolete("Use GridBuilderUtils.ScreenToWorld instead")]
		public static Vector3 ScreenToWorld(GameObject root, Vector3 screenPosition)
		{
			return GridBuilderUtils.ScreenToWorld(root, screenPosition);
		}

		public static void PaintScreenTexture<TPoint>(Texture2D texture, IMap<TPoint> map, Func<TPoint, int> colorFunction)
			where TPoint : IGridPoint<TPoint>
		{
			var pixels = texture.GetPixels();

			int width = texture.width;
			int height = texture.height;

			for (var i = 0; i < width; i++)
			{
				for (var j = 0; j < height; j++)
				{
					var imagePoint = new Vector2(i, j);
					var worldPoint = ImagePointToWorldPoint(imagePoint);
					var gridPoint = map[worldPoint];
					var color = Colors[colorFunction(gridPoint)];

					pixels[i + width*j] = color;
				}
			}

			texture.SetPixels(pixels);
			texture.Apply();
		}

		public static Color Blend(float t, Color color1, Color color2)
		{
			var r = color1.r*(1 - t) + color2.r*t;
			var g = color1.g*(1 - t) + color2.g*t;
			var b = color1.b*(1 - t) + color2.b*t;
			var a = color1.a*(1 - t) + color2.a*t;

			return new Color(r, g, b, a);
		}
	}
}