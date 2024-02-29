using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Menu_Base : MonoBehaviour
{
    protected bool _isOpen;
    public bool IsOpen
    {
        get { return _isOpen; }
    }

    public virtual void OpenMenu()
    {

    }

    public virtual void CloseMenu()
    {

    }
}
