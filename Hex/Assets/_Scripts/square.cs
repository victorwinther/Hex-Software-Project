using UnityEngine;

public class SquareLineRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetLinePositions();
    }

    private void SetLinePositions()
    {
        Vector3[] positions = new Vector3[5];
        // Define your square's corner positions
        positions[0] = new Vector3(-1f, 0f, -1f);
        positions[1] = new Vector3(1f, 0f, -1f);
        positions[2] = new Vector3(1f, 0f, 1f);
        positions[3] = new Vector3(-1f, 0f, 1f);
        positions[4] = positions[0]; // Close the loop

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }
}