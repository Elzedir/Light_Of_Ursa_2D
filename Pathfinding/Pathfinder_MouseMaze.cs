using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder_MouseMaze : Pathfinder_Base
{
    public Pathfinder_MouseMaze(int rows, int columns, Node startNode, Node targetNode)
    : base(rows, columns, startNode, targetNode) 
    {
        
    }

    protected override double GetCost(Node from, Node to, Cell[,] cells)
    {
        if (from.X + 1 == to.X && (!cells[from.X, from.Y].Sides[Wall.Right] || !cells[to.X, to.Y].Sides[Wall.Left])) return double.MaxValue;
        if (from.X - 1 == to.X && (!cells[from.X, from.Y].Sides[Wall.Left] || !cells[to.X, to.Y].Sides[Wall.Right])) return double.MaxValue;
        if (from.Y + 1 == to.Y && (!cells[from.X, from.Y].Sides[Wall.Top] || !cells[to.X, to.Y].Sides[Wall.Bottom])) return double.MaxValue;
        if (from.Y - 1 == to.Y && (!cells[from.X, from.Y].Sides[Wall.Bottom] || !cells[to.X, to.Y].Sides[Wall.Top])) return double.MaxValue;

        return 1.0;
    }
}
