using System;

namespace Gamelogic.Grids
{
	[Version(1, 14)]
	public class CustomMap3DBuilder : GLMonoBehaviour
	{
		/// <summary>
		/// Creates a new IMap3D for the given point type.
		/// </summary>
		/// <typeparam name="TPoint"></typeparam>
		/// <returns></returns>
		virtual public IMap3D<TPoint> CreateMap<TPoint>()
			where TPoint : IGridPoint<TPoint>
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates a new IMap3D for the given point type.
		/// </summary>
		/// <typeparam name="TPoint"></typeparam>
		/// <returns></returns>
		virtual public IMeshMap<TPoint> CreateMeshMap<TPoint>()
			where TPoint : IGridPoint<TPoint>
		{
			throw new NotImplementedException();
		}
	}
}