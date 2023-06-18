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
    public int gridSize = 11;

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
    private Color originalColor;
    public bool isHighlighted;
    List<Hex> shortestPath;
    Color newColor;
    public void onClick()
    {
        // Create an instance of HexTileGame
        HexTileGame game = new HexTileGame(GridManager.Instance.tiles);
        // Call the FindShortestPath method


        if (!isHighlighted)
        {
            shortestPath = game.FindShortestPath(GameManager.CurrentPlayer);
            Debug.Log("Shortest Path in tile");
            string s = "";
            foreach (Hex hex in shortestPath)
            {
                s += ($"Position: ({hex.Position.Row}, {hex.Position.Col}), Value: {hex.Value}");
                s += "\n";
            }
            Debug.Log(s);
            // Print the path
            //Console.WriteLine("Shortest Path:");


            if (GameManager.CurrentPlayer == 2)
            {
                newColor = ColorUtility.TryParseHtmlString("#00A8FF", out Color convertedColor) ? convertedColor : Color.magenta;
                newColor.a = 0.2f;
            }
            else
            {
                newColor = Color.red;
                newColor.a = 0.5f;
            }

            foreach (Hex hex in shortestPath)
            {
                if (GridManager.Instance.tiles[hex.Position.Col][hex.Position.Row].GetComponent<SpriteRenderer>().color == Color.white)
                {
                    GridManager.Instance.tiles[hex.Position.Col][hex.Position.Row].GetComponent<SpriteRenderer>().color = newColor;
                };

            }
            isHighlighted = true;
        }
        else
        {
            foreach (Hex hex in shortestPath)
            {
                if (GridManager.Instance.tiles[hex.Position.Col][hex.Position.Row].GetComponent<SpriteRenderer>().color == newColor)
                {
                    GridManager.Instance.tiles[hex.Position.Col][hex.Position.Row].GetComponent<SpriteRenderer>().color = Color.white;
                };

            }
            isHighlighted = false;

        }
    }


    public void CreateGrid()
    {
        int gridSize = MainMenuManager.gridSize;
        tiles = new Tile[gridSize][];
        for (int i = 0; i < gridSize; i++)
        {
            tiles[i] = new Tile[gridSize];
        }
        float offRoxXOffset = 0.9f;
        float yOffset = 0.77f;
        int xStart = -12;
        int yStart = -8;
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                float xPos = (x + xStart) * offRoxXOffset + (y * offRoxXOffset) / 2;
                Transform hex = Instantiate(Tile) as Transform;
                hex.position = new Vector2(xPos, -(y + yStart) * yOffset);
                hex.parent = this.transform;
                hex.name = "Hexagon" + x + "|" + y;
                hex.gameObject.AddComponent<BoxCollider2D>();

                Tile tileScript = hex.gameObject.GetComponent<Tile>();

                tiles[x][y] = tileScript;
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
                if (tiles[x][y].Owner == 0)
                    boardState += "0 "; // Unclaimed
                else if (tiles[x][y].Owner == 1)
                    boardState += "1 "; // Player 1
                else if (tiles[x][y].Owner == 2)
                    boardState += "2 "; // Player 2
            }
            boardState += "\n";
        }

        Debug.Log(boardState);
    }
}
