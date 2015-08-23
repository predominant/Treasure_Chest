//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//
namespace Gamelogic.Grids
{
	/**
		Provides a function for wrapping points that is used
		by wrapped grids.

		Since many such functions require lookup tables, 
		it's more suitable to provide it as a class than 
		providing it as a delagate.
		
		@version1_7
		@ingroup Scaffolding
	*/
	public interface IPointWrapper<TPoint> where TPoint : IGridPoint<TPoint>
	{
		/**
			Returns a new point, that corresponds to the wrapped version of the 
			give point.
		*/
		TPoint Wrap(TPoint point);
	}
}
