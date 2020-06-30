using System.Collections.Generic;
using UnityEngine;

public class HexMapGenerator : MonoBehaviour {

	public HexGrid grid;

	public bool useFixedSeed;

	public int seed;

	[Range(0f, 0.5f)]
	public float jitterProbability = 0.25f;

	[Range(20, 200)]
	public int chunkSizeMin = 30;

	[Range(20, 200)]
	public int chunkSizeMax = 100;

	[Range(0f, 1f)]
	public float highRiseProbability = 0.25f;

	[Range(0f, 0.4f)]
	public float sinkProbability = 0.2f;

	[Range(5, 95)]
	public int landPercentage = 50;

	[Range(1, 5)]
	public int waterLevel = 3;

	[Range(-4, 0)]
	public int elevationMinimum = -2;

	[Range(6, 10)]
	public int elevationMaximum = 8;

	HexCellPriorityQueue searchFrontier;

	int searchFrontierPhase;

	int cellCount;

	public void GenerateMap (int x, int z) {
		Random.State originalRandomState = Random.state;
		if (!useFixedSeed) {
			seed = Random.Range(0, int.MaxValue);
			seed ^= (int)System.DateTime.Now.Ticks;
			seed ^= (int)Time.unscaledTime;
			seed &= int.MaxValue;
		}
		Random.InitState(seed);

		cellCount = x * z;
		grid.CreateMap(x, z);
		if (searchFrontier == null) {
			searchFrontier = new HexCellPriorityQueue();
		}
		for (int i = 0; i < cellCount; i++) {
			grid.GetCell(i).WaterLevel = waterLevel;
		}
		CreateLand();
		SetTerrainType();
		for (int i = 0; i < cellCount; i++) {
			grid.GetCell(i).SearchPhase = 0;
		}

		Random.state = originalRandomState;
	}

	void CreateLand () {
		int landBudget = Mathf.RoundToInt(cellCount * landPercentage * 0.01f);
		while (landBudget > 0) {
			int chunkSize = Random.Range(chunkSizeMin, chunkSizeMax - 1);
			if (Random.value < sinkProbability) {
				landBudget = SinkTerrain(chunkSize, landBudget);
			}
			else {
				landBudget = RaiseTerrain(chunkSize, landBudget);
			}
		}
	}

	int RaiseTerrain (int chunkSize, int budget) {
		searchFrontierPhase += 1;
		HexCell firstCell = GetRandomCell();
		firstCell.SearchPhase = searchFrontierPhase;
		firstCell.Distance = 0;
		firstCell.SearchHeuristic = 0;
		searchFrontier.Enqueue(firstCell);
		HexCoordinates center = firstCell.coordinates;

		int rise = Random.value < highRiseProbability ? 2 : 1;
		int size = 0;
		while (size < chunkSize && searchFrontier.Count > 0) {
			HexCell current = searchFrontier.Dequeue();
			int originalElevation = current.Elevation;
			int newElevation = originalElevation + rise;
			if (newElevation > elevationMaximum) {
				continue;
			}
			current.Elevation = newElevation;
			if (
				originalElevation < waterLevel &&
				newElevation >= waterLevel && --budget == 0
			) {
				break;
			}
			size += 1;

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = current.GetNeighbor(d);
				if (neighbor && neighbor.SearchPhase < searchFrontierPhase) {
					neighbor.SearchPhase = searchFrontierPhase;
					neighbor.Distance = neighbor.coordinates.DistanceTo(center);
					neighbor.SearchHeuristic =
						Random.value < jitterProbability ? 1: 0;
					searchFrontier.Enqueue(neighbor);
				}
			}
		}
		searchFrontier.Clear();
		return budget;
	}

	int SinkTerrain (int chunkSize, int budget) {
		searchFrontierPhase += 1;
		HexCell firstCell = GetRandomCell();
		firstCell.SearchPhase = searchFrontierPhase;
		firstCell.Distance = 0;
		firstCell.SearchHeuristic = 0;
		searchFrontier.Enqueue(firstCell);
		HexCoordinates center = firstCell.coordinates;

		int sink = Random.value < highRiseProbability ? 2 : 1;
		int size = 0;
		while (size < chunkSize && searchFrontier.Count > 0) {
			HexCell current = searchFrontier.Dequeue();
			int originalElevation = current.Elevation;
			int newElevation = current.Elevation - sink;
			if (newElevation < elevationMinimum) {
				continue;
			}
			current.Elevation = newElevation;
			if (
				originalElevation >= waterLevel &&
				newElevation < waterLevel
			) {
				budget += 1;
			}
			size += 1;

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = current.GetNeighbor(d);
				if (neighbor && neighbor.SearchPhase < searchFrontierPhase) {
					neighbor.SearchPhase = searchFrontierPhase;
					neighbor.Distance = neighbor.coordinates.DistanceTo(center);
					neighbor.SearchHeuristic =
						Random.value < jitterProbability ? 1: 0;
					searchFrontier.Enqueue(neighbor);
				}
			}
		}
		searchFrontier.Clear();
		return budget;
	}

	void SetTerrainType () {
		for (int i = 0; i < cellCount; i++) {
			HexCell cell = grid.GetCell(i);
			if (!cell.IsUnderwater) {
				cell.TerrainTypeIndex = cell.Elevation - cell.WaterLevel;
			}
		}
	}

	HexCell GetRandomCell () {
		return grid.GetCell(Random.Range(0, cellCount));
	}
}