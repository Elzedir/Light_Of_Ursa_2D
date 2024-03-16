using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Manager_Grid : MonoBehaviour
{
    public Tilemap Floor { get; private set; }
    public static int Rows { get; private set; }
    public static int XOffset { get; private set; }
    public static int Columns { get; private set; }
    public static int YOffset { get; private set; }
    public Tilemap Walls { get; private set; }

    void Start()
    {
        Floor = Manager_Game.FindTransformRecursively(transform, "Floor_01").GetComponent<Tilemap>();
        Walls = Manager_Game.FindTransformRecursively(transform, "Walls").GetComponent<Tilemap>();

        _initialiseTilemap();
    }

    void _initialiseTilemap()
    {
        Rows = Floor.cellBounds.xMax - Floor.cellBounds.xMin;
        Columns = Floor.cellBounds.xMax - Floor.cellBounds.xMin;

        NodeArray.Nodes = NodeArray.InitializeArray(Floor.cellBounds.xMax - Floor.cellBounds.xMin, Floor.cellBounds.yMax - Floor.cellBounds.yMin);

        XOffset = 0 - Floor.cellBounds.xMin;
        YOffset = 0 - Floor.cellBounds.yMin;

        for (int row = Floor.cellBounds.xMin; row < Floor.cellBounds.xMax; row++)
        {
            for (int col = Floor.cellBounds.yMin; col < Floor.cellBounds.yMax; col++)
            {
                Pathfinder_Base.GetNodeAtPosition(row + XOffset, col + YOffset).UpdateMovementCost(Direction.None, 1);
            }
        }

        for (int row = Walls.cellBounds.xMin; row < Walls.cellBounds.xMax; row++)
        {
            for (int col = Walls.cellBounds.yMin; col < Walls.cellBounds.yMax; col++)
            {
                if (Walls.GetTile(new Vector3Int(row, col, 0)) == null) continue;

                Vector3Int nodePos = new Vector3Int(row + XOffset, col + YOffset, 0);
                Pathfinder_Base.GetNodeAtPosition(nodePos.x, nodePos.y).UpdateMovementCost(Direction.None, double.PositiveInfinity);
            }
        }
    }
}
