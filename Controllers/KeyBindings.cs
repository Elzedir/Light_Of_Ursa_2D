using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public enum ActionKey
{
    Move_Up, Move_Down, Move_Left, Move_Right,
    Arrow_Up, Arrow_Down, Arrow_Left, Arrow_Right,
    Mouse0, Mouse1,
    E, Escape,Space,
}

[Serializable]
public class KeyBindings
{
    public Dictionary<ActionKey, KeyCode> Keys = new();
    public Dictionary<ActionKey, Action> SinglePressKeyActions {  get; private set; }
    public Dictionary<ActionKey, Action> ContinuousPressKeyActions { get; private set; }
    public KeyBindings()
    {
        Keys.Add(ActionKey.Move_Up, KeyCode.W);
        Keys.Add(ActionKey.Move_Down, KeyCode.S);
        Keys.Add(ActionKey.Move_Left, KeyCode.A);
        Keys.Add(ActionKey.Move_Right, KeyCode.D);

        Keys.Add(ActionKey.Arrow_Up, KeyCode.UpArrow);
        Keys.Add(ActionKey.Arrow_Down, KeyCode.DownArrow);
        Keys.Add(ActionKey.Arrow_Left, KeyCode.LeftArrow);
        Keys.Add(ActionKey.Arrow_Right, KeyCode.RightArrow);

        Keys.Add(ActionKey.Mouse0, KeyCode.Mouse0);
        Keys.Add(ActionKey.Mouse1, KeyCode.Mouse1);

        Keys.Add(ActionKey.E, KeyCode.E);
        Keys.Add(ActionKey.Escape, KeyCode.Escape);
        Keys.Add(ActionKey.Space, KeyCode.Space);
    }

    public void InitialiseKeyActions(Controller controller)
    {
        SinglePressKeyActions = new Dictionary<ActionKey, Action>
        {
            { ActionKey.E, controller.HandleEPressed },
            { ActionKey.Escape, controller.HandleEscapePressed }
        };

        ContinuousPressKeyActions = new Dictionary<ActionKey, Action>
        {
            { ActionKey.Move_Up, controller.HandleWPressed },
            { ActionKey.Move_Down, controller.HandleSPressed },
            { ActionKey.Move_Left, controller.HandleAPressed },
            { ActionKey.Move_Right, controller.HandleDPressed },
            { ActionKey.Space, controller.HandleSpacePressed },
            { ActionKey.Arrow_Up, controller.HandleUpPressed },
            { ActionKey.Arrow_Down, controller.HandleDownPressed },
            { ActionKey.Arrow_Left, controller.HandleLeftPressed },
            { ActionKey.Arrow_Right, controller.HandleRightPressed }
        };
    }

    public void RebindKey(ActionKey action, KeyCode newKey)
    {
        if (Keys.ContainsKey(action))
        {
            Keys[action] = newKey;
        }

        SaveBindings();
    }

    public void SaveBindings()
    {
        foreach (var key in Keys)
        {
            PlayerPrefs.SetInt(key.Key.ToString(), (int)key.Value);
        }

        PlayerPrefs.Save();
    }

    public void LoadBindings()
    {
        foreach (ActionKey key in Enum.GetValues(typeof(ActionKey)))
        {
            string keyString = key.ToString();

            if (PlayerPrefs.HasKey(keyString))
            {
                Keys[key] = (KeyCode)PlayerPrefs.GetInt(keyString);
            }
        }
    }
}