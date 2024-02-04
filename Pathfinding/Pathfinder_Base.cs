using System;
using System.Collections.Generic;

public class Pathfinder_Base
{
    protected Node[,] _nodes;
    protected Node _startNode;
    protected Node _targetNode;
    protected PriorityQueue _priorityQueue = new();

    public Pathfinder_Base(int rows, int columns, Node startNode, Node targetNode)
    {
        _nodes = new Node[rows, columns];
        _startNode = startNode;
        _targetNode = targetNode;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                _nodes[row, column] = new Node(row, column);
            }
        }

        _targetNode.RHS = 0;
        _priorityQueue.Enqueue(_targetNode.Priority(_startNode), _targetNode);
    }

    public List<Node> ComputeShortestPath()
    {
        while (_priorityQueue.Count > 0 && (_priorityQueue.Peek().Priority(_targetNode).Item1 < _startNode.Priority(_targetNode).Item1 || _startNode.RHS != _startNode.G))
        {
            Node node = _priorityQueue.Dequeue();
            if (node.G > node.RHS)
            {
                node.G = node.RHS;
                foreach (Node neighbor in GetNeighbors(node))
                {
                    UpdateVertex(neighbor);
                }
            }
            else
            {
                node.G = double.MaxValue;
                foreach (Node neighbor in GetNeighbors(node))
                {
                    UpdateVertex(neighbor);
                }
                UpdateVertex(node);
            }
        }

        return ReconstructPath();
    }

    protected void UpdateVertex(Node node)
    {
        if (node != _targetNode)
        {
            double minRHS = double.MaxValue;
            Node minRHSNeighbor = null;

            foreach (var neighbor in GetNeighbors(node))
            {
                double rhsValue = neighbor.G + GetCost(node, neighbor);
                if (rhsValue < minRHS)
                {
                    minRHS = rhsValue;
                    minRHSNeighbor = neighbor;
                }
            }
            node.RHS = minRHS;

            if (minRHSNeighbor != null)
            {
                node.Parent = minRHSNeighbor;
            }
        }

        _priorityQueue.Remove(node);

        if (node.G != node.RHS)
        {
            _priorityQueue.Enqueue(node.Priority(_targetNode), node);
        }
    }

    protected List<Node> ReconstructPath()
    {
        var path = new List<Node>();
        var currentNode = _targetNode;

        while (currentNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }

    protected IEnumerable<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        int[] dRow = { -1, 0, 1, 0 };
        int[] dCol = { 0, 1, 0, -1 };

        for (int i = 0; i < 4; i++)
        {
            int newRow = node.X + dRow[i];
            int newCol = node.Y + dCol[i];

            if (newRow >= 0 && newRow < _nodes.GetLength(0) && newCol >= 0 && newCol < _nodes.GetLength(1))
            {
                Node potentialNeighbor = _nodes[newRow, newCol];
                if (potentialNeighbor.IsPassable) neighbors.Add(potentialNeighbor);
            }
        }

        return neighbors;
    }

    protected virtual double GetCost(Node from, Node to, Cell[,] cells = null)
    {
        return 1.0;
    }

    public void HandlePathChange(Node changedNode, bool isNowPassable)
    {
        changedNode.IsPassable = isNowPassable;

        if (!isNowPassable)
        {
            changedNode.IsValid = false;
            changedNode.G = double.MaxValue;
            changedNode.RHS = double.MaxValue;
        }

        UpdateVertex(changedNode);

        foreach (Node neighbor in GetNeighbors(changedNode))
        {
            UpdateVertex(neighbor);
        }

        if (isNowPassable)ComputeShortestPath();
    }
}

public class Node : IComparable<Node>
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsPassable { get; set; } = true;
    public bool IsValid { get; set; } = true;
    public double G { get; set; } = double.MaxValue;
    public double RHS { get; set; } = double.MaxValue;
    public Node Parent { get; set; }

    public Node(int x, int y)
    {
        X = x;
        Y = y;
    }

    public (double, double) Priority(Node startNode)
    {
        return (Math.Min(G, RHS) + ManhattanDistance(this, startNode), Math.Min(G, RHS));
    }

    public int CompareTo(Node otherNode, Node goalNode)
    {
        var thisPriority = this.Priority(goalNode);
        var otherPriority = otherNode.Priority(goalNode);

        var primaryCompare = thisPriority.Item1.CompareTo(otherPriority.Item1);
        if (primaryCompare != 0) return primaryCompare;

        return thisPriority.Item2.CompareTo(otherPriority.Item2);
    }

    public int CompareTo(Node node)
    {
        return 0; // Redundant but necessary
    }

    public int ManhattanDistance(Node a, Node b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }
}

public class PriorityQueue
{
    SortedDictionary<(double, double), Queue<Node>> _priorityQueue = new();

    public void Enqueue((double, double) priority, Node node)
    {
        if (!_priorityQueue.ContainsKey(priority)) _priorityQueue.Add(priority, new Queue<Node>());

        _priorityQueue[priority].Enqueue(node);
    }

    public Node Dequeue()
    {
        while (_priorityQueue.Count > 0)
        {
            var enumerator = _priorityQueue.GetEnumerator();
            enumerator.MoveNext();
            var priority = enumerator.Current.Key;
            Queue<Node> nodes = enumerator.Current.Value;
            if (nodes.Count == 0) { _priorityQueue.Remove(priority); continue; }
            if (!nodes.Peek().IsValid) { nodes.Dequeue(); continue; }
            return nodes.Dequeue();
        }

        return null;
    }

    public Node Peek()
    {
        if (_priorityQueue.Count == 0) return null;

        var enumerator = _priorityQueue.GetEnumerator();
        enumerator.MoveNext();
        Queue<Node> nodes = enumerator.Current.Value;
        if (nodes.Count > 0) return nodes.Peek(); else return null;
    }

    public bool Remove(Node nodeToRemove)
    {
        foreach (var pair in _priorityQueue)
        {
            if (pair.Value.Contains(nodeToRemove))
            {
                nodeToRemove.IsValid = false;

                return true;
            }
        }
        return false;
    }

    public int Count
    {
        get
        {
            int count = 0;
            foreach (Queue<Node> queue in _priorityQueue.Values)
            {
                count += queue.Count;
            }
            return count;
        }
    }
}

