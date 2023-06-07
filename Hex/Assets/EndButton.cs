using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndButton : MonoBehaviour
{
    public int endSceneIndex; // Index of the end scene

    public void EndGame()
    {
        // Check if there is a winning player
        bool hasWinner = PlayerPrefs.HasKey("WinningPlayerName");

        if (!hasWinner)
        {
            // No winner, set "No Winner" as the winning player name
            PlayerPrefs.SetString("WinningPlayerName", "No Winner");
        }

        // Load the end scene
        SceneManager.LoadScene(endSceneIndex);
    }
}
