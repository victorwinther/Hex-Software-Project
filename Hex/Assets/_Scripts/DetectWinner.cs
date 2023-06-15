using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
class GameUtils
{
    public (int, List<(int, int)>) CheckWin(Tile[][] matrix, int player)
    {
        // Create a queue to perform BFS
        Queue<(int, int)> queue = new Queue<(int, int)>();
        // Create a set to keep track of visited positions
        HashSet<(int, int)> visited = new HashSet<(int, int)>();
        // Create a dictionary to store the parent of each visited position
        Dictionary<(int, int), (int, int)> parent = new Dictionary<(int, int), (int, int)>();

        if (player == 1)
        {
            // Check the top row for player 1's tiles
            for (int c = 0; c < matrix[0].Length; c++)
            {
                if (matrix[0][c].Owner == player)
                {
                    // Enqueue the position and mark it as visited
                    queue.Enqueue((0, c));
                    visited.Add((0, c));
                    parent[(0, c)] = (-1, -1); // Set a dummy parent for the starting position
                }
            }

            // Perform BFS
            while (queue.Count > 0)
            {
                // Dequeue a position
                (int r, int c) = queue.Dequeue();

                // Check if the current position is in the last row
                if (r == matrix.Length - 1)
                {
                    // Player 1 has reached the bottom, reconstruct the path
                    List<(int, int)> shortestPath = new List<(int, int)>();
                    (int, int) currentPosition = (r, c);

                    while (currentPosition != (-1, -1)) // Stop when reaching the dummy parent
                    {
                        shortestPath.Insert(0, currentPosition);
                        currentPosition = parent[currentPosition];
                    }
                    // Player 1 has won, return the player number
                    return (player, shortestPath);
                }

                // Define the neighboring positions
                (int, int)[] neighbors = {
            (r - 1, c), (r, c - 1), (r + 1, c), (r, c + 1), (r - 1, c + 1), (r + 1, c - 1)
        };

                // Check each neighboring position
                foreach ((int x, int y) in neighbors)
                {
                    // Ensure the position is within the matrix bounds, owned by the player, and not visited
                    if (x >= 0 && x < matrix.Length && y >= 0 && y < matrix[x].Length &&
                        matrix[x][y].Owner == player && !visited.Contains((x, y)))
                    {
                        // Enqueue the position, mark it as visited, and store its parent
                        queue.Enqueue((x, y));
                        visited.Add((x, y));
                        parent[(x, y)] = (r, c); // Set the parent of the current position
                    }
                }
            }

            // Player 1 has not won, return 0
            return (0, new List<(int, int)>());
        }

        else if (player == 2)
        {
            // Check the leftmost column for player 2's tiles
            for (int r = 0; r < matrix.Length; r++)
            {
                if (matrix[r][0].Owner == player)
                {
                    // Enqueue the position and mark it as visited
                    queue.Enqueue((r, 0));
                    visited.Add((r, 0));
                    parent[(r, 0)] = (-1, -1); // Set a dummy parent for the starting position
                }
            }

            // Perform BFS
            while (queue.Count > 0)
            {
                // Dequeue a position
                (int r, int c) = queue.Dequeue();

                // Check if the current position is in the rightmost column
                if (c == matrix[0].Length - 1)

                {
                    List<(int, int)> shortestPath = new List<(int, int)>();
                    (int, int) currentPosition = (r, c);

                    while (currentPosition != (-1, -1)) // Stop when reaching the dummy parent
                    {
                        shortestPath.Insert(0, currentPosition);
                        currentPosition = parent[currentPosition];
                    }
                    // Player 1 has won, return the player number
                    return (player, shortestPath);
                }

                // Define the neighboring positions
                (int, int)[] neighbors = {
                (r - 1, c), (r, c - 1), (r + 1, c), (r, c + 1), (r - 1, c + 1), (r + 1, c - 1)
            };

                // Check each neighboring position
                foreach ((int x, int y) in neighbors)
                {
                    // Ensure the position is within the matrix bounds, owned by the player, and not visited
                    if (x >= 0 && x < matrix.Length && y >= 0 && y < matrix[x].Length &&
                        matrix[x][y].Owner == player && !visited.Contains((x, y)))
                    {
                        // Enqueue the position and mark it as visited
                        queue.Enqueue((x, y));
                        visited.Add((x, y));
                        parent[(x, y)] = (r, c); // Set the parent of the current position
                    }
                }
            }

            // Player 2 has not won, return 0
            return (0, new List<(int, int)>());
        }
        else
        {
            // Return 0 for other players
            return (0, new List<(int, int)>());
        }
    }

}
