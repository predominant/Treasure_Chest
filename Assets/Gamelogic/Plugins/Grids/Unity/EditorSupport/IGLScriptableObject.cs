//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2014 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

namespace Gamelogic.Grids
{
	/**
		Classes that can be edited in the inspector using a custum editor should implement this 
		interface to allow the editor to update the presentation when necessary.

		@version1_8
	*/
	public interface IGLScriptableObject
	{
		/**
			@param forceUpdate When true, the object must update its presentation state with its logical state.
			When false, it can do so depending on the update settings of the object.
		*/
		void __UpdatePresentation(bool forceUpdate);
	}
}
