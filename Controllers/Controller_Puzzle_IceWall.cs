using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IceWallDirection { None, Up, Down, Left, Right }


public class Controller_Puzzle_IceWall : Controller
{
    (bool, IceWallDirection) Direction;
    Vector3 _centrePosition;

    Spawner_IceWall _spawner;

    void Start()
    {
        //transform.position += new Vector3(0.5f, 0.5f, 0);

        KeyBindings.ContinuousPressKeyActions.Remove(ActionKey.Space);

        KeyBindings.SinglePressKeyActions.Add(ActionKey.Space, HandleSpacePressed);
    }

    public void Initialise(Spawner_IceWall spawner)
    {
        _spawner = spawner;
    }

    public override void HandleWPressed()
    {
        _lean(IceWallDirection.Up);
    }

    public override void HandleSPressed()
    {
        _lean(IceWallDirection.Down);
    }

    public override void HandleAPressed()
    {
        _lean(IceWallDirection.Left);
    }

    public override void HandleDPressed()
    {
        _lean(IceWallDirection.Right);
    }
    public override void HandleSpacePressed()
    {
        if (!Direction.Item1) return;

        if (!_spawner.PlayerCanMove(Direction.Item2)) return;

        transform.position = _centrePosition + _leanDirection(Direction.Item2);
        Direction = (false, IceWallDirection.None);
        _centrePosition = transform.position;
    }

    protected override void Update()
    {
        base.Update();

        if (Direction.Item2 == IceWallDirection.None) return;

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) _returnToCenter();
    }

    void _lean(IceWallDirection direction)
    {
        if (Direction.Item1) return;

        transform.position += _leanDirection(direction) * 0.3f;
        Direction = (true, direction);
    }

    void _returnToCenter()
    {
        if (!Direction.Item1) return;

        transform.position = _centrePosition; // Change to an IENumerator to move over time.
        Direction = (false, IceWallDirection.None);
    }

    Vector3 _leanDirection(IceWallDirection direction)
    {
        switch (direction)
        {
            case IceWallDirection.Up: return Vector3.up;
            case IceWallDirection.Down: return Vector3.down;
            case IceWallDirection.Left: return Vector3.left;
            case IceWallDirection.Right: return Vector3.right;
            default: return Vector3.zero;
        }
    }
}
