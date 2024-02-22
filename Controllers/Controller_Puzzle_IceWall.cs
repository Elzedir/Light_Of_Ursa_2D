using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IceWallDirection { None, Up, Down, Left, Right }


public class Controller_Puzzle_IceWall : Controller, PathfinderMover
{
    (bool, IceWallDirection) Direction;
    Vector3 _centrePosition;

    Spawner_IceWall _spawner;
    int _playerStaminaCurrent;
    int _playerStaminaMax;
    float _playerExtraStamina;
    Cell_IceWall _lastCell;
    public Pathfinder_Base Pathfinder { get; private set; }
    public Cell_IceWall CurrentCell { get; private set; }

    void Start()
    {
        //transform.position += new Vector3(0.5f, 0.5f, 0);

        KeyBindings.ContinuousPressKeyActions.Remove(ActionKey.Space);

        KeyBindings.SinglePressKeyActions.Add(ActionKey.Space, HandleSpacePressed);
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

    public override void HandleUpPressed()
    {
        _lean(IceWallDirection.Up);
    }

    public override void HandleDownPressed()
    {
        _lean(IceWallDirection.Down);
    }

    public override void HandleLeftPressed()
    {
        _lean(IceWallDirection.Left);
    }

    public override void HandleRightPressed()
    {
        _lean(IceWallDirection.Right);
    }
    public override void HandleSpacePressed()
    {
        if (!Direction.Item1) return;

        if (!_spawner.PlayerCanMove(Direction.Item2)) return;

        transform.position = _centrePosition + _leanDirection(Direction.Item2);
        Direction = (false, IceWallDirection.None);
    }

    public void Initialise(Spawner_IceWall spawner, int playerExtraStamina)
    {
        _spawner = spawner;
        _playerExtraStamina = playerExtraStamina;
        Pathfinder = new Pathfinder_Base();
    }

    protected override void Update()
    {
        base.Update();

        if (Direction.Item2 == IceWallDirection.None) return;

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D) 
            || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow)) _returnToCenter();
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

    public bool DecreaseStamina(Cell_IceWall cell)
    {
        if (_lastCell == cell) return true;

        _playerStaminaCurrent -= cell.CellHealth;
        Manager_Puzzle.Instance.UseStamina(_playerStaminaCurrent.ToString());
        _lastCell = cell;

        if (_playerStaminaCurrent <= 0) return false;
        else return true;
    }

    public void Fall()
    {
        _playerStaminaCurrent = _playerStaminaMax;
        Manager_Puzzle.Instance.UseStamina(_playerStaminaCurrent.ToString());
    }

    public void MoveTo(Node_Base target)
    {
        List<Coordinates> path = Pathfinder.RetrievePath(Pathfinder_Base.GetNodeAtPosition(CurrentCell.Coordinates.X, CurrentCell.Coordinates.Y), target);

        foreach (Coordinates coordinate in path)
        {
            _playerStaminaMax += (int)Pathfinder_Base.GetNodeAtPosition(coordinate.X, coordinate.Y).MovementCost;
        }

        _playerStaminaMax += (int)(_playerStaminaMax * (_playerExtraStamina / 100));
        _playerStaminaCurrent = _playerStaminaMax;
    }

    public LinkedList<Coordinates> GetObstaclesInVision()
    {
        return new LinkedList<Coordinates>();
    }

    public void SetCurrentCell(Cell_IceWall cell)
    {
        CurrentCell = cell;
        _centrePosition = cell.transform.position;
    }
}
