using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{

    public int gameStartScene;
    public TMP_InputField player1NameInput;
    public TMP_InputField player2NameInput;

    public InputField inputField;
    public Text output;
    public GridManager gridManager;
    public static int gridSize = 11;

    public TextMeshProUGUI output1;

    public static string Player1Type = "Human";
    public static string Player2Type = "Human";

    public void NumberOfTiles(string value)
{
    output.text = inputField.text;
        gridSize = 11;

    if (int.TryParse(inputField.text, out int newGridSize))
    {
        gridSize = newGridSize;
       
    }
}

  public void HandleInputData(int val)
    {
        if (val == 0)
        {
            Player1Type = "Human";
        }
        if (val == 1)
        {
            Player1Type = "AI";
        }
    }

    public void HandleInputData2(int val)
    {
        if (val == 0)
        {
            Player2Type = "Human";
        }
        if (val == 1)
        {
            Player2Type = "AI";
        }
    }

    public void StartGame()
    {
        
    string player1Name = string.IsNullOrEmpty(player1NameInput.text) ? "Player 1" : player1NameInput.text;
    string player2Name = string.IsNullOrEmpty(player2NameInput.text) ? "Player 2" : player2NameInput.text;

    // Store the player names for the game scene
    PlayerPrefs.SetString("Player1Name", player1Name);
    PlayerPrefs.SetString("Player2Name", player2Name);

    SceneManager.LoadScene(gameStartScene);
     
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

}
