using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
	public int width = 6;
	public int height = 6;

	public HexCell cellPrefab;

	HexCell[] cells;

	public Text cellLabelPrefab;

	Canvas gridCanvas;

	void Awake()
	{
		gridCanvas = GetComponentInChildren<Canvas>();
		cells = new HexCell[height * width];

		for (int z = 0, i = 0; z < height; z++)
		{
			for (int x = 0; x< width; x++)
			{
				CreateCell(x, z, i++);
			}
		}
	}

	void CreateCell(int x, int z, int i)
	{
		Vector3 position;
		position.x = x * 10f;
		position.y = 0f;
		position.z = z * 10f;

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		Text label = Instantiate<Text>(cellLabelPrefab);
		label.rectTransform.SetParent(gridCanvas.transform, false);
		label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
		label.text = x.ToString() + "\n" + z.ToString();
	
	}
}
