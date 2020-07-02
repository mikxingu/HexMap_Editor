using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;

//Classe com as funções do meu editor de mapa.
public class HexMapEditor : MonoBehaviour
{
	//public Color[] colors;

	public Material terrainMaterial;

	//bool editMode;

	public HexGrid hexGrid;

//	public HexUnit unitPrefab;

	int activeElevation;
	int activeWaterLevel;
	int activeTerrainTypeIndex;
	int activeUrbanLevel, activeFarmLevel, activePlantLevel, activeSpecialIndex;

	//Color activeColor;

	int brushSize;

	//bool applyColor;
	bool applyElevation = true;
	bool applyWaterLevel = true;

	bool applyUrbanLevel, applyFarmLevel, applyPlantLevel, applySpecialIndex;

	enum OptionalToggle { Ignore, Yes, No }

	OptionalToggle riverMode, roadMode, walledMode;

	bool isDrag;
	HexDirection dragDirection;
	//HexCell previousCell, searchFromCell, searchToCell;
	HexCell previousCell;

	/*
	public void SelectColor(int index)
	{
		applyColor = index >= 0;
		if (applyColor)
		{
			activeColor = colors[index];
		}
	}
	*/


	public void SetBrushSize(float size)
	{
		brushSize = (int)size;
	}


	public void SetElevation(float elevation)
	{
		activeElevation = (int)elevation;
	}

	public void SetApplyElevation(bool toggle)
	{
		applyElevation = toggle;
	}


	/*public void ShowUI(bool visible)
	{
		hexGrid.ShowUI(visible);
	}*/

	public void ShowGrid (bool visible)
	{
		if (visible)
		{
			terrainMaterial.EnableKeyword("GRID_ON");
		}
		else
		{
			terrainMaterial.DisableKeyword("GRID_ON");
		}
	}

	public void SetRiverMode(int mode)
	{
		riverMode = (OptionalToggle)mode;
	}

	public void SetRoadMode(int mode)
	{
		roadMode = (OptionalToggle)mode;
	}

	public void SetEditMode (bool toggle)
	{
		//editMode = toggle;
		//hexGrid.ShowUI(!toggle);
		enabled = toggle;
	}

	public void SetApplySpecialIndex (bool toggle)
	{
		applySpecialIndex = toggle;
	}

	public void SetTerrainTypeIndex (int index)
	{
		activeTerrainTypeIndex = index;
	}

	public void SetApplyWaterlevel(bool toggle)
	{
		applyWaterLevel = toggle;
	}

	public void SetWaterLevel(float level)
	{
		activeWaterLevel = (int)level;
	}

	public void SetApplyUrbanLevel(bool toggle)
	{
		applyUrbanLevel = toggle;
	}

	public void SetUrbanLevel(float level)
	{
		activeUrbanLevel = (int)level;
	}

	public void SetApplyFarmLevel(bool toggle)
	{
		applyFarmLevel = toggle;
	}

	public void SetFarmLevel(float level)
	{
		activeFarmLevel = (int)level;
	}

	public void SetApplyPlantLevel(bool toggle)
	{
		applyPlantLevel = toggle;
	}

	public void SetPlantLevel(float level)
	{
		activePlantLevel = (int)level;
	}

	public void SetSpecialIndex (float index)
	{
		activeSpecialIndex = (int)index;
	}

	void Awake()
	{
		terrainMaterial.DisableKeyword("GRID_ON");
		Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
		SetEditMode(true);
	}

	void Update()
	{
		if (!EventSystem.current.IsPointerOverGameObject())
		{
			if (Input.GetMouseButton(0))
			{
				HandleInput();
				return;
			}
			if (Input.GetKeyDown(KeyCode.U))
			{
				if (Input.GetKey(KeyCode.LeftShift))
				{
					DestroyUnit();
				}
				else
				{
					CreateUnit();
				}
				return;
			}
		}
		previousCell = null;
	}

	/*
if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
{
	HandleInput();
}
else
{
	previousCell = null;
}*/
	/*
	if (!EventSystem.current.IsPointerOverGameObject())
	{
		if (Input.GetMouseButton(0))
		{
			HandleInput();
			return;
		}
		if (Input.GetKeyDown(KeyCode.U))
		{
			CreateUnit();
			return;
		}
	}
	previousCell = null;*/

	void CreateUnit ()
	{
		HexCell cell = GetCellUnderCursor();
		if (cell && !cell.Unit)
		{
			//HexUnit unit = Instantiate(unitPrefab);
			//unit.transform.SetParent(hexGrid.transform, false);
			//unit.Location = cell;
			//unit.Orientation = Random.Range(0f, 360f);
			hexGrid.AddUnit(
				Instantiate(HexUnit.unitPrefab), cell, Random.Range(0f, 360f));
		}
	}

	void DestroyUnit()
	{
		HexCell cell = GetCellUnderCursor();
		if (cell && cell. Unit)
		{
			//cell.Unit.Die();
			hexGrid.RemoveUnit(cell.Unit);
		}
	}

	void HandleInput()
	{
		HexCell currentCell = GetCellUnderCursor();
		if (currentCell)
		{
			if (previousCell && previousCell != currentCell)
			{
				ValidateDrag(currentCell);
			}
			else
			{
				isDrag = false;
			}
			
			EditCells(currentCell);
			previousCell = currentCell;
		}
		else
		{
			previousCell = null;
		}
	}

	HexCell GetCellUnderCursor()
	{
		return hexGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
	}

	void ValidateDrag(HexCell currentCell)
	{
		for (dragDirection = HexDirection.NE; dragDirection <= HexDirection.NW; dragDirection++)
		{
			if (previousCell.GetNeighbor(dragDirection) == currentCell)
			{
				isDrag = true;
				return;
			}
		}
		isDrag = false;
	}

	public void SetWalledMode (int mode)
	{
		walledMode = (OptionalToggle)mode;
	}

	void EditCells(HexCell center)
	{
		int centerX = center.coordinates.X;
		int centerZ = center.coordinates.Z;

		for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
		{
			for (int x = centerX - r; x <= centerX + brushSize; x++)
			{
				EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
			}
		}
		for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
		{
			for (int x = centerX - brushSize; x <= centerX + r; x++)
			{
				EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
			}
		}
	}



	void EditCell(HexCell cell)
	{
		if (cell)
		{
			if (activeTerrainTypeIndex >= 0)
			{
				cell.TerrainTypeIndex = activeTerrainTypeIndex;
			}
		//	if (applyColor)
			//{
				//cell.Color = activeColor;
			//}
			if (applyElevation)
			{
				cell.Elevation = activeElevation;
			}
			if (applyWaterLevel)
			{
				cell.WaterLevel = activeWaterLevel;
			}
			if (applySpecialIndex)
			{
				cell.SpecialIndex = activeSpecialIndex;
			}
			if (applyUrbanLevel)
			{
				cell.UrbanLevel = activeUrbanLevel;
			}
			if (applyFarmLevel)
			{
				cell.FarmLevel = activeFarmLevel;
			}
			if (applyPlantLevel)
			{
				cell.PlantLevel = activePlantLevel;
			}
			if (riverMode == OptionalToggle.No)
			{
				cell.RemoveRiver();
			}
			if (roadMode == OptionalToggle.No)
			{
				cell.RemoveRoads();
			}
			if (walledMode != OptionalToggle.Ignore)
			{
				cell.Walled = walledMode == OptionalToggle.Yes;
			}
			if (isDrag)
			{
				HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
				if (otherCell)
				{
					if (riverMode == OptionalToggle.Yes)
					{
						otherCell.SetOutgoingRiver(dragDirection);
					}
					if (roadMode == OptionalToggle.Yes)
					{
						otherCell.AddRoad(dragDirection);
					}
				}
			}
		}
	}
}