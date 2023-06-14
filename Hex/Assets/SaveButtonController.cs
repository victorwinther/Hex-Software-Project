using UnityEngine;
using TMPro;
using System;
using System.Linq;

public class SaveButtonController : MonoBehaviour
{
    public void SaveGame()
    {
        SaveData data = new SaveData(GridManager.Instance.gridSize, GameManager.CurrentPlayer, GridManager.Instance.tiles);

        // Convert SaveData to a JSON string
        string json = JsonUtility.ToJson(data);

        // Save the string to PlayerPrefs
        PlayerPrefs.SetString("SavedGame", json);
        PlayerPrefs.Save();

        Debug.Log("Game Saved!");
    }
}
