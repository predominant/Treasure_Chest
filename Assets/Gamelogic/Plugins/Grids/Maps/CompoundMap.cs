//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System;
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		A maps that combines two maps through function composition.
	
		
		
		@version1_0

		@ingroup Scaffolding
	*/
	[Immutable]
	public class CompoundMap<TPoint> : AbstractMap<TPoint>
		where TPoint : IGridPoint<TPoint>
	{
		protected readonly Func<Vector2, Vector2> transform;
		protected readonly Func<Vector2, Vector2> inverseTransform;
		protected readonly IMap<TPoint> leftMap;

		public CompoundMap(IMap<TPoint> leftMap, Func<Vector2, Vector2> transform, Func<Vector2, Vector2> inverseTransform) :
			base(leftMap.GetCellDimensions(), leftMap.GetAnchorTranslation())
		{
			this.leftMap = leftMap;
			this.transform = transform;
			this.inverseTransform = inverseTransform;

			gridPointTransform = leftMap.GridPointTransform;
			inverseGridPointTransform = leftMap.InverseGridPointTransform;
		}

		override public TPoint RawWorldToGrid(Vector2 point)
		{
			return leftMap.RawWorldToGrid(inverseTransform(point));
		}

		override public Vector2 GridToWorld(TPoint point)
		{
			return transform(leftMap.GridToWorld(point));
		}

		override public Vector2 GetCellDimensions(TPoint point)
		{
			return leftMap.GetCellDimensions(point);
		}
	}
}