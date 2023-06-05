using UnityEngine;
using TMPro;

public class WinnerText : MonoBehaviour
{
    private TMP_Text winningPlayerText;

    private void Start()
    {
        winningPlayerText = GetComponent<TMP_Text>();

        string winningPlayerName = PlayerPrefs.GetString("WinningPlayerName", "Player X");
        winningPlayerText.text = winningPlayerName + " Wins";
    }
}
