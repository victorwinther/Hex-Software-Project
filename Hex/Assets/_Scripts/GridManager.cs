using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour


{

    [SerializeField] private Transform cam;

    public static GridManager Instance;
    public float offset = 0;

    Vector3 startPos;
    public Transform Tile;

    public int gridWidth = 3;
    public int gridHeight = 3;

    float hexWidth = 1.732f;
    float hexHeight = 2.0f;
    public float gap = 0.0f;

    public Tile[,] tiles;

    private void Awake()
    {
        Instance = this;
    }

    public void AddGap()
    {
        hexWidth += hexWidth * gap;
        hexHeight += hexHeight * gap;
    }

    public void CalcStartPos()
    {
        float offset = 0;
        if (gridHeight / 2 % 2 != 0)
        {
            offset = hexWidth / 2;
        }

        float x = -hexWidth * (gridWidth / 2) - offset;
        float y = hexHeight * 0.75f * (gridHeight / 2);
    }

    public Vector3 CalcWorldPos(Vector2 gridPos)
    {
        float x = startPos.x + gridPos.x * hexWidth + offset;
        float y = startPos.y - gridPos.y * hexHeight * 0.75f;

        return new Vector2(x, y);
    }

    public void CreateGrid()
    {
        tiles = new Tile[gridWidth, gridHeight];
        startPos = Vector3.zero;

        for (int y = 0; y < gridHeight; y++)
        {
            startPos.x += hexWidth / 2;
            for (int x = 0; x < gridWidth; x++)
            {
                Transform hex = Instantiate(Tile) as Transform;
                Vector2 gridPos = new Vector2(x, y);
                hex.position = CalcWorldPos(gridPos);
                hex.parent = this.transform;
                hex.name = "Hexagon" + x + "|" + y;
                hex.gameObject.AddComponent<BoxCollider2D>();

                Tile tileScript = hex.gameObject.GetComponent<Tile>();

                tiles[x, y] = tileScript;
            }
        }

        // Adjust camera position and orthographic size to fit the entire board
        Vector3 boardCenter = new Vector3((gridWidth - 1) * hexWidth / 2, -(gridHeight - 1) * hexHeight / 2, -10);
        cam.transform.position = boardCenter;

        // Calculate the new orthographic size
        // Note: This assumes that your game is in landscape mode
        float cameraSizeX = (gridWidth * hexWidth) / 2;
        float cameraSizeY = (gridHeight * hexHeight) / 2;
        cam.GetComponent<Camera>().orthographicSize = Mathf.Max(cameraSizeX, cameraSizeY);
    }


    public Tile GetTileAtPostion(Vector2 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;

        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
        {
            return tiles[x, y];
        }
        return null;
    }

    public void PrintBoardState()
    {
        string boardState = "";
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (tiles[x, y].Owner == 0)
                    boardState += "0 "; // Unclaimed
                else if (tiles[x, y].Owner == 1)
                    boardState += "1 "; // Player 1
                else if (tiles[x, y].Owner == 2)
                    boardState += "2 "; // Player 2
            }
            boardState += "\n";
        }

        Debug.Log(boardState);
    }


}
