using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Vector3 Move;
    public float Speed = 1;
    public Transform Target;
    void Update()
    {
        if (Move != Vector3.zero) transform.position += (Move.normalized * Speed * Time.deltaTime);

        if (Target != null)
        {
            Vector2 direction = (Target.position - transform.position).normalized;
            transform.position += new Vector3(direction.x, direction.y, 0) * Speed * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f);
        }
    }

    public void DestroyArrow()
    {
        Destroy(gameObject);
    }
}
