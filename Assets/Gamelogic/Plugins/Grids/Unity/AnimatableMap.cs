//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		A map that changes over time.  
		
		@version1_0

		@ingroup UnityUtilities
	*/
	public class AnimatableMap<TPoint> : AbstractMap<TPoint>
		where TPoint : IGridPoint<TPoint>
	{
		readonly IMap<TPoint> baseMap;
		readonly Func<Vector2, float, Vector2> animation;
		readonly Func<Vector2, float, Vector2> inverseAnimation;
	
		public AnimatableMap(
			Vector2 cellDimensions, 
			IMap<TPoint> baseMap, 
			Func<Vector2, float, Vector2> animation,
			Func<Vector2, float, Vector2> inverseAnimation) :
			base(cellDimensions)
		{
			this.baseMap = baseMap;
			this.animation = animation;
			this.inverseAnimation = inverseAnimation;
			gridPointTransform = baseMap.GridPointTransform;
			inverseGridPointTransform = baseMap.InverseGridPointTransform;
		}
	
		public AnimatableMap(
			Vector2 cellDimensions,
			IMap<TPoint> baseMap,
			Func<Vector2, float, Vector2> animation) :
			this(cellDimensions, baseMap, animation, (x, t)=> animation(x, -t))
		{}
	
		public override TPoint RawWorldToGrid(Vector2 worldPoint)
		{
#if UNITY_EDITOR
			return baseMap.RawWorldToGrid(inverseAnimation(worldPoint, (float) EditorApplication.timeSinceStartup));
#else
			return baseMap.RawWorldToGrid(inverseAnimation(worldPoint, Time.time));
#endif
		}
	
		public override Vector2 GridToWorld(TPoint gridPoint)
		{
#if UNITY_EDITOR
			return animation(baseMap.GridToWorld(gridPoint), (float) EditorApplication.timeSinceStartup);
#else
			return animation(baseMap.GridToWorld(gridPoint), Time.time);
#endif
		}
	}
	
	/**
		These functions are defined as extensions so that IMap can remain pure (that is, not
		access Time.time).

		These methods are implemented as extension methods so that they do not
		become part of the "pure" interface of IMap. (Time.time is a "non-pure", 
		very Unity-specific feature).
	*/
	public static class MapAnimationExtensions
	{
		/**
			Only use this method if animation(x, -t) is the inverse of animation(x, t).
		*/
		public static IMap<TPoint> Animate<TPoint>(this IMap<TPoint> map, Func<Vector2, float, Vector2> animation)
			where TPoint : IGridPoint<TPoint>
		{
			return new AnimatableMap<TPoint>(map.GetCellDimensions(), map, animation);
		}
	
		/**
			Animates this grid using a function animation that takes a point and time. 
			The inverse animation is the inverse mapping at time t, that is,
	
			inverseAmimation(animation(someVector, t), t) == someVector
	
			Example:
			\code
			map = new PointyHexMap(hexDimensions)
				.AnchorCellMiddleCenter()
				.WithWindow(ExampleUtils.ScreenRect)
				.AlignMiddleCenter(grid)
				
				//Rotate
				.Animate((x, t) => x.Rotate(45*t), (x, t) => x.Rotate(-45*t))
	
				//Translate
				.Animate((x, t) => x + new Vector2(75 * Mathf.Sin(t*5), 0),
						 (x, t) => x - new Vector2(75 * Mathf.Sin(t*5), 0))
	
				//Scale
				.Animate((x, t) => x * (1 + Mathf.Sin(t*7)), 
						 (x, t) => x / (1 + Mathf.Sin(t*7))) 
					;
			\endcode
		*/
		public static IMap<TPoint> Animate<TPoint>(this IMap<TPoint> map, Func<Vector2, float, Vector2> animation, Func<Vector2, float, Vector2> inverseAnimation)
			where TPoint : IGridPoint<TPoint>
		{
			return new AnimatableMap<TPoint>(map.GetCellDimensions(), map, animation, inverseAnimation);
		}
	}
}
