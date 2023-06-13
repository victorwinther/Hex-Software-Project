using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    public float offset = 0;
    Vector3 startPos;

    private Dictionary<Vector2, Tile> _tiles;
    public List<Vector2> clickedTiles = new List<Vector2>();
    [SerializeField] private Transform cam;

    public GameObject hexPrefab;

    public Transform gameBoard;

    public Transform Tile;

    public Tile[][] tiles;
    public int[][] tileOwners;

    public int gridSize = 3;

    public float tileSize; // Adjust the size of the tiles

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreateGrid()
    {
        
        int gridSize = MainMenuManager.gridSize;
       // creating the tile array
        tiles = new Tile[gridSize][];
        for (int i = 0; i < gridSize; i++)
        {
            tiles[i] = new Tile[gridSize];
        }
        // creating the int array
        tileOwners = new int[gridSize][];
        for (int i = 0; i < gridSize; i++)
        {
            tileOwners[i] = new int[gridSize];
        }


        float offRoxXOffset = 0.9f;
        float yOffset = 0.77f;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                float xPos = x * offRoxXOffset + (y * offRoxXOffset) / 2;
                Transform hex = Instantiate(Tile) as Transform;
                hex.position = new Vector2(xPos, -y * yOffset);
                hex.name = "Hexagon" + x + "|" + y;
                hex.gameObject.AddComponent<BoxCollider2D>();

                Tile tileScript = hex.gameObject.GetComponent<Tile>();

                tiles[x][y] = tileScript;
                tileOwners[x][y] = tileScript.Owner; // Assign the owner to tileOwners
            }
        }

    }

    public Tile GetTileAtPostion(Vector2 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;

        if (x >= 0 && x < gridSize && y >= 0 && y < gridSize)
        {
            return tiles[x][y];
        }
        return null;
    }

    public void PrintBoardState()
    {
        string boardState = "";
        for (int y = 0; y < MainMenuManager.gridSize; y++)
        {
            for (int x = 0; x < MainMenuManager.gridSize; x++)
            {
                if (tileOwners[x][y] == 0)
                    boardState += "0 "; // Unclaimed
                else if (tileOwners[x][y] == 1)
                    boardState += "1 "; // Player 1
                else if (tileOwners[x][y] == 2)
                    boardState += "2 "; // Player 2
            }
            boardState += "\n";
        }

        Debug.Log(boardState);
    }
}
