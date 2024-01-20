using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Puzzle_XOY : Controller
{
    void Start()
    {
        transform.position += new Vector3(0.5f, 0.5f, 0);

        KeyBindings.ContinuousPressKeyActions.Remove(ActionKey.Move_Up);
        KeyBindings.ContinuousPressKeyActions.Remove(ActionKey.Move_Down);
        KeyBindings.ContinuousPressKeyActions.Remove(ActionKey.Move_Left);
        KeyBindings.ContinuousPressKeyActions.Remove(ActionKey.Move_Right);

        KeyBindings.SinglePressKeyActions.Add(ActionKey.Move_Up, HandleWPressed);
        KeyBindings.SinglePressKeyActions.Add(ActionKey.Move_Down, HandleSPressed);
        KeyBindings.SinglePressKeyActions.Add(ActionKey.Move_Left, HandleAPressed);
        KeyBindings.SinglePressKeyActions.Add(ActionKey.Move_Right, HandleDPressed);
    }

    public override void HandleWPressed()
    {
        transform.position += Vector3.up;
    }

    public override void HandleSPressed()
    {
        transform.position += Vector3.down;
    }

    public override void HandleAPressed()
    {
        transform.position += Vector3.left;
    }

    public override void HandleDPressed()
    {
        transform.position += Vector3.right;
    }
}
