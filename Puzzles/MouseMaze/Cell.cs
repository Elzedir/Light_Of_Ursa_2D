using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Wall { Top, Bottom, Left, Right }

public class Cell : MonoBehaviour
{
    public int Row { get; private set; }
    public int Col { get; private set; }

    SpriteRenderer _spriteBase;
    SpriteRenderer _spriteRenderer;
    BoxCollider2D _colliderTop;
    BoxCollider2D _colliderBottom;
    BoxCollider2D _colliderLeft;
    BoxCollider2D _colliderRight;

    public bool Visited;
    bool _initialised = false;

    Dictionary<Wall, bool> _sides;

    Spawner_Maze _spawner;
    
    public void InitialiseCell(int row, int col, Spawner_Maze spawner)
    {
        Row = row;
        Col = col;
        _spawner = spawner;

        _spriteBase = gameObject.AddComponent<SpriteRenderer>();
        if (_spawner.Background) _spriteBase.sprite = Resources.Load<Sprite>("Sprites/White ground");

        _spriteRenderer = new GameObject("CellSprite").AddComponent<SpriteRenderer>();
        _spriteRenderer.transform.parent = transform;
        _spriteRenderer.transform.localPosition = Vector3.zero;

        BoxCollider2D coll = gameObject.AddComponent<BoxCollider2D>();
        coll.size = new Vector2(1, 1);
        coll.isTrigger = true;

        _colliderTop = CreateSideCollider("Top");
        _colliderBottom = CreateSideCollider("Bottom");
        _colliderLeft = CreateSideCollider("Left");
        _colliderRight = CreateSideCollider("Right");
    }

    BoxCollider2D CreateSideCollider(string name)
    {
        BoxCollider2D collider = new GameObject($"Collider{name}").AddComponent<BoxCollider2D>();
        collider.transform.parent = transform;
        switch(name)
        {
            case "Top": collider.transform.localPosition = new Vector3(0, 0.5f, 0); collider.size = new Vector2(1, 0.12f); break;
            case "Bottom": collider.transform.localPosition = new Vector3(0, -0.5f, 0); collider.size = new Vector2(1, 0.12f); break;
            case "Left": collider.transform.localPosition = new Vector3(-0.5f, 0, 0); collider.size = new Vector2(0.12f, 1); break;
            case "Right": collider.transform.localPosition = new Vector3(0.5f, 0, 0); collider.size = new Vector2(0.12f, 1); break;
            default: break;
        }

        return collider;
    }

    public void ClearWall(Wall wall)
    {
        if (!_initialised)
        {
            _sides = new Dictionary<Wall, bool>
            {
                { Wall.Top, false },
                { Wall.Bottom, false },
                { Wall.Left, false },
                { Wall.Right, false }
            };

            _initialised = true;
        }

        switch (wall)
        {
            case Wall.Top: Destroy(_colliderTop.gameObject); break;
            case Wall.Bottom: Destroy(_colliderBottom.gameObject); break;
            case Wall.Left: Destroy(_colliderLeft.gameObject); break;
            case Wall.Right: Destroy(_colliderRight.gameObject); break;
            default: break;
        }

        _sides[wall] = true;

        Sprite sprite = null;

        if (!_sides[Wall.Top] && !_sides[Wall.Bottom] && !_sides[Wall.Left] && !_sides[Wall.Right]) sprite = Resources.Load<Sprite>("Sprites/Grid");
        
        if (_sides[Wall.Top] && !_sides[Wall.Bottom] && !_sides[Wall.Left] && !_sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_OpenOneSide"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 270); }
        if (!_sides[Wall.Top] && _sides[Wall.Bottom] && !_sides[Wall.Left] && !_sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_OpenOneSide"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90); }
        if (!_sides[Wall.Top] && !_sides[Wall.Bottom] && _sides[Wall.Left] && !_sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_OpenOneSide"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 0); }
        if (!_sides[Wall.Top] && !_sides[Wall.Bottom] && !_sides[Wall.Left] && _sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_OpenOneSide"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 180); }

        if (_sides[Wall.Top] && _sides[Wall.Bottom] && !_sides[Wall.Left] && !_sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_OpenTwoSides"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90); }
        if (!_sides[Wall.Top] && !_sides[Wall.Bottom] && _sides[Wall.Left] && _sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_OpenTwoSides"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 0); }

        if (_sides[Wall.Top] && !_sides[Wall.Bottom] && _sides[Wall.Left] && !_sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_Corner"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90); }
        if (_sides[Wall.Top] && !_sides[Wall.Bottom] && !_sides[Wall.Left] && _sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_Corner"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 0); }
        if (!_sides[Wall.Top] && _sides[Wall.Bottom] && _sides[Wall.Left] && !_sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_Corner"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 180); }
        if (!_sides[Wall.Top] && _sides[Wall.Bottom] && !_sides[Wall.Left] && _sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_Corner"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 270); }
        
        if (_sides[Wall.Top] && _sides[Wall.Bottom] && _sides[Wall.Left] && !_sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_OpenThreeSides"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 180); }
        if (_sides[Wall.Top] && _sides[Wall.Bottom] && !_sides[Wall.Left] && _sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_OpenThreeSides"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 0); }
        if (_sides[Wall.Top] && !_sides[Wall.Bottom] && _sides[Wall.Left] && _sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_OpenThreeSides"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90); }
        if (!_sides[Wall.Top] && _sides[Wall.Bottom] && _sides[Wall.Left] && _sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_OpenThreeSides"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 270); }

        if (_sides[Wall.Top] && _sides[Wall.Bottom] && _sides[Wall.Left] && _sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_OpenAllSides"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 0); }

        if (sprite != null) _spriteRenderer.sprite = sprite;
        else Debug.Log("Sprite not found.");
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name != "Focus") return;

        _spawner.RefreshMaze(this);
    }

    public void Show()
    {
        if (_spawner.Background) _spriteBase.enabled = false;
        _spriteRenderer.enabled = true;
    }

    public void Hide()
    {
        if (_spawner.Background) _spriteBase.enabled = true;
        _spriteRenderer.enabled = false;
    }
}
