using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class upper_blue : MonoBehaviour
{
    public LineRenderer line;
    public int boardHeight; // Set this to your actual board height

    //comment

    void Start()
    {
        boardHeight = MainMenuManager.gridSize;
        //boardHeight = GridManager.Instance.gridSize;
        // The number of points for the line renderer is the height times 2 plus 1 for the starting point
        line.positionCount = boardHeight * 2 + 1;
        List<Vector3> points = new List<Vector3>();

        float y = 0.28f;
        float x = -0.50f;
        points.Add(new Vector3(x, y, 0));
        for (int i = 0; i < boardHeight; i++)
        {
           
            x = x + 0.455f;
            y = 0.57f; ;
            points.Add(new Vector3(x, y, 0));
            Debug.Log("i is" + i + "bh is" + boardHeight);

            if (i == boardHeight - 1)
            {
                y = 0.25f;
                x += 0.5f;
                points.Add(new Vector3(x, y, 0));

            }
            else
            {

                y = 0.30f;
                x += 0.455f;
                points.Add(new Vector3(x, y, 0));
            }
            
        }

        // Apply the points to the line renderer
        line.SetPositions(points.ToArray());
    }
}
