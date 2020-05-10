﻿using UnityEngine;


//Classe que gerencia a criação de cada triângulo
public class HexCell : MonoBehaviour
{

	public HexCoordinates coordinates;

	public Color color;

	

	[SerializeField]
	HexCell[] neighbors;

	public RectTransform uiRect;

	public HexCell GetNeighbor (HexDirection direction)
	{
		return neighbors[(int)direction];
	}

	public void SetNeighbor (HexDirection direction, HexCell cell)
	{
		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
	}

	public int Elevation
	{
		get
		{
			return elevation;
		}
		set
		{
			elevation = value;
			Vector3 position = transform.localPosition;
			position.y = value * HexMetrics.elevationStep;
			transform.localPosition = position;

			Vector3 uiPosition = uiRect.localPosition;
			uiPosition.z = elevation * -HexMetrics.elevationStep;
			uiRect.localPosition = uiPosition;
		}
	}

	private int elevation;

	public HexEdgeType GetEdgeType (HexDirection direction)
	{
		return HexMetrics.GetEdgeType(elevation, neighbors[(int)direction].elevation);
	}

	public HexEdgeType GetEdgeType (HexCell otherCell)
	{
		return HexMetrics.GetEdgeType(elevation, otherCell.elevation);
	}
}