using UnityEngine;

public class HexCell : MonoBehaviour
{
    [SerializeField]
    HexCell[] neighbors;
    public HexCoordinates coordinates;

    public Color color;

    public HexCell GetNeighbor(HexDirection direction){
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell){
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }
}
