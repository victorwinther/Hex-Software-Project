using UnityEngine;

public class Scale : MonoBehaviour
{
    public Vector3 newScale; // The new scale you want to set

    void Start()
    {
        Debug.Log(MainMenuManager.gridSize);

        // Access the transform component of the GameObject
        Transform backgroundTransform = gameObject.transform;

        if (MainMenuManager.gridSize > 30)
        {
            backgroundTransform.localScale = new Vector3(5f, 5f, 1f);
        }
        else
        {
            backgroundTransform.localScale = (new Vector3(1f, 1f, 1f));
        }

       
    }
}

