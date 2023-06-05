using UnityEngine;
using TMPro;

public class WinnerText : MonoBehaviour
{
    private TMP_Text textMesh;

    private void Start()
    {
        textMesh = GetComponent<TMP_Text>();

        // Retrieve the winning player name from PlayerPrefs
        string winningPlayerName = PlayerPrefs.GetString("WinningPlayerName", "");

        // Check if there is a winning player
        if (!string.IsNullOrEmpty(winningPlayerName))
        {
            // Display the winning player's name
            textMesh.text = winningPlayerName + " Wins";
        }
        else
        {
            // No winning player, display "No Winner"
            textMesh.text = "No Winner";
        }
    }
}
