using UnityEngine;

public class NewMapMenu : MonoBehaviour
{
	public HexGrid hexGrid;

	public void Open()
	{
		gameObject.SetActive(true);
		HexMapCamera.Locked = true;
	}

	public void Close ()
	{
		gameObject.SetActive(false);
		HexMapCamera.Locked = false;
	}

	void CreateMap (int x, int z)
	{
		hexGrid.CreateMap(x, z);
		HexMapCamera.ValidatePosition();
		Close();
	}

	public void CreateSmallMap ()
	{
		CreateMap(20, 15);
	}

	public void CreateMediumMap()
	{
		CreateMap(20, 30);
	}

	public void CreateLargeMap ()
	{
		CreateMap(80, 60);
	}
}
