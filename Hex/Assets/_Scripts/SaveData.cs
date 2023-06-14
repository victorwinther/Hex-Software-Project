using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public List<List<int>> boardState;
    public int gridSize;
    public int currentPlayer;

    public SaveData(int gridSize, int currentPlayer, Tile[][] tiles)
    {
        this.gridSize = gridSize;
        this.currentPlayer = currentPlayer;

        boardState = new List<List<int>>();
        for (int x = 0; x < gridSize; x++)
        {
            List<int> row = new List<int>();
            for (int y = 0; y < gridSize; y++)
            {
                row.Add(tiles[x][y].Owner);
            }
            boardState.Add(row);
        }
    }
}
