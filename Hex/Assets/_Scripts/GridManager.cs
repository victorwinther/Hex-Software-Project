using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


 
public class GridManager : MonoBehaviour {
    public float offset = 0;

    public Transform Tile;

    public int gridWidth = 9;
    public int gridHeight = 9;

    float hexWidth = 1.732f;
    float hexHeight= 2.0f;
    public float gap = 0.0f;

    Vector3 startPos;

    void Start() {
        AddGap();
        CalcStartPos();
        CreateGrid();

    }

    void AddGap(){
        hexWidth += hexWidth * gap;
        hexHeight += hexHeight * gap;
    }

    void CalcStartPos() {
        float offset = 0;
         if(gridHeight / 2 % 2 != 0){
        offset = hexWidth / 2;
         }

        float x = -hexWidth * (gridWidth / 2) - offset;
        float y = hexHeight *0.75f * (gridHeight/2);
    }

    Vector3 CalcWorldPos(Vector2 gridPos){
        
         //if(gridPos.y % 2 != 0) 
        //offset += (hexWidth/2); 
 
        float x = startPos.x + gridPos.x * hexWidth + offset;
        float y = startPos.y - gridPos.y * hexHeight * 0.75f;
        

        return new Vector3(x,y,0);
    }

    void CreateGrid() {

        for (int y = 0; y < gridHeight; y++) {
            // the line below shapes the board like a paralelogram.
            startPos.x += hexWidth/2;
            for (int x = 0; x < gridWidth; x++) {
                        Transform hex = Instantiate(Tile) as Transform;
                        Vector2 gridPos = new Vector2(x,y);
                        hex.position = CalcWorldPos(gridPos);
                        hex.parent = this.transform; 
                        hex.name = "Hexagon" + x + "|" + y; 


                }
            }

    }


}
   