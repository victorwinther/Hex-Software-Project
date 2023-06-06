using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lower_blue : MonoBehaviour
{
    public LineRenderer line;
    public int boardHeight; // Set this to your actual board height

    void Start()
    {
        boardHeight = MainMenuManager.gridSize;
        //boardHeight = GridManager.Instance.gridSize;
        // The number of points for the line renderer is the height times 2 plus 1 for the starting point
        line.positionCount = boardHeight * 2;
        List<Vector3> points = new List<Vector3>();

        float y = ((boardHeight - 1) * -0.77f)  - (0.55f) - 0.04f;
        float x = (boardHeight - 1) * 0.45f;
        points.Add(new Vector3(x, y, 0));
        for (int i = 0; i < boardHeight; i++)
        {
            x += 0.4525f;
            points.Add(new Vector3(x, y+0.3f, 0));

            x += 0.4525f;
            points.Add(new Vector3(x, y, 0));
        }

        // Apply the points to the line renderer
        line.SetPositions(points.ToArray());
    }
}
