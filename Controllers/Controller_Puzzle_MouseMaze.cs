using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MouseMazeColour { None, Blue, Green, Red }

public class Controller_Puzzle_MouseMaze : Controller
{
    public event Action OnBreakWall;
    public MouseMazeColour PlayerColour { get; private set; }
    float _playerSpeed = 2;
    void Start()
    {
        KeyBindings.ContinuousPressKeyActions.Remove(ActionKey.Space);
        KeyBindings.SinglePressKeyActions.Add(ActionKey.Space, HandleSpacePressed);
    }
    public override void HandleWPressed()
    {
        transform.position += (Vector3.up * 0.01f * _playerSpeed);
    }

    public override void HandleSPressed()
    {
        transform.position += (Vector3.down * 0.01f * _playerSpeed);
    }

    public override void HandleAPressed()
    {
        transform.position += (Vector3.left * 0.01f * _playerSpeed);
    }

    public override void HandleDPressed()
    {
        transform.position += (Vector3.right * 0.01f * _playerSpeed);
    }

    public override void HandleSpacePressed()
    {
        OnBreakWall?.Invoke();
    }

    public void SetPlayerColour(MouseMazeColour playerColour, Color color)
    {
        PlayerColour = playerColour;
        _spriteRenderer.color = color;
    }
}
