//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013-2015 Gamelogic (Pty) Ltd  //
//----------------------------------------------//
using System;

using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Class for holding properties for generating a Poisson disk sample set.
		
		@version1_8
		@ingroup UnityUtilities
	*/
	[Serializable]
	public class PoissonDiskProperties
	{
		/**
			Number of points tried per iteration.
		*/
		public int pointCount;

		/**
			The minimum distance between points.
		*/
		public float minimumDistance;

		/**
			The rectangle in which points are generated.
		*/
		public SerializableRect range;
	}

	[Serializable]
	/**
		Class used for keeping the proeprties of a rectangle.
		@version1_8
		@ingroup UnityUtilities
	*/
	public class SerializableRect
	{
		public float left;
		public float top;
		public float width;
		public float height;

		public Rect ToRect()
		{
			return new Rect(left, top, width, height);
		}
	}
}