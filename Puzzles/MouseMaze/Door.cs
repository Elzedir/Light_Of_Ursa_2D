using UnityEngine;

public class Door : MonoBehaviour
{
    Door_Base _doorBase;
    BoxCollider2D _collider;

    public void InitialiseDoor(Door_Base doorBase, Color color)
    {
        _doorBase = doorBase;
        _collider = gameObject.AddComponent<BoxCollider2D>();
        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/X");
        spriteRenderer.sortingLayerName = "Actors";
        spriteRenderer.color = color;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name != "Focus") return;

        collision.gameObject.TryGetComponent<Controller_Puzzle_MouseMaze>(out Controller_Puzzle_MouseMaze player);
        if (player.PlayerColour != _doorBase.MouseMazeDoorColour) _collider.isTrigger = false;
        else _collider.isTrigger = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name != "Focus") return;

        collision.gameObject.TryGetComponent<Controller_Puzzle_MouseMaze>(out Controller_Puzzle_MouseMaze player);
        if (player.PlayerColour == _doorBase.MouseMazeDoorColour) _collider.isTrigger = true;
        else _collider.isTrigger = false;
    }
}
