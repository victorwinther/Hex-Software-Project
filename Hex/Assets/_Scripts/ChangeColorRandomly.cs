using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorRandomly : MonoBehaviour
{
    int counter = 1;

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (counter % 2 == 0)
            {
                this.GetComponent<Renderer>().material.color =
                    Color.blue;
                counter++;
            }
            else {
                this.GetComponent<Renderer>().material.color =
                    Color.red;
                counter++;
            }
        }
    }
}
