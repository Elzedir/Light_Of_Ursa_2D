using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell_Base : MonoBehaviour
{
    protected SpriteRenderer _spriteRenderer;
    protected BoxCollider2D _boxCollider;
    public Coordinates Coordinates { get; protected set; }
}
