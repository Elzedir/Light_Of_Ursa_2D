using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder_Base
{
    PriorityQueue _mainPriorityQueue;
    double _priorityModifier;
    Node_Base _targetNode;
    Node_Base _startNode;
    PuzzleSet _puzzleSet;

    #region Initialisation
    public void RunPathfinderOpenWorld(Coordinates start, Coordinates target, PathfinderMover mover)
    {
        RunPathfinder(Manager_Grid.Rows, Manager_Grid.Columns, new Coordinates(start.X + Manager_Grid.XOffset, start.Y + Manager_Grid.YOffset), new Coordinates(target.X + Manager_Grid.XOffset, target.Y + Manager_Grid.YOffset), mover, PuzzleSet.None);
    }
    public void RunPathfinder(int rows, int columns, Coordinates start, Coordinates target, PathfinderMover mover, PuzzleSet puzzleSet)
    {
        if (start.Equals(target)) return;

        _puzzleSet = puzzleSet;
        _startNode = new Node_Base();
        _startNode.X = start.X;
        _startNode.Y = start.Y;
        _targetNode = new Node_Base();
        _targetNode.X = target.X;
        _targetNode.Y = target.Y;

        Node_Base lastNode = _startNode;

        _initialise(rows, columns);

        _computeShortestPath();

        int infinity = 0;

        while (!_startNode.Equals(_targetNode) && infinity < 1000)
        {
            _startNode = _minimumSuccessorNode(_startNode);
            LinkedList<Coordinates> obstacleCoordinates = mover.GetObstaclesInVision();
            double oldPriorityModifier = _priorityModifier;
            Node_Base oldLastNode = lastNode;
            _priorityModifier += _manhattanDistance(_startNode, lastNode);
            lastNode = _startNode;
            bool change = false;

            foreach (Coordinates coordinate in obstacleCoordinates)
            {
                Node_Base node = NodeArray.Nodes[coordinate.X, coordinate.Y];
                if (node.IsObstacle) continue;
                change = true;
                node.IsObstacle = true;
                foreach (Node_Base p in node.GetPredecessors(NodeArray.Nodes))
                {
                    _updateVertex(p);
                }
            }
            if (!change)
            {
                _priorityModifier = oldPriorityModifier;
                lastNode = oldLastNode;
            }
            _computeShortestPath();

            infinity++;
        }

        mover.MoveTo(_targetNode);
    }

    void _initialise(int rows, int columns)
    {
        foreach (Node_Base node in NodeArray.Nodes){ node.RHS = double.PositiveInfinity; node.G = double.PositiveInfinity; }
        _mainPriorityQueue = new PriorityQueue(rows * columns);
        _priorityModifier = 0;
        _targetNode = NodeArray.Nodes[_targetNode.X, _targetNode.Y];
        _startNode = NodeArray.Nodes[_startNode.X, _startNode.Y];
        _targetNode.RHS = 0;
        _mainPriorityQueue.Enqueue(_targetNode, _calculatePriority(_targetNode));
    }

    Priority _calculatePriority(Node_Base node)
    {
        return new Priority(Math.Min(node.G, node.RHS) + _manhattanDistance(node, _startNode) + _priorityModifier, Math.Min(node.G, node.RHS));
    }
    double _manhattanDistance(Node_Base a, Node_Base b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }
    
    void _updateVertex(Node_Base node)
    {
        if (!node.Equals(_targetNode))
        {
            node.RHS = _minimumSuccessorCost(node);
        }
        if (_mainPriorityQueue.Contains(node))
        {
            _mainPriorityQueue.Remove(node);
        }
        if (node.G != node.RHS)
        {
            _mainPriorityQueue.Enqueue(node, _calculatePriority(node));
        }
    }
    Node_Base _minimumSuccessorNode(Node_Base node)
    {
        double minimumCostToMove = Double.PositiveInfinity;
        Node_Base newNode = null;
        foreach (Node_Base successor in node.GetSuccessors(NodeArray.Nodes))
        {
            double costToMove = node.GetMovementCostTo(successor, _puzzleSet) + successor.G;

            if (costToMove <= minimumCostToMove && !successor.IsObstacle)
            {
                minimumCostToMove = costToMove;
                newNode = successor;
            }
        }

        newNode.Predecessor = node;

        return newNode;
    }
    double _minimumSuccessorCost(Node_Base node)
    {
        double minimumCost = Double.PositiveInfinity;
        foreach (Node_Base successor in node.GetSuccessors(NodeArray.Nodes))
        {
            double costToMove = node.GetMovementCostTo(successor, _puzzleSet) + successor.G;
            if (costToMove < minimumCost && !successor.IsObstacle) minimumCost = costToMove;
        }
        return minimumCost;
    }
    void _computeShortestPath()
    {
        while (_mainPriorityQueue.Peek().CompareTo(_calculatePriority(_startNode)) < 0 || _startNode.RHS != _startNode.G)
        {
            Priority highestPriority = _mainPriorityQueue.Peek();
            Node_Base node = _mainPriorityQueue.Dequeue();
            if (node == null) break;

            if (highestPriority.CompareTo(_calculatePriority(node)) < 0)
            {
                _mainPriorityQueue.Enqueue(node, _calculatePriority(node));
            }
            else if (node.G > node.RHS)
            {
                node.G = node.RHS;
                foreach (Node_Base neighbour in node.GetPredecessors(NodeArray.Nodes))
                {
                    _updateVertex(neighbour);
                }
            }
            else
            {
                node.G = Double.PositiveInfinity;
                _updateVertex(node);
                foreach (Node_Base neighbour in node.GetPredecessors(NodeArray.Nodes))
                {
                    _updateVertex(neighbour);
                }
            }
        }
    }
    #endregion

    public void UpdateWallState(Node_Base node, Direction direction, bool wallExists)
    {
        double newCost = wallExists ? Double.PositiveInfinity : 1;

        node.UpdateMovementCost(direction, newCost);

        Node_Base neighborNode = GetNeighbor(node, direction);

        Direction oppositeDirection = GetOppositeDirection(direction);
        neighborNode.UpdateMovementCost(oppositeDirection, newCost);
    }

    Node_Base GetNeighbor(Node_Base node, Direction direction)
    {
        switch (direction)
        {
            case Direction.Top: return node.Y + 1 < NodeArray.Nodes.GetLength(1) ? NodeArray.Nodes[node.X, node.Y + 1] : null;
            case Direction.Bottom: return node.Y - 1 >= 0 ? NodeArray.Nodes[node.X, node.Y - 1] : null;
            case Direction.Left: return node.X - 1 >= 0 ? NodeArray.Nodes[node.X - 1, node.Y] : null;
            case Direction.Right: return node.X + 1 < NodeArray.Nodes.GetLength(0) ? NodeArray.Nodes[node.X + 1, node.Y] : null;
            default: return null;
        }
    }

    Direction GetOppositeDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Top: return Direction.Bottom;
            case Direction.Bottom: return Direction.Top;
            case Direction.Left: return Direction.Right;
            case Direction.Right: return Direction.Left;
            default: throw new InvalidOperationException("Invalid wall direction.");
        }
    }

    public static Node_Base GetNodeAtPosition(int x, int y)
    {
        if (NodeArray.Nodes[x, y] != null) return NodeArray.Nodes[x, y];
        return null;
    }

    public List<Coordinates> RetrievePath(Node_Base startNode, Node_Base targetNode)
    {
        List<Coordinates> path = new List<Coordinates>();
        Node_Base currentNode = targetNode;

        while (currentNode != null && !currentNode.Equals(startNode))
        {
            if (currentNode == null || currentNode.Equals(startNode)) break;

            path.Add(new Coordinates(currentNode.X, currentNode.Y));
            currentNode = currentNode.Predecessor;
        }

        if (currentNode != null)
        {
            path.Add(new Coordinates(startNode.X, startNode.Y));
        }

        path.Reverse();

        return path;
    }

    public static void FindAllPredecessors(Node_Base node, int infiniteEnd, int infinityStart = 0)
    {
        infinityStart++;
        if (infinityStart > infiniteEnd) return;
        if (node.Predecessor == null) { Debug.Log($"{node.X}_{node.Y} predecessor is null"); return; }

        Debug.Log($"{node.X}_{node.Y} -> {node.Predecessor.X}_{node.Predecessor.Y}");
        FindAllPredecessors(node.Predecessor, infinityStart, infiniteEnd);
    }
}

public class NodeArray
{
    public static Node_Base[,] Nodes;

    public static Node_Base[,] InitializeArray(int rows, int columns)
    {
        Nodes = new Node_Base[rows, columns];

        for (int row = 0; row < Nodes.GetLength(0); row++)
        {
            for (int column = 0; column < Nodes.GetLength(1); column++)
            {
                Nodes[row, column] = new Node_Base();
                Nodes[row, column].X = row;
                Nodes[row, column].Y = column;
                Nodes[row, column].G = Double.PositiveInfinity;
                Nodes[row, column].RHS = Double.PositiveInfinity;
            }
        }

        return Nodes;
    }
}

public enum Direction { None, Top, Bottom, Left, Right }

public class Node_Base
{
    public int X;
    public int Y;
    public double G;
    public double RHS;
    private Node_Base _predecessor;
    public Node_Base Predecessor { get; set; }

    public double MovementCost;
    public Dictionary<Direction, double> MovementCosts { get; private set; }

    public bool IsObstacle;

    public Node_Base()
    {
        MovementCosts = new Dictionary<Direction, double>
        {
            { Direction.Top, Double.PositiveInfinity },
            { Direction.Bottom, Double.PositiveInfinity },
            { Direction.Left, Double.PositiveInfinity },
            { Direction.Right, Double.PositiveInfinity }
        };
    }

    public void UpdateMovementCost(Direction direction, double cost)
    {
        if (direction == Direction.None) MovementCost = cost;
        else MovementCosts[direction] = cost;
    }

    public double GetMovementCostTo(Node_Base successor, PuzzleSet puzzleSet)
    {
        if (puzzleSet == PuzzleSet.MouseMaze)
        {
            Direction directionToSuccessor = Direction.None;

            if (X == successor.X)
            {
                if (Y == successor.Y - 1) directionToSuccessor = Direction.Top;
                else if (Y == successor.Y + 1) directionToSuccessor = Direction.Bottom;
                else throw new InvalidOperationException("Nodes are not X adjacent.");
            }
            else if (Y == successor.Y)
            {
                if (X == successor.X - 1) directionToSuccessor = Direction.Right;
                else if (X == successor.X + 1) directionToSuccessor = Direction.Left;
                else throw new InvalidOperationException("Nodes are not Y adjacent.");
            }
            else throw new InvalidOperationException("Nodes are not adjacent.");

            if (!MovementCosts.TryGetValue(directionToSuccessor, out double cost) || directionToSuccessor == Direction.None) throw new InvalidOperationException("Invalid direction.");

            return cost;
        }
        else if (puzzleSet == PuzzleSet.IceWall || puzzleSet == PuzzleSet.None)
        {
            return successor.MovementCost;
        }

        throw new InvalidOperationException($"{puzzleSet} not valid.");
    }

    public bool Equals(Node_Base that)
    {
        if (X == that.X && Y == that.Y) return true;
        return false;
    }

    public LinkedList<Node_Base> GetSuccessors(Node_Base[,] nodes)
    {
        LinkedList<Node_Base> successors = new LinkedList<Node_Base>();
        if (X + 1 < nodes.GetLength(0)) successors.AddFirst(nodes[X + 1, Y]);
        if (Y + 1 < nodes.GetLength(1)) successors.AddFirst(nodes[X, Y + 1]);
        if (X - 1 >= 0) successors.AddFirst(nodes[X - 1, Y]);
        if (Y - 1 >= 0) successors.AddFirst(nodes[X, Y - 1]);
        return successors;
    }

    public LinkedList<Node_Base> GetPredecessors(Node_Base[,] nodes)
    {
        LinkedList<Node_Base> neighbours = new LinkedList<Node_Base>();
        Node_Base tempNode;
        if (X + 1 < nodes.GetLength(0))
        {
            tempNode = nodes[X + 1, Y];
            if (!tempNode.IsObstacle) neighbours.AddFirst(tempNode);
        }
        if (Y + 1 < nodes.GetLength(1))
        {
            tempNode = nodes[X, Y + 1];
            if (!tempNode.IsObstacle) neighbours.AddFirst(tempNode);
        }
        if (X - 1 >= 0)
        {
            tempNode = nodes[X - 1, Y];
            if (!tempNode.IsObstacle) neighbours.AddFirst(tempNode);
        }
        if (Y - 1 >= 0)
        {
            tempNode = nodes[X, Y - 1];
            if (!tempNode.IsObstacle) neighbours.AddFirst(tempNode);
        }
        return neighbours;
    }
}

public class Priority
{
    public double PrimaryPriority;
    public double SecondaryPriority;

    public Priority(double primaryPriority, double secondaryPriority)
    {
        PrimaryPriority = primaryPriority;
        SecondaryPriority = secondaryPriority;
    }
    public int CompareTo(Priority that)
    {
        if (PrimaryPriority < that.PrimaryPriority) return -1;
        else if (PrimaryPriority > that.PrimaryPriority) return 1;
        if (SecondaryPriority > that.SecondaryPriority) return 1;
        else if (SecondaryPriority < that.SecondaryPriority) return -1;
        return 0;
    }
}

public class NodeQueue
{
    public Node_Base Node;
    public Priority Priority;

    public NodeQueue(Node_Base node, Priority priority)
    {
        Node = node;
        Priority = priority;
    }
}

public class PriorityQueue
{
    int _currentPosition;
    NodeQueue[] _nodeQueue;
    Dictionary<Node_Base, int> _priorityQueue;

    public PriorityQueue(int maxNodes)
    {
        _currentPosition = 0;
        _nodeQueue = new NodeQueue[maxNodes];
        _priorityQueue = new Dictionary<Node_Base, int>();
    }

    public Priority Peek()
    {
        if (_currentPosition == 0) return new Priority(Double.PositiveInfinity, Double.PositiveInfinity);

        return _nodeQueue[1].Priority;
    }

    public Node_Base Dequeue()
    {
        if (_currentPosition == 0) return null;

        Node_Base node = _nodeQueue[1].Node;
        _nodeQueue[1] = _nodeQueue[_currentPosition];
        _priorityQueue[_nodeQueue[1].Node] = 1;
        _priorityQueue[node] = 0;
        _currentPosition--;
        _moveDown(1);
        return node;
    }

    public void Enqueue(Node_Base node, Priority priority)
    {
        NodeQueue nodeQueue = new NodeQueue(node, priority);
        _currentPosition++;
        _priorityQueue[node] = _currentPosition;
        if (_currentPosition == _nodeQueue.Length) Array.Resize<NodeQueue>(ref _nodeQueue, _nodeQueue.Length * 2);
        _nodeQueue[_currentPosition] = nodeQueue;
        _moveUp(_currentPosition);
    }

    public void Update(Node_Base node, Priority priority)
    {
        int index = _priorityQueue[node];
        if (index == 0) return;
        Priority priorityOld = _nodeQueue[index].Priority;
        _nodeQueue[index].Priority = priority;
        if (priorityOld.CompareTo(priority) < 0)
        {
            _moveDown(index);
        }
        else
        {
            _moveUp(index);
        }
    }

    public void Remove(Node_Base node)
    {
        int index = _priorityQueue[node];

        if (index == 0) return;

        _priorityQueue[node] = 0;
        _nodeQueue[index] = _nodeQueue[_currentPosition];
        _priorityQueue[_nodeQueue[index].Node] = index;
        _currentPosition--;
        _moveDown(index);
    }

    public bool Contains(Node_Base node)
    {
        int index;
        if (!_priorityQueue.TryGetValue(node, out index))
        {
            return false;
        }
        return index != 0;
    }

    void _moveDown(int index)
    {
        int childL = index * 2;
        if (childL > _currentPosition) return;
        int childR = index * 2 + 1;
        int smallerChild;
        if (childR > _currentPosition)
        {
            smallerChild = childL;
        }
        else if (_nodeQueue[childL].Priority.CompareTo(_nodeQueue[childR].Priority) < 0)
        {
            smallerChild = childL;
        }
        else
        {
            smallerChild = childR;
        }
        if (_nodeQueue[index].Priority.CompareTo(_nodeQueue[smallerChild].Priority) > 0)
        {
            _swap(index, smallerChild);
            _moveDown(smallerChild);
        }
    }

    void _moveUp(int index)
    {
        if (index == 1) return;
        int parent = index / 2;
        if (_nodeQueue[parent].Priority.CompareTo(_nodeQueue[index].Priority) > 0)
        {
            _swap(parent, index);
            _moveUp(parent);
        }
    }

    void _swap(int indexA, int indexB)
    {
        NodeQueue tempQueue = _nodeQueue[indexA];
        _nodeQueue[indexA] = _nodeQueue[indexB];
        _priorityQueue[_nodeQueue[indexB].Node] = indexA;
        _nodeQueue[indexB] = tempQueue;
        _priorityQueue[tempQueue.Node] = indexB;
    }
}

public class Coordinates
{
    public int X;
    public int Y;

    public Coordinates(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public interface PathfinderMover
{
    void MoveTo(Node_Base target);
    LinkedList<Coordinates> GetObstaclesInVision();
}

