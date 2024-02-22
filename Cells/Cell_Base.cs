using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell_Base : MonoBehaviour
{
    protected SpriteRenderer _spriteRenderer;
    protected BoxCollider2D _boxCollider;
    public Coordinates Coordinates { get; protected set; }

    public virtual void Show()
    {
        _spriteRenderer.enabled = true;
    }

    public virtual void Hide()
    {
        _spriteRenderer.enabled = false;
    }

    public virtual void MarkCell(Color color)
    {
        _spriteRenderer.color = color;
    }
}
