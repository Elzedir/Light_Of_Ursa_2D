using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Wall { Top, Bottom, Left, Right }

public class Cell : MonoBehaviour
{
    public Node Node { get; private set; }
    public Coordinates Coordinates { get; private set; }

    SpriteRenderer _spriteBase;
    SpriteRenderer _spriteRenderer;
    BoxCollider2D _colliderTop;
    BoxCollider2D _colliderBottom;
    BoxCollider2D _colliderLeft;
    BoxCollider2D _colliderRight;

    public bool Visited;
    bool _initialised = false;

    public Dictionary<Wall, bool> Sides { get; private set; }

    Spawner_Maze _spawner;
    
    public void InitialiseCell(Coordinates coordinates, Spawner_Maze spawner)
    {
        Coordinates = coordinates;
        _spawner = spawner;

        _spriteBase = gameObject.AddComponent<SpriteRenderer>();
        if (_spawner.Background) _spriteBase.sprite = Resources.Load<Sprite>("Sprites/White ground");

        _spriteRenderer = new GameObject("CellSprite").AddComponent<SpriteRenderer>();
        _spriteRenderer.transform.parent = transform;
        _spriteRenderer.transform.localPosition = Vector3.zero;

        BoxCollider2D coll = gameObject.AddComponent<BoxCollider2D>();
        coll.size = new Vector2(0.9f, 0.9f);
        coll.isTrigger = true;

        _colliderTop = CreateSideCollider(Wall.Top);
        _colliderBottom = CreateSideCollider(Wall.Bottom);
        _colliderLeft = CreateSideCollider(Wall.Left);
        _colliderRight = CreateSideCollider(Wall.Right);

        GameObject textGO = new GameObject();
        textGO.transform.parent = transform;
        textGO.transform.localPosition = Vector3.zero;
        TextMeshPro text = textGO.AddComponent<TextMeshPro>();
        text.text = $"{Coordinates.X}_{Coordinates.Y}";
        text.alignment = TextAlignmentOptions.Center;
        text.sortingLayerID = -967159649;
        text.fontSize = 3; text.color = Color.black;

    }

    BoxCollider2D CreateSideCollider(Wall wall)
    {
        BoxCollider2D collider = new GameObject($"Collider{wall}").AddComponent<BoxCollider2D>();
        collider.transform.parent = transform;
        switch(wall)
        {
            case Wall.Top: collider.transform.localPosition = new Vector3(0, 0.5f, 0); collider.size = new Vector2(1, 0.12f); break;
            case Wall.Bottom: collider.transform.localPosition = new Vector3(0, -0.5f, 0); collider.size = new Vector2(1, 0.12f); break;
            case Wall.Left: collider.transform.localPosition = new Vector3(-0.5f, 0, 0); collider.size = new Vector2(0.12f, 1); break;
            case Wall.Right: collider.transform.localPosition = new Vector3(0.5f, 0, 0); collider.size = new Vector2(0.12f, 1); break;
            default: break;
        }

        return collider;
    }

    public void ClearWall(Wall wall)
    {
        if (!_initialised)
        {
            Sides = new Dictionary<Wall, bool>
            {
                { Wall.Top, true },
                { Wall.Bottom, true },
                { Wall.Left, true },
                { Wall.Right, true }
            };

            _initialised = true;
        }

        switch (wall)
        {
            case Wall.Top: if (_colliderTop != null) Destroy(_colliderTop.gameObject); break; // Node.IsPassableTop = true; 
            case Wall.Bottom: if (_colliderBottom != null) Destroy(_colliderBottom.gameObject); break;// Node.IsPassableBottom = true;
            case Wall.Left: if (_colliderLeft != null) Destroy(_colliderLeft.gameObject); break; // Node.IsPassableLeft = true;
            case Wall.Right: if (_colliderRight != null) Destroy(_colliderRight.gameObject); break; //Node.IsPassableRight = true;
            default: break;
        }

        Sides[wall] = false;

        Sprite sprite = null;

        if (Sides[Wall.Top] && Sides[Wall.Bottom] && Sides[Wall.Left] && Sides[Wall.Right]) sprite = Resources.Load<Sprite>("Sprites/Grid");
        
        if (!Sides[Wall.Top] && Sides[Wall.Bottom] && Sides[Wall.Left] && Sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_OpenOneSide"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 270); }
        if (Sides[Wall.Top] && !Sides[Wall.Bottom] && Sides[Wall.Left] && Sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_OpenOneSide"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90); }
        if (Sides[Wall.Top] && Sides[Wall.Bottom] && !Sides[Wall.Left] && Sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_OpenOneSide"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 0); }
        if (Sides[Wall.Top] && Sides[Wall.Bottom] && Sides[Wall.Left] && !Sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_OpenOneSide"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 180); }

        if (!Sides[Wall.Top] && !Sides[Wall.Bottom] && Sides[Wall.Left] && Sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_OpenTwoSides"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90); }
        if (Sides[Wall.Top] && Sides[Wall.Bottom] && !Sides[Wall.Left] && !Sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_OpenTwoSides"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 0); }

        if (!Sides[Wall.Top] && Sides[Wall.Bottom] && !Sides[Wall.Left] && Sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_Corner"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90); }
        if (!Sides[Wall.Top] && Sides[Wall.Bottom] && Sides[Wall.Left] && !Sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_Corner"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 0); }
        if (Sides[Wall.Top] && !Sides[Wall.Bottom] && !Sides[Wall.Left] && Sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_Corner"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 180); }
        if (Sides[Wall.Top] && !Sides[Wall.Bottom] && Sides[Wall.Left] && !Sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_Corner"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 270); }
        
        if (!Sides[Wall.Top] && !Sides[Wall.Bottom] && !Sides[Wall.Left] && Sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_OpenThreeSides"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 180); }
        if (!Sides[Wall.Top] && !Sides[Wall.Bottom] && Sides[Wall.Left] && !Sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_OpenThreeSides"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 0); }
        if (!Sides[Wall.Top] && Sides[Wall.Bottom] && !Sides[Wall.Left] && !Sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_OpenThreeSides"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90); }
        if (Sides[Wall.Top] && !Sides[Wall.Bottom] && !Sides[Wall.Left] && !Sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_OpenThreeSides"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 270); }

        if (!Sides[Wall.Top] && !Sides[Wall.Bottom] && !Sides[Wall.Left] && !Sides[Wall.Right]) 
        { sprite = Resources.Load<Sprite>("Sprites/Grid_OpenAllSides"); _spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 0); }

        if (sprite != null) _spriteRenderer.sprite = sprite;
        else Debug.Log("Sprite not found.");
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "Focus") _spawner.RefreshMaze(this);
        else if (collider.TryGetComponent<Chaser>(out Chaser chaser)) chaser.CurrentCell = this;
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

    public void MarkCell(Color color)
    {
        _spriteRenderer.color = color;
    }
}
