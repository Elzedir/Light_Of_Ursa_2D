using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 Move;
    public float Speed;

    public void Initialise(Sprite sprite, Vector3 move, Vector3 spawnPosition, float speed = 3)
    {
        transform.localScale = new Vector3(0.2f, 0.2f, 1);
        transform.position = new Vector3(spawnPosition.x + 0.5f, spawnPosition.y, spawnPosition.z);

        SpriteRenderer bulletSprite = gameObject.AddComponent<SpriteRenderer>();
        bulletSprite.sprite = sprite;

        gameObject.AddComponent<BoxCollider2D>();

        Move = move;
        Speed = speed;
    }

    void Update()
    {
        if (Move == Vector3.zero) Move = Vector3.right; transform.position += (Move.normalized * Speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Mine>() 
            || collision.gameObject.GetComponent<Column>() 
            || collision.gameObject.GetComponent<Controller>()
            || collision.gameObject.GetComponent<Bullet>()) 
            DestroyBullet();
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
