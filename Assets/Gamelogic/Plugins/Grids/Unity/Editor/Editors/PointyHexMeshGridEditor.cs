//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEditor;

namespace Gamelogic.Grids.Editor.Internal
{
	[CustomEditor(typeof (PointyHexMeshGridBuilder))]
	public class PointyHexMeshGridEditor : MeshGridEditor<PointyHexMeshGridBuilder, PointyHexPoint>
	{
		protected override bool Is2DMap(int mapType)
		{
			var mapTypeEnum = (PointyHexMeshGridBuilder.MapType)mapType;

			if (mapTypeEnum == PointyHexMeshGridBuilder.MapType.Custom) return false;

			return true;
		}

		override protected bool ShowSize(int shape)
		{
			var shapeEnum = (PointyHexMeshGridBuilder.Shape)shape;

			if (shapeEnum == PointyHexMeshGridBuilder.Shape.Diamond) return true;
			if (shapeEnum == PointyHexMeshGridBuilder.Shape.DownTriangle) return true;
			if (shapeEnum == PointyHexMeshGridBuilder.Shape.UpTriangle) return true;
			if (shapeEnum == PointyHexMeshGridBuilder.Shape.Hexagon) return true;

			return false;
		}

		override protected bool ShowDimensions(int shape)
		{
			var shapeEnum = (PointyHexMeshGridBuilder.Shape)shape;

			if (shapeEnum == PointyHexMeshGridBuilder.Shape.Rectangle) return true;
			if (shapeEnum == PointyHexMeshGridBuilder.Shape.FatRectangle) return true;
			if (shapeEnum == PointyHexMeshGridBuilder.Shape.ThinRectangle) return true;
			if (shapeEnum == PointyHexMeshGridBuilder.Shape.Parallelogram) return true;

			return false;
		}

		protected override bool IsCustomShape(int shape)
		{
			var shapeEnum = (PointyHexMeshGridBuilder.Shape)shape;

			if (shapeEnum == PointyHexMeshGridBuilder.Shape.Custom) return true;

			return false;
		}
	}

	[CustomEditor(typeof(FlatHexMeshGridBuilder))]
	public class FlatHexMeshGridEditor : MeshGridEditor<FlatHexMeshGridBuilder, FlatHexPoint>
	{

		protected override bool Is2DMap(int mapType)
		{
			var mapTypeEnum = (FlatHexMeshGridBuilder.MapType)mapType;

			if (mapTypeEnum == FlatHexMeshGridBuilder.MapType.Custom) return false;

			return true;
		}

		protected override bool ShowSize(int shape)
		{
			var shapeEnum = (FlatHexMeshGridBuilder.Shape)shape;

			if (shapeEnum == FlatHexMeshGridBuilder.Shape.Diamond) return true;
			if (shapeEnum == FlatHexMeshGridBuilder.Shape.LeftTriangle) return true;
			if (shapeEnum == FlatHexMeshGridBuilder.Shape.RightTriangle) return true;
			if (shapeEnum == FlatHexMeshGridBuilder.Shape.Hexagon) return true;

			return false;
		}

		protected override bool ShowDimensions(int shape)
		{
			var shapeEnum = (FlatHexMeshGridBuilder.Shape)shape;

			if (shapeEnum == FlatHexMeshGridBuilder.Shape.Rectangle) return true;
			if (shapeEnum == FlatHexMeshGridBuilder.Shape.FatRectangle) return true;
			if (shapeEnum == FlatHexMeshGridBuilder.Shape.ThinRectangle) return true;
			if (shapeEnum == FlatHexMeshGridBuilder.Shape.Parallelogram) return true;

			return false;
		}

		protected override bool IsCustomShape(int shape)
		{
			var shapeEnum = (FlatHexMeshGridBuilder.Shape)shape;

			if (shapeEnum == FlatHexMeshGridBuilder.Shape.Custom) return true;

			return false;
		}
	}

	[CustomEditor(typeof(RectMeshGridBuilder))]
	public class RectMeshGridEditor : MeshGridEditor<RectMeshGridBuilder, RectPoint>
	{
		protected override bool Is2DMap(int mapType)
		{
			var mapTypeEnum = (RectMeshGridBuilder.MapType)mapType;

			if (mapTypeEnum == RectMeshGridBuilder.MapType.Custom) return false;

			return true;
		}

		protected override bool ShowSize(int shape)
		{
			var shapeEnum = (RectMeshGridBuilder.Shape)shape;

			if (shapeEnum == RectMeshGridBuilder.Shape.Circle) return true;

			return false;
		}

		protected override bool ShowDimensions(int shape)
		{
			var shapeEnum = (RectMeshGridBuilder.Shape)shape;

			switch (shapeEnum)
			{
				case RectMeshGridBuilder.Shape.Rectangle:
				case RectMeshGridBuilder.Shape.Parallelogram:
				case RectMeshGridBuilder.Shape.CheckerBoard:
					return true;
			}

			return false;
		}

		protected override bool IsCustomShape(int shape)
		{
			var shapeEnum = (RectMeshGridBuilder.Shape)shape;

			if (shapeEnum == RectMeshGridBuilder.Shape.Custom) return true;

			return false;
		}
	}

	[CustomEditor(typeof(DiamondMeshGridBuilder))]
	public class DiamondMeshGridEditor : MeshGridEditor<DiamondMeshGridBuilder, DiamondPoint>
	{
		protected override bool Is2DMap(int mapType)
		{
			var mapTypeEnum = (DiamondMeshGridBuilder.MapType)mapType;

			if (mapTypeEnum == DiamondMeshGridBuilder.MapType.Custom) return false;

			return true;
		}

		protected override bool ShowSize(int shape)
		{
			var shapeEnum = (DiamondMeshGridBuilder.Shape)shape;

			if (shapeEnum == DiamondMeshGridBuilder.Shape.Diamond) return true;

			return false;
		}

		protected override bool ShowDimensions(int shape)
		{
			var shapeEnum = (DiamondMeshGridBuilder.Shape)shape;

			if (shapeEnum == DiamondMeshGridBuilder.Shape.Rectangle) return true;
			if (shapeEnum == DiamondMeshGridBuilder.Shape.FatRectangle) return true;
			if (shapeEnum == DiamondMeshGridBuilder.Shape.ThinRectangle) return true;
			if (shapeEnum == DiamondMeshGridBuilder.Shape.Parallelogram) return true;

			return false;
		}

		protected override bool IsCustomShape(int shape)
		{
			var shapeEnum = (DiamondMeshGridBuilder.Shape)shape;

			if (shapeEnum == DiamondMeshGridBuilder.Shape.Custom) return true;

			return false;
		}
	}
}