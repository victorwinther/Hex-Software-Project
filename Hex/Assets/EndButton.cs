using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndButton : MonoBehaviour
{
    public int endSceneIndex; // Index of the end scene

    public void EndGame()
    {
        // Load the end scene
        Tile.AllowClickable();
        GameManager.notHumanTurn = false;
        PlayerTurnText.win = false;
        MainMenuManager.Player1Type = "Human";
        MainMenuManager.Player2Type = "Human";
        SceneManager.LoadScene(endSceneIndex);
    }
}
