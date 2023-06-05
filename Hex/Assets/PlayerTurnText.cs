using UnityEngine;
using TMPro;

public class PlayerTurnText : MonoBehaviour
{
    private TMP_Text playerTurnText;

    private void Start()
    {
    playerTurnText = GetComponent<TMP_Text>();

    string winningPlayerName = PlayerPrefs.GetString("WinningPlayerName", "Player X");
    playerTurnText.text = winningPlayerName + "'s Turn";
    }

    private void Update()
    {
        int currentPlayer = GameManager.CurrentPlayer;

        string player1Name = PlayerPrefs.GetString("Player1Name", "Player 1");
        string player2Name = PlayerPrefs.GetString("Player2Name", "Player 2");

        if (currentPlayer == 1)
        {
            playerTurnText.text = player1Name + "'s Turn";
        }
        else if (currentPlayer == 2)
        {
            playerTurnText.text = player2Name + "'s Turn";
        }
    }
}
