using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPTColorChanger : MonoBehaviour
{

    // public Material mat;

    public static int colorDecidingCounter = 0;
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0))
        {
            

            
            if (colorDecidingCounter % 2 == 0)
            {
                // if statement makes sure we can only change the color of the tile,
                // if it has not been touched yet(aka is white) 
                if (this.GetComponent<Renderer>().material.color == Color.white)
                {
                    this.GetComponent<Renderer>().material.color = Color.green;
                    colorDecidingCounter++;
                }
                
            }
            else 
            {
                // if statement makes sure we can only change the color of the tile,
                // if it has not been touched yet(aka is white) 
                if (this.GetComponent<Renderer>().material.color == Color.white)
                {
                    this.GetComponent<Renderer>().material.color = Color.red;
                    colorDecidingCounter++;
                }
            }
            Debug.Log("TEST" + colorDecidingCounter);
            
        }
    }




}
