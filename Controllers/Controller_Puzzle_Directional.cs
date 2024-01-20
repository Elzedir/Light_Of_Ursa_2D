using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Controller_Puzzle_Directional : Controller
{
    BoxCollider2D _collider;
    PuzzleSet _puzzleSet;
    PuzzleType _puzzleType;
    Transform Target;

    public float shieldRadius = 1.5f;

    void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _puzzleSet = Manager_Puzzle.Instance.Puzzle.PuzzleData.PuzzleSet;
        _puzzleType = Manager_Puzzle.Instance.Puzzle.PuzzleData.PuzzleType;
        Target = GameObject.Find("Focus").transform;
    }

    protected override void Update()
    {
        if (_puzzleSet == PuzzleSet.AntiDirectional)
        {
            MoveShield();
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (Manager_Puzzle.Instance.Puzzle.PuzzleData.PuzzleSet == PuzzleSet.Directional)
        {
            int zAngle = Mathf.RoundToInt(collision.transform.localRotation.eulerAngles.z) % 360;
            if (zAngle < 0) zAngle += 360;

            if (zAngle == 0 && (Input.GetKey(KeyCode.UpArrow) || Input.GetKeyDown(KeyBindings.Keys[ActionKey.Move_Up])) &&
                !(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow)))
                collision.gameObject.GetComponent<Arrow>().DestroyArrow();
            else if (zAngle == 90 && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKeyDown(KeyBindings.Keys[ActionKey.Move_Left])) &&
                !(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow)))
                collision.gameObject.GetComponent<Arrow>().DestroyArrow();
            else if (zAngle == 180 && (Input.GetKey(KeyCode.DownArrow) || Input.GetKeyDown(KeyBindings.Keys[ActionKey.Move_Down])) &&
                !(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.RightArrow)))
                collision.gameObject.GetComponent<Arrow>().DestroyArrow();
            else if (zAngle == 270 && (Input.GetKey(KeyCode.RightArrow) || Input.GetKeyDown(KeyBindings.Keys[ActionKey.Move_Right])) &&
                !(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow)))
                collision.gameObject.GetComponent<Arrow>().DestroyArrow();
        }

        else if (Manager_Puzzle.Instance.Puzzle.PuzzleData.PuzzleSet == PuzzleSet.AntiDirectional && collision.gameObject.name != "Focus") collision.gameObject.GetComponent<Arrow>().DestroyArrow();
    }

    void MoveShield()
    {
        Vector3 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - Target.position).normalized;
        transform.position = Target.position + direction * shieldRadius;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
