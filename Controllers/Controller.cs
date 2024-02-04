using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Controller : MonoBehaviour
{
    public KeyBindings KeyBindings;
    protected SpriteRenderer _spriteRenderer;

    void Awake()
    {
        KeyBindings = new KeyBindings();
        KeyBindings.LoadBindings();
        KeyBindings.InitialiseKeyActions(this);
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        foreach (var actionKey in KeyBindings.SinglePressKeyActions.Keys)
        {
            if (Input.GetKeyDown(KeyBindings.Keys[actionKey]))
            {
                KeyBindings.SinglePressKeyActions[actionKey]?.Invoke();
            }
        }

        foreach (var actionKey in KeyBindings.ContinuousPressKeyActions.Keys)
        {
            if (Input.GetKey(KeyBindings.Keys[actionKey]))
            {
                KeyBindings.ContinuousPressKeyActions[actionKey]?.Invoke();
            }
        }
    }

    public virtual void HandleWPressed()
    {

    }
    public virtual void HandleSPressed()
    {

    }
    public virtual void HandleAPressed()
    {

    }
    public virtual void HandleDPressed()
    {

    }

    public virtual void HandleSpacePressed()
    {

    }
    public virtual void HandleUpPressed()
    {

    }
    public virtual void HandleDownPressed()
    {

    }
    public virtual void HandleLeftPressed()
    {

    }
    public virtual void HandleRightPressed()
    {

    }

    public void HandleEPressed()
    {
        if (Manager_Game.Instance.Player.ClosestInteractableObject != null)
        {
            Manager_Game.Instance.Player.ClosestInteractableObject.Interact(Manager_Game.Instance.Player.gameObject);
        }
    }

    public virtual void HandleEscapePressed()
    {

    }
}
