using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Column : MonoBehaviour
{
    public Vector3 Move;
    public float Speed;

    public void Initialise(float columnHeight, Transform spawner, Sprite sprite, float speed)
    {
        transform.parent = spawner.transform;
        transform.localPosition = new Vector3(0, spawner.localPosition.y + columnHeight / 2, 0);
        transform.localScale = new Vector3(1, columnHeight, 1);

        SpriteRenderer columnSprite = gameObject.AddComponent<SpriteRenderer>();
        columnSprite.sprite = sprite;
        columnSprite.sortingLayerName = "Actors";

        gameObject.AddComponent<BoxCollider2D>();

        Rigidbody2D columnBody = gameObject.AddComponent<Rigidbody2D>();
        columnBody.gravityScale = 0;
        columnBody.mass = 1000;

        Speed = speed;
    }

    void Update()
    {
        if (Move == Vector3.zero) Move = Vector3.left; transform.position += (Move.normalized * Speed * Time.deltaTime);
    }

    public void DestroyColumn()
    {
        Destroy(gameObject);
    }
}
