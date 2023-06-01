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

    /*
    public Camera mainCamera;
    */

    public Transform Tile;

    public int gridWidth = 3;
    public int gridHeight = 3;

    float hexWidth = 1.732f;
    float hexHeight = 2.0f;
    public float gap = 0.0f;

  

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
                hex.parent = this.transform;
                hex.name = "Hexagon" + x + "|" + y;

              //  hex.AddComponent<BoxCollider2D>();
                
                hex.gameObject.AddComponent<BoxCollider2D>();

               

            }
        }
        Debug.Log(gridWidth + " and "+ gridHeight);
        // cam.transform.position = new Vector3((float) gridWidth - gridWidth/5, -(float)gridHeight / 2 + 0.8f, -10);
        float camX = (gridWidth - 1) * hexWidth * 0.75f;
        float camY = (gridHeight - 1) * hexHeight;
        cam.position = new Vector3(camX / 2, -camY / 2, -10);
        float camSize = Mathf.Max(camX, camY) / 2;
        cam.GetComponent<Camera>().orthographicSize = camSize;

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
