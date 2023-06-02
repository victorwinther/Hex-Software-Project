using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


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
    /*
    public Camera mainCamera;
    */

    public Transform Tile;

    public int gridSize = 3;
    public float tileSize; // Adjust the size of the tiles

    float hexSize = 2.0f;
    public float gap = 0.0f;

  

    private void Awake()
    {
        Instance = this;
    }

    
    public void AddGap()
    {
        hexSize = (11 / gridSize) * hexSize;
        gap = 1.0f;
        hexSize += 0;
       
    }
    /*
    public void CalcStartPos()
    {
        
        float offset = 0;
        if (gridSize / 2 % 2 != 0)
        {
            offset = hexSize / 2;
        }

        float x = -hexSize * (gridSize / 2) - offset;
        float y = hexSize * 0.75f * (gridSize / 2);
    }
    */
    public void CalcStartPos()
    {
        float offset = 0;
        if (gridSize / 2 % 2 != 0)
        {
            offset = hexSize / 2;
        }
        startPos.x = 0;
        startPos.y = 0;
        /*
        startPos.x = -hexSize * (gridSize / 2) - offset;
        startPos.y = hexSize * 0.5f * (gridSize / 2);
        */
    }
    public Vector3 CalcWorldPos(Vector2 gridPos)
    {

        //if(gridPos.y % 2 != 0) 
        //offset += (hexWidth/2); 
        float xSpacing = hexSize * 0.45f; // Adjust the spacing between tiles horizontally
        float ySpacing = hexSize * 0.45f; // Adjust the spacing between tiles vertically

        float x = startPos.x + gridPos.x * xSpacing;
        float y = startPos.y - gridPos.y * ySpacing;

        //float x = startPos.x + gridPos.x * hexSize + offset;
        //float y = startPos.y - gridPos.y * hexSize * 0.75f;


        return new Vector2(x, y);
    }
    /*
    public void CreateGrid()
    {
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

                //  hex.AddComponent<BoxCollider2D>();

               // hex.gameObject.AddComponent<BoxCollider2D>();

                hex.localScale = new Vector3(tileSize, tileSize, 1.0f);

            }
   
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
        if(_tiles.TryGetValue(pos,out var tile))
        {
            return tile;
        }
        return null;
    }


}
