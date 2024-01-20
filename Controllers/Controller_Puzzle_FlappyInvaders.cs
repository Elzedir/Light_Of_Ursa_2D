using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Controller_Puzzle_FlappyInvaders : Controller
{
    Transform _bulletParent;
    void Start()
    {
        KeyBindings.ContinuousPressKeyActions.Remove(ActionKey.Space);
        KeyBindings.SinglePressKeyActions.Add(ActionKey.Space, HandleSpacePressed);
        _bulletParent = GameObject.Find("BulletParent").transform;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Mine>() || collision.gameObject.GetComponent<Bullet>()) Hit(collision);
    }

    public override void HandleWPressed()
    {
        transform.position += (Vector3.up * 0.01f);
    }

    public override void HandleSPressed()
    {
        transform.position += (Vector3.down * 0.01f);
    }

    public override void HandleAPressed()
    {
        transform.position += (Vector3.left * 0.01f);
    }

    public override void HandleDPressed()
    {
        transform.position += (Vector3.right * 0.01f);
    }

    public override void HandleSpacePressed()
    {
        Shoot();
    }

    void Shoot()
    {
        GameObject bulletGO = new GameObject("Bullet");
        Bullet bullet = bulletGO.AddComponent<Bullet>();
        bulletGO.transform.parent = _bulletParent;
        bullet.Initialise(Manager_Puzzle.Instance.BulletSprite, Vector3.right, transform.position);
    }

    void Hit(Collision2D collision)
    {
        Debug.Log("Hit");
    }
}
