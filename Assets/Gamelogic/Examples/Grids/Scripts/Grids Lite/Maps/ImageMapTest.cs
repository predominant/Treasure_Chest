using UnityEngine;

namespace Gamelogic.Grids.Examples
{
	public class ImageMapTest : GLMonoBehaviour
	{
		public UVCell cellPrefab;
		public Texture2D texture;
		public GameObject gridRoot;

		public void Start()
		{
			BuildGrid();
		}

		public void BuildGrid()
		{
			var grid = PointyHexGrid<UVCell>.ThinRectangle(11, 11);

			var baseMap = new PointyHexMap(cellPrefab.Dimensions*1.1f);

			Debug.Log(cellPrefab.Dimensions);

			var cellMap = baseMap
				.WithWindow(ExampleUtils.ScreenRect)
				.AlignMiddleCenter(grid)
				.To3DXY();

			var imageMap =
				new ImageMap<PointyHexPoint>(new Rect(0, 0, 1, 1), grid, baseMap);

			foreach (var point in grid)
			{
				var worldPosition = cellMap[point];

				var cell = Instantiate(cellPrefab);
				Debug.Log(cell.Dimensions);

				cell.transform.parent = gridRoot.transform;
				cell.transform.localScale = Vector3.one;
				cell.transform.localPosition = worldPosition;

				cell.SetTexture(texture);
				cell.name = point.ToString();

				var imagePoint = imageMap[point];

				cell.SetUVs(imagePoint, imageMap.GetCellDimensions(point));
			}
		}
	}
}