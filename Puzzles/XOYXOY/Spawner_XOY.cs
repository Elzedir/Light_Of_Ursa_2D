using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner_XOY : MonoBehaviour
{
    int _numberOfXoys;
    public Sprite[] XoySprites { get; private set; }
    List<XOY> _xoys = new();

    public Transform XParent {  get; private set; }
    public Transform OParent {  get; private set; }
    public Transform YParent {  get; private set; }

    [SerializeField] int rows = 10;
    [SerializeField] int columns = 10;
    [SerializeField] float spacing = 1.0f;

    void Start()
    {
        Sprite X = Resources.Load<Sprite>("Sprites/X");
        Sprite O = Resources.Load<Sprite>("Sprites/O");
        Sprite Y = Resources.Load<Sprite>("Sprites/Y");

        XoySprites = new Sprite[] { X, O, Y };

        XParent = Manager_Game.Instance.FindTransformRecursively(transform, "X");
        OParent = Manager_Game.Instance.FindTransformRecursively(transform, "O");
        YParent = Manager_Game.Instance.FindTransformRecursively(transform, "Y");

        InitialisePuzzle();
    }

    void InitialisePuzzle()
    {
        if (Manager_Puzzle.Instance.Puzzle.PuzzleData.PuzzleState.PuzzleType == PuzzleType.Fixed) SpawnFixedPuzzle();
        else SpawnRandomPuzzle();
    }
    
    void SpawnFixedPuzzle()
    {

    }

    void SpawnRandomPuzzle()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                SpawnXoy((transform.position - new Vector3((columns - 1) * spacing / 2, (rows - 1) * spacing / 2, 0)) + new Vector3(column * spacing, row * spacing, 0));
            }
        }
    }

    void SpawnXoy(Vector3 position, int xoyIndex = -1)
    {
        GameObject xoyGO = new GameObject($"Piece{_numberOfXoys}"); _numberOfXoys++;

        xoyGO.transform.position = position;

        SpriteRenderer xoySprite = xoyGO.AddComponent<SpriteRenderer>();
        xoyIndex = xoyIndex != -1 ? xoyIndex : Random.Range(0, XoySprites.Length);
        xoySprite.sprite = XoySprites[xoyIndex];
        xoySprite.sortingLayerName = "Actors";

        XOY xoy = xoyGO.AddComponent<XOY>(); xoy.CurrentSpriteIndex = xoyIndex; xoy.Spawner = this;

        xoyGO.AddComponent<BoxCollider2D>().size = new Vector2(0.9f, 0.9f);

        switch(xoyIndex)
        {
            case 0:
                xoyGO.transform.parent = XParent;
                break;
            case 1:
                xoyGO.transform.parent = OParent;
                break;
            case 2:
                xoyGO.transform.parent = YParent;
                break;
        }
    }
}
