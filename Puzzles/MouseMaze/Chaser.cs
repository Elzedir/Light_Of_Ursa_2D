using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : MonoBehaviour
{
    public Cell CurrentCell;
    public Node StartNode;
    public Node TargetNode;

    public void BlowUp()
    {
        Destroy(gameObject);
    }
}
