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

    public Transform gameBoard;
    /*
    public Camera mainCamera;
    */

    public Transform Tile;

    public int gridWidth = 3;
    public int gridHeight = 3;

    float hexWidth = 1.732f;
    float hexHeight = 2.0f;
    public float gap = 0.0f;

    public float tileSize; // Adjust the size of the tiles

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

        //if(gridPos.y % 2 != 0) 
        //offset += (hexWidth/2); 

        float x = startPos.x + gridPos.x * hexWidth + offset;
        float y = startPos.y - gridPos.y * hexHeight * 0.75f;


        return new Vector2(x, y);
    }

    public void CreateGrid()
    {
        _tiles = new Dictionary<Vector2, Tile>();
        startPos = Vector3.zero; // Initialize startPos to Vector3.zero
        for (int y = 0; y < gridHeight; y++)
        {
            // the line below shapes the board like a paralelogram.
            startPos.x += hexWidth / 2;
            for (int x = 0; x < gridWidth; x++)
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

    
        // Set the camera position to the calculated position
        cam.position = new Vector3(7.85f, -4.35f, -1.0f); ;

        // Set the camera's orthographic size based on the required size
        cam.GetComponent<Camera>().orthographicSize = 5.77622f;

  
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
