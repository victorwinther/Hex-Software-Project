using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Tile : MonoBehaviour {
    [SerializeField] private Color _baseColor = Color.white;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
 
    public void Init() {
        _renderer.color = Color.white;
    }
 
    void OnMouseEnter() {
        _highlight.SetActive(true);
        
    }
 
    void OnMouseExit()
    {
        _highlight.SetActive(false);
    }
    public void OnClick()
    {
        Debug.Log($"Clicked on tile at position {transform.position}");
    }

    public static int colorDecidingCounter = 0;

    private void OnMouseDown()
    {
        Debug.Log(_renderer.color);

        //_renderer.color = Color.green;
        // Extract the position of the clicked tile
        int x = int.Parse(gameObject.name.Split('|')[0].Substring(7));
        int y = int.Parse(gameObject.name.Split('|')[1]);
        Debug.Log("Tile clicked: " + gameObject.name);
        Vector2 searchVector = new Vector2(x, y);

        if (colorDecidingCounter % 2 == 0)
        {
            // if statement makes sure we can only change the color of the tile,
            // if it has not been touched yet(aka is white) 
            //if (_renderer.color != Color.green && _renderer.color != Color.red)
                if (_renderer.color == Color.white)
                {
                _renderer.color = Color.blue;
                colorDecidingCounter++;
            }

        }
        else
        {
            // if statement makes sure we can only change the color of the tile,
            // if it has not been touched yet(aka is white) 
            //if (_renderer.color != Color.red && _renderer.color != Color.green)
            if(_renderer.color == Color.white)
            {
                _renderer.color = Color.red;
                colorDecidingCounter++;
            }
        }


        bool doesExist = false;

        foreach (Vector2 vector in GridManager.Instance.clickedTiles)
        {
            if (vector == searchVector)
            {
                doesExist = true;
                break;
            }
        }

        if (doesExist)
        {
            Debug.Log("Vector2 exists in array");
        }
        else
        {
            Debug.Log("Vector2 does not exist in array");
            GridManager.Instance.clickedTiles.Add(new Vector2(x, y));
        }


        for (int i = 0; i < GridManager.Instance.clickedTiles.Count; i++)
        {
            Vector2 tilePos = GridManager.Instance.clickedTiles[i];
            Debug.Log("Tile at position (" + tilePos.x + ", " + tilePos.y + ")");
        }





    }
    private void OnTileClick(Transform hex)
    {
        // Extract the position of the clicked tile
        int x = int.Parse(hex.name.Split('|')[0].Substring(7));
        int y = int.Parse(hex.name.Split('|')[1]);

        // Store the position in an array or list
        // For example, you could create a List<Vector2> in the GridManager class
        // and add the position to it:
        


       
    }
}
