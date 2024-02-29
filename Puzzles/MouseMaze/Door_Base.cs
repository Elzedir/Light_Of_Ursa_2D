using System.Collections.Generic;
using UnityEngine;

public class Door_Base : MonoBehaviour
{
    public MouseMazeColour MouseMazeDoorColour { get; private set; }
    public Color DoorColor { get; private set; }

    GameObject _topCollider;
    GameObject _bottomCollider;
    GameObject _leftCollider;
    GameObject _rightCollider;

    public void InitialiseDoor(MouseMazeColour doorColour, Color color, Cell_MouseMaze cell)
    {
        MouseMazeDoorColour = doorColour;
        DoorColor = color;

        foreach (KeyValuePair<Wall, bool> wall in cell.Sides)
        {
            if (wall.Value) continue;

            switch (wall.Key)
            {
                case Wall.Top: _topCollider = CreateDoor("Top"); _topCollider.transform.localPosition = new Vector3(cell.transform.position.x, cell.transform.position.y + 0.5f, 0); _topCollider.GetComponent<BoxCollider2D>().size = new Vector2(1, 0.12f); break;
                case Wall.Bottom: _bottomCollider = CreateDoor("Bottom"); _bottomCollider.transform.localPosition = new Vector3(cell.transform.position.x, cell.transform.position.y - 0.5f, 0); _bottomCollider.GetComponent<BoxCollider2D>().size = new Vector2(1, 0.12f); break;
                case Wall.Left: _leftCollider = CreateDoor("Left"); _leftCollider.transform.localPosition = new Vector3(cell.transform.position.x - 0.5f, cell.transform.position.y, 0); _leftCollider.GetComponent<BoxCollider2D>().size = new Vector2(0.12f, 1); break;
                case Wall.Right: _rightCollider = CreateDoor("Right"); _rightCollider.transform.localPosition = new Vector3(cell.transform.position.x + 0.5f, cell.transform.position.y, 0); _rightCollider.GetComponent<BoxCollider2D>().size = new Vector2(0.12f, 1); break;
                default: break;
            }
        }
    }

    GameObject CreateDoor(string name)
    {
        GameObject doorGO = new GameObject($"Door_{name}");
        doorGO.transform.parent = transform;
        Door door = doorGO.AddComponent<Door>();
        door.InitialiseDoor(this, DoorColor);
        return doorGO;
    }
}
