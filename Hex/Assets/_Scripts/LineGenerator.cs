using UnityEngine;

public class LineGenerator : MonoBehaviour
{
    public LineRenderer linePrefab; // Reference to the LineRenderer prefab
    public float distanceBetweenLines = 1f; // Distance between each line
    public int numberOfLines = 5; // Number of lines to generate

    void Start()
    {
        // Loop to create the specified number of lines
        for (int i = 0; i < numberOfLines; i++)
        {
            // Instantiate a new LineRenderer and set its properties
            LineRenderer line = Instantiate(linePrefab, transform);
            line.positionCount = 2; // Set the number of positions for the line (start and end points)
            line.SetPosition(0, new Vector3(i * distanceBetweenLines, 0f, 0f)); // Set the start position of the line
            line.SetPosition(1, new Vector3((i + 1) * distanceBetweenLines, 0f, 0f)); // Set the end position of the line
        }
    }
}