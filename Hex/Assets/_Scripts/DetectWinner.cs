using System.Collections.Generic;

class GameUtils
{
    public int CheckWin(Tile[][] matrix, int player)
    {
        Queue<(int, int)> queue = new Queue<(int, int)>();
        HashSet<(int, int)> visited = new HashSet<(int, int)>();

        if (player == 1)
        {
            for (int c = 0; c < matrix[0].Length; c++)
            {
                if (matrix[0][c].Owner == player)
                {
                    queue.Enqueue((0, c));
                    visited.Add((0, c));
                }
            }

            while (queue.Count > 0)
            {
                (int r, int c) = queue.Dequeue();

                if (r == matrix.Length - 1)
                {
                    return player;
                }

                (int, int)[] neighbors = {
                    (r - 1, c), (r, c - 1), (r + 1, c), (r, c + 1), (r - 1, c + 1), (r + 1, c - 1)
                };

                foreach ((int x, int y) in neighbors)
                {
                    if (x >= 0 && x < matrix.Length && y >= 0 && y < matrix[x].Length &&
                        matrix[x][y].Owner == player && !visited.Contains((x, y)))
                    {
                        queue.Enqueue((x, y));
                        visited.Add((x, y));
                    }
                }
            }

            return 0;
        }
        else if (player == 2)
        {
            for (int r = 0; r < matrix.Length; r++)
            {
                if (matrix[r][0].Owner == player)
                {
                    queue.Enqueue((r, 0));
                    visited.Add((r, 0));
                }
            }

            while (queue.Count > 0)
            {
                (int r, int c) = queue.Dequeue();

                if (c == matrix[0].Length - 1)
                {
                    return player;
                }

                (int, int)[] neighbors = {
                    (r - 1, c), (r, c - 1), (r + 1, c), (r, c + 1), (r - 1, c + 1), (r + 1, c - 1)
                };

                foreach ((int x, int y) in neighbors)
                {
                    if (x >= 0 && x < matrix.Length && y >= 0 && y < matrix[x].Length &&
                        matrix[x][y].Owner == player && !visited.Contains((x, y)))
                    {
                        queue.Enqueue((x, y));
                        visited.Add((x, y));
                    }
                }
            }

            return 0;
        }
        else
        {
            return 0; // Return 0 for other players
        }
    }
}
