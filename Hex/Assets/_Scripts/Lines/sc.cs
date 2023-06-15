using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class sc : MonoBehaviour
{
    public LineRenderer line;
    public int boardHeight; // Set this to your actual board height

    void Start()
    {
        boardHeight = MainMenuManager.gridSize;
        // The number of points for the line renderer
        // is the height times 2 plus 1 for the starting point
        //boardHeight = GridManager.Instance.gridSize;
        line.positionCount = boardHeight * 2 + 1;
        List<Vector3> points = new List<Vector3>();
        
        float y = 0.25f;
        float x = -0.49f;
        points.Add(new Vector3(x, y, 0));
        for (int i = 0; i < boardHeight; i++)
        {
            y = y - 0.52f;
            points.Add(new Vector3(x, y, 0));

            x = x + 0.45f;
            y = y - 0.25f;
            points.Add(new Vector3(x, y, 0));
        }

       
    


        // Apply the points to the line renderer
        line.SetPositions(points.ToArray());
    }
}