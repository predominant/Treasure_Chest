//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System;

namespace Gamelogic.Grids
{
	[AttributeUsage(AttributeTargets.Class |
					AttributeTargets.Struct |
					AttributeTargets.Method |
					AttributeTargets.Interface)]
	/**
		This attribute is used to mark components as experimental. 
		Typically, these are not thoroughly tested, or the design has not been 
		thought out completely. They are likely to contain bugs and change.

		
		
		@version1_0
	*/
	public sealed class ExperimentalAttribute : Attribute
	{
	}
}