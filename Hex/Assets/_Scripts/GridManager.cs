using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour


{

    [SerializeField] private Transform cam;

    public static GridManager Instance;
    public float offset = 0;
    Vector3 startPos;

    private Dictionary<Vector2, Tile> _tiles;
    public List<Vector2> clickedTiles = new List<Vector2>();
    [SerializeField] private Transform cam;

    public GameObject hexPrefab;

    public Transform gameBoard;
    /*
    public Camera mainCamera;
    */


    public Transform Tile;

    public int gridSize = 3;
    public float tileSize; // Adjust the size of the tiles

    float hexSize = 2.0f;

    public Tile[,] tiles;

    private void Awake()
    {
        Instance = this;
    }

    /*
    public void CreateGrid()
    {

        tiles = new Tile[gridWidth, gridHeight];
        startPos = Vector3.zero;

        for (int y = 0; y < gridHeight; y++)
        {
            startPos.x += hexWidth / 2;
            for (int x = 0; x < gridWidth; x++)

        tileSize = (11/ gridSize);
         _tiles = new Dictionary<Vector2, Tile>();
        startPos = Vector3.zero; // Initialize startPos to Vector3.zero
        for (int y = 0; y < gridSize; y++)
        {
            // the line below shapes the board like a paralelogram.
            startPos.x += hexSize / 4;
            for (int x = 0; x < gridSize; x++)

            {
                Transform hex = Instantiate(Tile) as Transform;
                Vector2 gridPos = new Vector2(x, y);
                hex.position = CalcWorldPos(gridPos);
                //hex.parent = this.transform;
                hex.parent = gameBoard;
                hex.name = "Hexagon" + x + "|" + y;

                hex.gameObject.AddComponent<BoxCollider2D>();

                Tile tileScript = hex.gameObject.GetComponent<Tile>();


                //  hex.AddComponent<BoxCollider2D>();

               // hex.gameObject.AddComponent<BoxCollider2D>();

                hex.localScale = new Vector3(tileSize, tileSize, 1.0f);


                tiles[x, y] = tileScript;
            }
   
        }

   

    
        // Set the camera position to the calculated position
        cam.position = new Vector3(7.85f, -4.35f, -1.0f); ;


        // Set the camera's orthographic size based on the required size
        cam.GetComponent<Camera>().orthographicSize = 5.77622f;

  
    }
   */
    
    public void CreateGrid()
    {
        float offRoxXOffset = 0.9f;
        float yOffset = 0.77f;
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                float xPos = x*offRoxXOffset + (y*offRoxXOffset)/2;

                GameObject hex = Instantiate(hexPrefab, new Vector2(xPos, -y * yOffset), Quaternion.identity);
            }

        }
        
    }

        public Tile GetTileAtPostion(Vector2 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;

        if (x >= 0 && x < gridSize && y >= 0 && y < gridSize)
        {
            return tiles[x, y];
        }
        return null;
    }

    public void PrintBoardState()
    {
        string boardState = "";
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
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
