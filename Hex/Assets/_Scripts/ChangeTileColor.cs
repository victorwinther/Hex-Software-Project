using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTileColor : MonoBehaviour

{
    public Material[] materials;
    public Renderer rend;

    private int index = 1;


    // Start is called before the first frame update
    void Start()

    {
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        
    }

    void OnMouseDown()
    {
        if (materials.Length == 0)  {
            return;
        }

        if (Input.GetMouseButtonDown (0))
        {
            index += 1;
            if (index == materials.Length + 1)
            {
                index = 1;
            }
            print(index); // used for debugging

            rend.sharedMaterial = materials[index - 1]; //this sets the material color values inside the index
        }
    }
}
