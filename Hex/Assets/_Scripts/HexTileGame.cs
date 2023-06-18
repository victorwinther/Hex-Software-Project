using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexTileGame
{
    private Tile[][] board;
    public int numRows;
    public int numCols;
    private int player;
    public HexTileGame(Tile[][] board)
    {
        this.board = MirrorBoard(board);
        numRows = board.Length;
        numCols = board[0].Length;

        
    }
    // Helper method to create a mirrored copy of the board array
    private Tile[][] MirrorBoard(Tile[][] originalBoard)
    {
        int numRows = originalBoard.Length;
        int numCols = originalBoard[0].Length;

        Tile[][] mirroredBoard = new Tile[numRows][];

        for (int x = 0; x < numCols; x++)
        {
            mirroredBoard[x] = new Tile[numRows];
            for (int y = 0; y < numRows; y++)
            {
                mirroredBoard[x][y] = originalBoard[y][x];
            }
        }

        return mirroredBoard;
    }

    public List<Hex> FindShortestPath(int player)
    {
        this.player = player;
        // Create a 2D array to track the distances from the bottom row
        double[,] distances = new double[numRows, numCols];
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                distances[i, j] = double.PositiveInfinity;
            }
        }

        // Create a 2D array to track the previous positions for constructing the path
        Position[,] previousPositions = new Position[numRows, numCols];

        // Create a priority queue to store the positions to visit
        PriorityQueue<Position> queue = new PriorityQueue<Position>();

        // Initialize the bottom row positions
        if (player == 2)
        {
            for (int col = 0; col < numCols; col++)
            {
                if (board[numRows - 1][col].Owner == player)
                {
                    distances[numRows - 1, col] = 0;
                    queue.Enqueue(new Position(numRows - 1, col), 0);
                }
                if (board[numRows - 1][col].Owner == 0)
                {
                    distances[numRows - 1, col] = 1;
                    queue.Enqueue(new Position(numRows - 1, col), 0);
                }
            }
        }
        else
        {
            for (int col = 0; col < numCols; col++)
            {
                if (board[col][0].Owner == player)
                {
                    distances[col, 0] = 0;
                    queue.Enqueue(new Position(col, 0), 0);
                }
                if (board[col][0].Owner == 0)
                {
                    distances[col, 0] = 1;
                    queue.Enqueue(new Position(col, 0), 0);
                }
            }
        }
        List<Hex> path1 = new List<Hex>();
        double minValue = 1000;

        while (!queue.IsEmpty())
        {
            Position current = queue.Dequeue();

            // Check if we reached the top row
            if ((current.Row == 0 && player == 2) || (current.Col == (numCols-1) && player == 1))
            {
                Console.Write(distances[current.Row, current.Col] + " distance from buttom");
                if (distances[current.Row, current.Col] < minValue)
                {
                    minValue = distances[current.Row, current.Col];
                    path1.Clear();
                    // Reconstruct the path from top to bottom
                    Position currentPosition = current;
                    while (currentPosition != null)
                    {
                        int value = board[currentPosition.Row][currentPosition.Col].Owner;
                        path1.Add(new Hex(currentPosition, value));
                        currentPosition = previousPositions[currentPosition.Row, currentPosition.Col];
                    }

                    path1.Reverse();  // Reverse the path to start from the top row
                }
            }

            // Get the neighbors of the current position
            List<Position> neighbors = GetNeighbors(current);

            foreach (Position neighbor in neighbors)
            {
                double weight = GetWeight(current, neighbor);
                double newDistance = distances[current.Row, current.Col] + weight;

                if (newDistance < distances[neighbor.Row, neighbor.Col])
                {
                    distances[neighbor.Row, neighbor.Col] = newDistance;
                    previousPositions[neighbor.Row, neighbor.Col] = current;
                    queue.Enqueue(neighbor, newDistance);
                }
            }
        }


       
        return path1;

        
    }

    private List<Position> GetNeighbors(Position position)
    {
        List<Position> neighbors = new List<Position>();

        int row = position.Row;
        int col = position.Col;

        // Define the six possible directions for hex tile neighbors

        int[,] directions = new int[,]
        {
            { -1, 0 },   // Top
            { -1, 1 },   // Top-right
            { 0, 1 },    // Bottom-right
            { 1, 0 },    // Bottom
            { 1, -1 },   // Bottom-left
            { 0, -1 }    // Top-left
        };

        for (int i = 0; i < 6; i++)
        {
            int newRow = row + directions[i, 0];
            int newCol = col + directions[i, 1];

            // Check if the neighbor position is within the board boundaries
            if (newRow >= 0 && newRow < numRows && newCol >= 0 && newCol < numCols)
            {
              neighbors.Add(new Position(newRow, newCol));
            }
        }
        Console.Write("returned neighbors");
        return neighbors;
    }

    private double GetWeight(Position current, Position neighbor)
    {
        int currentValue = board[current.Row][current.Col].Owner;
        int neighborValue = board[neighbor.Row][neighbor.Col].Owner;
        if (player == 2)
        {
            if (currentValue == 2 && neighborValue == 2)
            {
                Console.Write("Same");
                // Both blue when weight is 0
                return 0;
            }
            else if (currentValue == 0 && neighborValue == 2)
            {
                Console.Write("Correct move");
                // Blue player can move to unoccupied tile
                return 0;
            }
            else if (currentValue == 2 && neighborValue == 0)
            {
                Console.Write("Correct move");
                // Blue player can move to unoccupied tile
                return 1;
            }

            else if (currentValue == 0 && neighborValue == 0)
            {
                return 1;
            }
        }

        if(player == 1)
        {
            if (currentValue == 1 && neighborValue == 1)
            {
                Console.Write("Same");
                // Both blue when weight is 0
                return 0;
            }
            else if (currentValue == 0 && neighborValue == 1)
            {
                Console.Write("Correct move");
                // Blue player can move to unoccupied tile
                return 0;
            }
            else if (currentValue == 1 && neighborValue == 0)
            {
                Console.Write("Correct move");
                // Blue player can move to unoccupied tile
                return 1;
            }

            else if (currentValue == 0 && neighborValue == 0)
            {
                return 1;
            }
        }
      
        // All other moves have infinite weight
         return double.PositiveInfinity;
    }
}

public class Hex
{
    public Position Position { get; set; }
    public int Value { get; set; }

    public Hex(Position position, int value)
    {
        Position = position;
        Value = value;
    }
}

public class Position
{
    public int Row { get; set; }
    public int Col { get; set; }

    public Position(int row, int col)
    {
        Row = row;
        Col = col;
    }
}

// Priority queue implementation for Dijkstra's algorithm
public class PriorityQueue<T>
{
    private SortedDictionary<double, Queue<T>> sortedDictionary;

    public PriorityQueue()
    {
        sortedDictionary = new SortedDictionary<double, Queue<T>>();
    }

    public void Enqueue(T item, double priority)
    {
        if (!sortedDictionary.TryGetValue(priority, out Queue<T> queue))
        {
            queue = new Queue<T>();
            sortedDictionary.Add(priority, queue);
        }
        queue.Enqueue(item);
    }

    public T Dequeue()
    {
        var pair = sortedDictionary.First();
        var item = pair.Value.Dequeue();
        if (pair.Value.Count == 0)
        {
            sortedDictionary.Remove(pair.Key);
        }
        return item;
    }

    public bool IsEmpty()
    {
        return sortedDictionary.Count == 0;
    }
}