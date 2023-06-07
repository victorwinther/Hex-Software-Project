using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class right_red_line : MonoBehaviour
{
    public LineRenderer line;
    public int boardHeight; // Set this to your actual board height

    void Start()
    {
        boardHeight = MainMenuManager.gridSize;
        //boardHeight = GridManager.Instance.gridSize;
        // The number of points for the line renderer is the height times 2 plus 1 for the starting point
        line.positionCount = boardHeight*2;
        List<Vector3> points = new List<Vector3>();

        float y = 0.25f;
        //float x = 0.9f*boardHeight + 0.81f/2 + 0.09f;
        float x = 0.45f + (boardHeight - 1) * 0.9f + 0.04f;
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
