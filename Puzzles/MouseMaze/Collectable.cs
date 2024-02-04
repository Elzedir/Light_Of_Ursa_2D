using UnityEngine;

public class Collectable : MonoBehaviour
{
    Spawner_Maze _spawner;
    BoxCollider2D _collider;
    SpriteRenderer _spriteRenderer;

    public void SpawnCollectable(Spawner_Maze spawner)
    {
        _spawner = spawner;

        _collider = gameObject.AddComponent<BoxCollider2D>();
        _collider.isTrigger = true; 
        _collider.size = new Vector2(0.4f, 0.4f);

        _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        _spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/X");
        _spriteRenderer.sortingLayerName = "Actors";

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name != "Focus") return;
        _spawner.CollectableCollected(this);
    }
}
