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

    public int gridSize;

    private void Awake()
    {
        Instance = this;
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
                Transform hex = Instantiate(Tile) as Transform;
                hex.position = new Vector2(xPos, -y * yOffset);
                hex.name = "Hexagon" + x + "|" + y;
                hex.gameObject.AddComponent<BoxCollider2D>();
                //GameObject hex = Instantiate(hexPrefab, new Vector2(xPos, -y * yOffset), Quaternion.identity);
                // hex.transform.localScale = new Vector3(tileSize, tileSize, 1.0f);

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
