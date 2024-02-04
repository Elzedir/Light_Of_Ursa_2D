using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Key : MonoBehaviour
{
    MouseMazeColour _mouseMazeKeyColour;
    Color _keyColor;
    BoxCollider2D _collider;
    SpriteRenderer _spriteRenderer;

    public void InitialiseDoorKey(MouseMazeColour doorColour, Color color)
    {
        _mouseMazeKeyColour = doorColour;
        _keyColor = color;

        _collider = gameObject.AddComponent<BoxCollider2D>();
        _collider.size = new Vector3(0.9f, 0.9f, 0.9f);
        _collider.isTrigger = true;

        _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        _spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Blue ground");
        _spriteRenderer.sortingLayerName = "Actors";
        _spriteRenderer.color = color;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name != "Focus") return;

        collision.gameObject.GetComponent<Controller_Puzzle_MouseMaze>().SetPlayerColour(_mouseMazeKeyColour, _keyColor);
    }
}
