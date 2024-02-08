using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : MonoBehaviour
{
    public Cell CurrentCell;
    public Node StartNode;
    public Node TargetNode;
    public float ChaserSpeed { get; private set; } = 1;

    public Node GetStartNode()
    {
        return Pathfinder_Base.GetNodeAtPosition(CurrentCell.Coordinates.X, CurrentCell.Coordinates.Y);
    }
    public void BlowUp()
    {
        Destroy(gameObject);
    }
}
