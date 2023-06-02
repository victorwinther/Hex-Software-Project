using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateGameState(GameState.GenerateGrid);
    }
    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.GenerateGrid:
                GridManager.Instance.CreateGrid();
                break;
            case GameState.PlayerTurn:
                break;
            case GameState.EnemyTurn: 
                break;
            case GameState.Victory:
                break;
            case GameState.Lose:
                break;
           
        }
    }

    public int EndScene;

    public void EndGame()
    {
        SceneManager.LoadScene(EndScene);
    }
}
public enum GameState
{
    GenerateGrid,
    PlayerTurn,
    EnemyTurn,
    Victory,
    Lose
}