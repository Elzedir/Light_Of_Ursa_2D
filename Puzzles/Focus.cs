using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Focus : MonoBehaviour
{
    BoxCollider2D _collider2D;

    void Awake()
    {
        _collider2D = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Arrow>(out Arrow arrow))
        {
            arrow.DestroyArrow();

            if (!Manager_Puzzle.Instance.Invulnerable) Manager_Puzzle.Instance.TakeDamage();
        }
    }
}
