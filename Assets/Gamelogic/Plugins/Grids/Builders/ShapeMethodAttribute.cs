//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System;

namespace Gamelogic.Grids
{
	/**
		Methods in an Op (sublcasses of AbstractOp) marked with this 
		attribute are automatically added as static methods to the 
		appropriate grid class for convenience.		
		
		@version1_0
		@ingroup Scaffolding
	*/
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class ShapeMethodAttribute : Attribute
	{ }
}