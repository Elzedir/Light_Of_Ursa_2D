using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Pathfinder_Base
{
    PriorityQueue _mainPriorityQueue;
    double _priorityModifier;
    Node _targetNode;
    Node _startNode;
    Spawner_Maze _spawner;

    #region Initialisation
    public IEnumerator RunPathfinder(int rows, int columns, int startX, int startY, int targetX, int targetY, PathfinderMover mover, Spawner_Maze spawner)
    {
        _spawner = spawner;

        _startNode = new Node();
        _startNode.X = startX;
        _startNode.Y = startY;
        _targetNode = new Node();
        _targetNode.X = targetX;
        _targetNode.Y = targetY;

        Node lastNode = _startNode;

        _initialise(rows, columns);

        _computeShortestPath();

        while (!_startNode.Equals(_targetNode))
        {
            _startNode = _minimumSuccessorNode(_startNode);

            Debug.Log($"StartNode: {_startNode.X}_{_startNode.Y}");
            Debug.Log($"StartNode Pred: {_startNode.Predecessor.X}_{_startNode.Predecessor.Y}");

            LinkedList<Coordinates> obstacleCoordinates = mover.GetObstaclesInVision();
            double oldPriorityModifier = _priorityModifier;
            Node oldLastNode = lastNode;
            _priorityModifier += _manhattanDistance(_startNode, lastNode);
            lastNode = _startNode;
            bool change = false;

            foreach (Coordinates coordinate in obstacleCoordinates)
            {
                Node node = NodeArray.Nodes[coordinate.X, coordinate.Y];
                if (node.IsObstacle) continue;
                change = true;
                node.IsObstacle = true;
                foreach (Node p in node.GetPredecessors(NodeArray.Nodes))
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

            Cell cell = _spawner.GetCell(_startNode.X, _startNode.Y);

            cell.MarkCell(Color.blue);

            yield return null;
        }

        Debug.Log($"Predecessor1: {PredecessorLoopCheck(_startNode, 1000)}");

        mover.MoveTo(_targetNode);

        Debug.Log($"Predecessor: {PredecessorLoopCheck(_startNode, 1000)}");
    }

    void _initialise(int rows, int columns)
    {
        _mainPriorityQueue = new PriorityQueue(rows * columns);
        _priorityModifier = 0;
        _targetNode = NodeArray.Nodes[_targetNode.X, _targetNode.Y];
        _startNode = NodeArray.Nodes[_startNode.X, _startNode.Y];
        _targetNode.RHS = 0;
        _mainPriorityQueue.Enqueue(_targetNode, _calculatePriority(_targetNode));
    }

    Priority _calculatePriority(Node node)
    {
        return new Priority(Math.Min(node.G, node.RHS) + _manhattanDistance(node, _startNode) + _priorityModifier, Math.Min(node.G, node.RHS));
    }
    double _manhattanDistance(Node a, Node b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }
    
    void _updateVertex(Node node)
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
    Node _minimumSuccessorNode(Node node)
    {
        double minimumCostToMove = Double.PositiveInfinity;
        Node newNode = null;
        foreach (Node successor in node.GetSuccessors(NodeArray.Nodes))
        {
            double costToMove = node.GetMovementCostTo(successor) + successor.G;

            if (costToMove <= minimumCostToMove && !successor.IsObstacle)
            {
                minimumCostToMove = costToMove;
                newNode = successor;
                newNode.Predecessor = node;
            }
        }

        return newNode;
    }
    double _minimumSuccessorCost(Node node)
    {
        double minimumCost = Double.PositiveInfinity;
        foreach (Node successor in node.GetSuccessors(NodeArray.Nodes))
        {
            double costToMove = node.GetMovementCostTo(successor) + successor.G;
            if (costToMove < minimumCost && !successor.IsObstacle) minimumCost = costToMove;
        }
        return minimumCost;
    }
    void _computeShortestPath()
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
                foreach (Node neighbour in node.GetPredecessors(NodeArray.Nodes))
                {
                    _updateVertex(neighbour);
                }
            }
            else
            {
                node.G = Double.PositiveInfinity;
                _updateVertex(node);
                foreach (Node neighbour in node.GetPredecessors(NodeArray.Nodes))
                {
                    _updateVertex(neighbour);
                }
            }
        }
    }
    #endregion

    public void UpdateWallState(Node node, Direction direction, bool wallExists)
    {
        double newCost = wallExists ? Double.PositiveInfinity : 1;

        node.UpdateMovementCost(direction, newCost);

        Node neighborNode = GetNeighbor(node, direction);

        if (neighborNode != null) { Debug.Log("Has no neighbours."); return; }

        Direction oppositeDirection = GetOppositeDirection(direction);
        neighborNode.UpdateMovementCost(oppositeDirection, newCost);
    }

    Node GetNeighbor(Node node, Direction direction)
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

    public static Node GetNodeAtPosition(int x, int y)
    {
        if (NodeArray.Nodes[x, y] != null) return NodeArray.Nodes[x, y];
        return null;
    }

    public List<Coordinates> RetrievePath(Node startNode, Node targetNode)
    {
        List<Coordinates> path = new List<Coordinates>();
        Node currentNode = targetNode;

        
        Debug.Log($"Start node: {startNode.X}_{startNode.Y}");
        Debug.Log($"Predecessor2: {Pathfinder_Base.PredecessorLoopCheck(startNode, 1000)}");

        for (int i = 0; i < 1000; i++)
        {
            if (currentNode == null || currentNode.Equals(startNode)) break;

            Debug.Log($"Current node: {currentNode.X}_{currentNode.Y} And Predecessor: {currentNode.Predecessor.X}_{currentNode.Predecessor.Y}");

            path.Add(new Coordinates(currentNode.X, currentNode.Y));
            currentNode = currentNode.Predecessor;
        }

        FindAllPredecessors(startNode, 1000);

        while (currentNode != null && !currentNode.Equals(startNode))
        {
            break;
        }

        if (currentNode != null)
        {
            Debug.Log("Added start");
            path.Add(new Coordinates(startNode.X, startNode.Y));
        }

        return path;
    }

    public static bool PredecessorLoopCheck(Node node, int infiniteEnd, int infinityStart = 0)
    {
        infinityStart++;
        if (infinityStart > infiniteEnd) return true;
        if (node.Predecessor == null) { Debug.Log($"{node.X}_{node.Y} predecessor is null"); return false; }
        Debug.Log($"{node.X}_{node.Y} -> {node.Predecessor.X}_{node.Predecessor.Y}");
        FindAllPredecessors(node.Predecessor, infinityStart, infiniteEnd);

        return false;
    }

    public static void FindAllPredecessors(Node node, int infiniteEnd, int infinityStart = 0)
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
    public static Node[,] Nodes;

    public static Node[,] InitializeArray(int rows, int columns)
    {
        Nodes = new Node[rows, columns];
        for (int row = 0; row < Nodes.GetLength(0); row++)
        {
            for (int column = 0; column < Nodes.GetLength(1); column++)
            {
                Nodes[row, column] = new Node();
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

public class Node
{
    public int X;
    public int Y;
    public double G;
    public double RHS;
    public Node Predecessor { get; set; }

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

public interface PathfinderMover
{
    void MoveTo(Node target);
    LinkedList<Coordinates> GetObstaclesInVision();
}

