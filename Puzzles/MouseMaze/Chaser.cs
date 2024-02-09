using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : MonoBehaviour, PathfinderMover
{
    public Pathfinder_Base Pathfinder { get; set; }
    public Cell[,] Cells;
    public Cell CurrentCell;
    public Node Target;
    public float ChaserSpeed { get; private set; } = 1;

    bool _isCoroutineRunning;

    public Node GetStartNode()
    {
        return Pathfinder_Base.GetNodeAtPosition(CurrentCell.Coordinates.X, CurrentCell.Coordinates.Y);
    }
    public void BlowUp()
    {
        Destroy(gameObject);
    }

    public void MoveTo(Node target)
    {
        if (_isCoroutineRunning) return;

        Debug.Log($"Predecessor2: {Pathfinder_Base.PredecessorLoopCheck(GetStartNode(), 1000)}");

        Target = target;

        Debug.Log($"Predecessor2: {Pathfinder_Base.PredecessorLoopCheck(GetStartNode(), 1000)}");

        List<Coordinates> path = Pathfinder.RetrievePath(GetStartNode(), target);

        StartCoroutine(FollowPath(path));
    }

    IEnumerator FollowPath(List<Coordinates> path)
    {
        _isCoroutineRunning = true;

        int test = 0;

        foreach (var coord in path)
        {
            Debug.Log($"Chaser Path: {coord.X}_{coord.Y}");
            test++;
        }

        Debug.Log(test);

        foreach (var coord in path)
        {
            Vector3 nextPosition = Cells[coord.X, coord.Y].transform.position;

            yield return Move(nextPosition);
        }
    }

    IEnumerator Move(Vector3 nextPosition)
    {
        while (Vector3.Distance(transform.position, nextPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextPosition, ChaserSpeed * Time.deltaTime);
            yield return null;
        }

        _isCoroutineRunning = false;
    }

    public LinkedList<Coordinates> GetObstaclesInVision()
    {
        return new LinkedList<Coordinates>();
    }
}
