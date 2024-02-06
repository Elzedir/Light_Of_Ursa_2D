using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder_Base
{
    static PriorityQueue _mainPriorityQueue;
    static Node[,] _nodes;
    static double _priorityModifier;
    static Node _targetNode;
    static Node _startNode;

    #region Initialisation
    public static void RunPathfinder(int rows, int columns, int startX, int startY, int targetX, int targetY, PathfinderEnvironment environment)
    {
        _startNode = new Node();
        _startNode.X = startX;
        _startNode.Y = startY;
        _targetNode = new Node();
        _targetNode.X = targetX;
        _targetNode.Y = targetY;

        Node lastNode = _startNode;

        _initialize(rows, columns);

        _computeShortestPath();

        while (!_startNode.Equals(_targetNode))
        {
            _startNode = _minimumSuccessorNode(_startNode);
            environment.MoveTo(new Coordinates(_startNode.X, _startNode.Y));
            LinkedList<Coordinates> obstacleCoordinates = environment.GetObstaclesInVision();
            double oldPriorityModifier = _priorityModifier;
            Node oldLastNode = lastNode;
            _priorityModifier += _manhattanDistance(_startNode, lastNode);
            lastNode = _startNode;
            bool change = false;

            foreach (Coordinates coordinate in obstacleCoordinates)
            {
                Node node = _nodes[coordinate.X, coordinate.Y];
                if (node.IsObstacle) continue;
                change = true;
                node.IsObstacle = true;
                foreach (Node p in node.GetPredecessors(_nodes))
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
        }
    }
    static Priority _calculatePriority(Node node)
    {
        return new Priority(Math.Min(node.G, node.RHS) + _manhattanDistance(node, _startNode) + _priorityModifier, Math.Min(node.G, node.RHS));
    }
    static double _manhattanDistance(Node a, Node b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }
    static void _initialize(int rows, int columns)
    {
        _mainPriorityQueue = new PriorityQueue(rows * columns);
        _nodes = new Node[rows, columns];
        _priorityModifier = 0;
        for (int row = 0; row < _nodes.GetLength(0); row++)
        {
            for (int column = 0; column < _nodes.GetLength(1); column++)
            {
                _nodes[row, column] = new Node();
                _nodes[row, column].X = row;
                _nodes[row, column].Y = column;
                _nodes[row, column].G = Double.PositiveInfinity;
                _nodes[row, column].RHS = Double.PositiveInfinity;
            }
        }
        _targetNode = _nodes[_targetNode.X, _targetNode.Y];
        _startNode = _nodes[_startNode.X, _startNode.Y];
        _targetNode.RHS = 0;
        _mainPriorityQueue.Enqueue(_targetNode, _calculatePriority(_targetNode));
    }
    static void _updateVertex(Node node)
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
    static Node _minimumSuccessorNode(Node node)
    {
        double minimumCostToMove = Double.PositiveInfinity;
        Node newNode = null;
        foreach (Node successor in node.GetSuccessors(_nodes))
        {
            double costToMove = node.GetMovementCostTo(successor) + successor.G;
            if (costToMove <= minimumCostToMove && !successor.IsObstacle)
            {
                minimumCostToMove = costToMove;
                newNode = successor;
            }
        }
        return newNode;
    }
    static double _minimumSuccessorCost(Node node)
    {
        double minimumCost = Double.PositiveInfinity;
        foreach (Node successor in node.GetSuccessors(_nodes))
        {
            double costToMove = node.GetMovementCostTo(successor) + successor.G;
            if (costToMove < minimumCost && !successor.IsObstacle) minimumCost = costToMove;
        }
        return minimumCost;
    }
    static void _computeShortestPath()
    {
        while (_mainPriorityQueue.Peek().CompareTo(_calculatePriority(_startNode)) < 0 || _startNode.RHS != _startNode.G)
        {
            Priority highestPriority = _mainPriorityQueue.Peek();
            Node node = _mainPriorityQueue.Dequeue();
            if (node == null) break;

            if (highestPriority.CompareTo(_calculatePriority(node)) < 0)
            {
                _mainPriorityQueue.Enqueue(node, _calculatePriority(node));
            }
            else if (node.G > node.RHS)
            {
                node.G = node.RHS;
                foreach (Node neighbour in node.GetPredecessors(_nodes))
                {
                    _updateVertex(neighbour);
                }
            }
            else
            {
                node.G = Double.PositiveInfinity;
                _updateVertex(node);
                foreach (Node neighbour in node.GetPredecessors(_nodes))
                {
                    _updateVertex(neighbour);
                }
            }
        }
    }
    #endregion

    public static void UpdateWallState(Node node, Direction direction, bool wallExists)
    {
        double newCost = wallExists ? Double.PositiveInfinity : 1;

        node.UpdateMovementCost(direction, newCost);

        Node neighborNode = GetNeighbor(node, direction);

        if (neighborNode != null) { Debug.Log("Has no neighbours."); return; }

        Direction oppositeDirection = GetOppositeDirection(direction);
        neighborNode.UpdateMovementCost(oppositeDirection, newCost);
    }

    static Node GetNeighbor(Node node, Direction direction)
    {
        switch (direction)
        {
            case Direction.Top: return node.Y + 1 < _nodes.GetLength(1) ? _nodes[node.X, node.Y + 1] : null;
            case Direction.Bottom: return node.Y - 1 >= 0 ? _nodes[node.X, node.Y - 1] : null;
            case Direction.Left: return node.X - 1 >= 0 ? _nodes[node.X - 1, node.Y] : null;
            case Direction.Right: return node.X + 1 < _nodes.GetLength(0) ? _nodes[node.X + 1, node.Y] : null;
            default: return null;
        }
    }

    static Direction GetOppositeDirection(Direction direction)
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

    public static Node GetNodeAtPosition(int x, int y)
    {
        if (_nodes[x, y] != null) return _nodes[x, y];
        return null;
    }

    public static void RunPathfinderForChaser(Chaser chaser, PathfinderEnvironment environment)
    {
        Node startNode = chaser.StartNode;
        Node targetNode = chaser.TargetNode;

        Node currentNode = startNode;

        while (!currentNode.Equals(targetNode))
        {
            currentNode = _minimumSuccessorNode(currentNode);
            environment.MoveTo(new Coordinates(currentNode.X, currentNode.Y));

            _computeShortestPathForChaser(currentNode, targetNode);
        }
    }

    static void _computeShortestPathForChaser(Node startNode, Node targetNode)
    {
        while (_mainPriorityQueue.Peek().CompareTo(_calculatePriority(startNode)) < 0 || startNode.RHS != startNode.G)
        {
            Priority highestPriority = _mainPriorityQueue.Peek();
            Node node = _mainPriorityQueue.Dequeue();
            if (node == null) break;

            if (highestPriority.CompareTo(_calculatePriority(node)) < 0)
            {
                _mainPriorityQueue.Enqueue(node, _calculatePriority(node));
            }
            else if (node.G > node.RHS)
            {
                node.G = node.RHS;
                foreach (Node neighbour in node.GetPredecessors(_nodes))
                {
                    _updateVertex(neighbour);
                }
            }
            else
            {
                node.G = Double.PositiveInfinity;
                _updateVertex(node);
                foreach (Node neighbour in node.GetPredecessors(_nodes))
                {
                    _updateVertex(neighbour);
                }
            }
        }
    }
}

public enum Direction { None, Top, Bottom, Left, Right }

public class Node
{
    public int X;
    public int Y;
    public double G;
    public double RHS;

    public Dictionary<Direction, double> MovementCosts { get; private set; }

    public bool IsObstacle;

    public Node()
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
        MovementCosts[direction] = cost;
    }

    public double GetMovementCostTo(Node successor)
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

    public bool Equals(Node that)
    {
        if (X == that.X && Y == that.Y) return true;
        return false;
    }

    public LinkedList<Node> GetSuccessors(Node[,] nodes)
    {
        LinkedList<Node> successors = new LinkedList<Node>();
        if (X + 1 < nodes.GetLength(0)) successors.AddFirst(nodes[X + 1, Y]);
        if (Y + 1 < nodes.GetLength(1)) successors.AddFirst(nodes[X, Y + 1]);
        if (X - 1 >= 0) successors.AddFirst(nodes[X - 1, Y]);
        if (Y - 1 >= 0) successors.AddFirst(nodes[X, Y - 1]);
        return successors;
    }

    public LinkedList<Node> GetPredecessors(Node[,] nodes)
    {
        LinkedList<Node> neighbours = new LinkedList<Node>();
        Node tempNode;
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
        if (this.PrimaryPriority < that.PrimaryPriority) return -1;
        else if (this.PrimaryPriority > that.PrimaryPriority) return 1;
        if (this.SecondaryPriority > that.SecondaryPriority) return 1;
        else if (this.SecondaryPriority < that.SecondaryPriority) return -1;
        return 0;
    }
}

public class NodeQueue
{
    public Node Node;
    public Priority Priority;

    public NodeQueue(Node node, Priority priority)
    {
        Node = node;
        Priority = priority;
    }
}

public class PriorityQueue
{
    int _currentPosition;
    NodeQueue[] _nodeQueue;
    Dictionary<Node, int> _priorityQueue;

    public PriorityQueue(int maxNodes)
    {
        _currentPosition = 0;
        _nodeQueue = new NodeQueue[maxNodes];
        _priorityQueue = new Dictionary<Node, int>();
    }

    public Priority Peek()
    {
        if (_currentPosition == 0) return new Priority(Double.PositiveInfinity, Double.PositiveInfinity);

        return _nodeQueue[1].Priority;
    }

    public Node Dequeue()
    {
        if (_currentPosition == 0) return null;

        Node node = _nodeQueue[1].Node;
        _nodeQueue[1] = _nodeQueue[_currentPosition];
        _priorityQueue[_nodeQueue[1].Node] = 1;
        _priorityQueue[node] = 0;
        _currentPosition--;
        _moveDown(1);
        return node;
    }

    public void Enqueue(Node node, Priority priority)
    {
        NodeQueue nodeQueue = new NodeQueue(node, priority);
        _currentPosition++;
        _priorityQueue[node] = _currentPosition;
        if (_currentPosition == _nodeQueue.Length) Array.Resize<NodeQueue>(ref _nodeQueue, _nodeQueue.Length * 2);
        _nodeQueue[_currentPosition] = nodeQueue;
        _moveUp(_currentPosition);
    }

    public void Update(Node node, Priority priority)
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

    public void Remove(Node node)
    {
        int index = _priorityQueue[node];

        if (index == 0) return;

        _priorityQueue[node] = 0;
        _nodeQueue[index] = _nodeQueue[_currentPosition];
        _priorityQueue[_nodeQueue[index].Node] = index;
        _currentPosition--;
        _moveDown(index);
    }

    public bool Contains(Node node)
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
        this.X = x;
        this.Y = y;
    }
}

public interface PathfinderEnvironment
{
    void MoveTo(Coordinates coordinates);
    LinkedList<Coordinates> GetObstaclesInVision();
}

