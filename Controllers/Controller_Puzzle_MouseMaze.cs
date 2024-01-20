using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Puzzle_MouseMaze : Controller
{
    public override void HandleWPressed()
    {
        transform.position += (Vector3.up * 0.01f);
    }

    public override void HandleSPressed()
    {
        transform.position += (Vector3.down * 0.01f);
    }

    public override void HandleAPressed()
    {
        transform.position += (Vector3.left * 0.01f);
    }

    public override void HandleDPressed()
    {
        transform.position += (Vector3.right * 0.01f);
    }
}
