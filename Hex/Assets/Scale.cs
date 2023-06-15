using UnityEngine;

public class Scale : MonoBehaviour
{
    public Vector3 newScale; // The new scale you want to set

    void Start()
    {
        Debug.Log(MainMenuManager.gridSize);

        // Access the transform component of the GameObject
        Transform backgroundTransform = gameObject.transform;

    }
}

