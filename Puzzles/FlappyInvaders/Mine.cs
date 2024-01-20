using UnityEngine;

public class Mine : MonoBehaviour
{
    Vector3 _move;
    float _speed;
    float _reloadTime;
    float _shootInterval = 3;

    Transform _bulletParent;
    
    public void Initialise(Transform bulletParent, Sprite sprite, float speed)
    {
        SpriteRenderer columnSprite = gameObject.AddComponent<SpriteRenderer>();
        columnSprite.sprite = sprite;
        columnSprite.sortingLayerName = "Actors";

        gameObject.AddComponent<BoxCollider2D>();

        Rigidbody2D columnBody = gameObject.AddComponent<Rigidbody2D>();
        columnBody.gravityScale = 0;
        columnBody.mass = 1000;

        _speed = speed;
        _bulletParent = bulletParent;
    }

    void Update()
    {
        if (_move == Vector3.zero) _move = Vector3.left; transform.position += (_move.normalized * _speed * Time.deltaTime);

        if (_reloadTime >= _shootInterval)
        {
            Shoot();

            _reloadTime = 0;
        }

        _reloadTime += Time.deltaTime;

        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("1"); if (collision.gameObject.GetComponent<Bullet>()) DestroyMine();
    }

    void Shoot()
    {
        GameObject bulletGO = new GameObject("Bullet");
        bulletGO.transform.parent = _bulletParent;
        Bullet bullet = bulletGO.AddComponent<Bullet>();
        bullet.Initialise(Manager_Puzzle.Instance.BulletSprite, Vector3.left, transform.position);
    }

    void DestroyMine()
    {
        Destroy(gameObject);
    }
}
