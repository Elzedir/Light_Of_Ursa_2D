using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : MonoBehaviour, PathfinderMover
{
    public Pathfinder_Base Pathfinder { get; set; }
    public Cell_MouseMaze CurrentCell;
    float _chaserSpeed;

    Coroutine _chasingCoroutine;
    public Spawner_Maze Spawner;

    public void InitialiseChaser(Cell_MouseMaze startCell, Spawner_Maze spawner, float chaserSpeed = 1)
    {
        CurrentCell = startCell;
        Spawner = spawner;
        SpriteRenderer chaserSprite = gameObject.AddComponent<SpriteRenderer>();
        chaserSprite.sprite = Resources.Load<Sprite>("Sprites/Mine");
        chaserSprite.sortingLayerName = "Actors";
        BoxCollider2D chaserColl = gameObject.AddComponent<BoxCollider2D>();
        chaserColl.size = new Vector3(0.4f, 0.4f, 0.4f);
        chaserColl.isTrigger = true;
        Rigidbody2D chaserBody = gameObject.AddComponent<Rigidbody2D>();
        chaserBody.gravityScale = 0;
        chaserBody.freezeRotation = true;
        Pathfinder = new Pathfinder_Base();
        _chaserSpeed = chaserSpeed;
    }

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
        if (_chasingCoroutine != null) StopChasing();

        _chasingCoroutine = StartCoroutine(FollowPath(Pathfinder.RetrievePath(GetStartNode(), target)));
    }

    IEnumerator FollowPath(List<Coordinates> path)
    {
        foreach (Coordinates coordinate in path)
        {
            yield return Move(Spawner.Cells[coordinate.X, coordinate.Y].transform.position);
        }

        _chasingCoroutine = null;
        Spawner.GetNewRoute(this);
    }

    IEnumerator Move(Vector3 nextPosition)
    {
        while (Vector3.Distance(transform.position, nextPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextPosition, _chaserSpeed * Time.deltaTime);
            if (_chasingCoroutine == null) break;
            yield return null;
        }
    }

    public void StopChasing()
    {
        if (_chasingCoroutine == null) return;

        StopCoroutine(_chasingCoroutine);
        _chasingCoroutine = null;
    }

    public LinkedList<Coordinates> GetObstaclesInVision()
    {
        return new LinkedList<Coordinates>();
    }
}
