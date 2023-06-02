using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State;

    public enum PlayerType { Human, AI }

    public PlayerType Player1Type = PlayerType.Human;
    public PlayerType Player2Type = PlayerType.AI;

    public static int CurrentPlayer { get; private set; } = 2; // Start with AI

    private void Awake()
    {
        Instance = this;
    }

    public void SwitchPlayer()
    {
        CurrentPlayer = CurrentPlayer == 1 ? 2 : 1;

        if (GetPlayerType() == PlayerType.AI)
        {
            StartCoroutine(AIMove());
        }
    }

    public PlayerType GetPlayerType()
    {
        if (CurrentPlayer == 1)
        {
            return Player1Type;
        }
        else
        {
            return Player2Type;
        }
    }

    private List<Vector2> opponentMoves = new List<Vector2>();
    private List<Vector2> aiMoves = new List<Vector2>();


    private Vector2 secondMove;
    private IEnumerator AIMove()
    {
        yield return new WaitForSeconds(1); // wait for 1 second before AI makes move

        Vector2 chosenMove;




        if (GridManager.Instance.gridSize == 3)
        {
            // Specific strategy for 3x3 grid
            if (opponentMoves.Count == 0)
            {
                chosenMove = new Vector2(1, 1); // Center position
            }
            else if (opponentMoves.Count == 1)
            {
                if (opponentMoves[0].y == 0) // Top row
                {
                    chosenMove = GridManager.Instance.tiles[1, 0].Owner == 0 ? new Vector2(1, 0) : new Vector2(2, 0);
                }
                else if (opponentMoves[0].y == 1) // Middle row
                {
                    if (secondMove.y == 0) // If AI's second move was in the top row
                    {
                        chosenMove = GridManager.Instance.tiles[0, 2].Owner == 0 ? new Vector2(0, 2) : new Vector2(1, 2);
                    }
                    else if (secondMove.y == 2) // If AI's second move was in the bottom row
                    {
                        chosenMove = GridManager.Instance.tiles[1, 0].Owner == 0 ? new Vector2(1, 0) : new Vector2(2, 0);
                    }
                    else
                    {
                        chosenMove = GridManager.Instance.tiles[1, 0].Owner == 0 ? new Vector2(1, 0) : new Vector2(2, 0);
                    }
                }
                else // opponentMoves[0].y == 2 Bottom row
                {
                    chosenMove = GridManager.Instance.tiles[0, 2].Owner == 0 ? new Vector2(0, 2) : new Vector2(1, 2);
                }
                secondMove = chosenMove; // Record the second move of AI
            }
            else // opponentMoves.Count == 2
            {
                if (opponentMoves[0].y == 0) // Top row
                {
                    chosenMove = GridManager.Instance.tiles[0, 2].Owner == 0 ? new Vector2(0, 2) : new Vector2(1, 2);
                }
                else // opponentMoves[0].y == 2 Bottom row
                {
                    chosenMove = GridManager.Instance.tiles[1, 0].Owner == 0 ? new Vector2(1, 0) : new Vector2(2, 0);
                }
            }

            // If the chosen tile is not available, switch to the default random strategy
            if (GridManager.Instance.tiles[(int)chosenMove.x, (int)chosenMove.y].Owner != 0)
            {
                chosenMove = GetRandomAvailableTile();
            }
        }
        else if (GridManager.Instance.gridSize == 2)
        {
            // Specific strategy for 2x2 grid
            if (aiMoves.Count == 0)
            {
                // Always take the cell (1,0)
                chosenMove = new Vector2(1, 0);
            }
            else
            {
                // pick either (0,1) or (1,1) depending on which one is available
                if (GridManager.Instance.tiles[0, 1].Owner == 0)
                {
                    chosenMove = new Vector2(0, 1);
                }
                else if (GridManager.Instance.tiles[1, 1].Owner == 0)
                {
                    chosenMove = new Vector2(1, 1);
                }
                else
                {
                    // Default random strategy
                    chosenMove = GetRandomAvailableTile();
                }
            }
        }
        else
        {
            // Default random strategy
            chosenMove = GetRandomAvailableTile();
        }

        // Make the chosen move

        GridManager.Instance.tiles[(int)chosenMove.x, (int)chosenMove.y].Owner = CurrentPlayer;
        GridManager.Instance.tiles[(int)chosenMove.x, (int)chosenMove.y].GetComponent<SpriteRenderer>().color = CurrentPlayer == 1 ? Color.blue : Color.red;

        // Record AI's move
        aiMoves.Add(chosenMove);

        // Log AI's move
        Debug.Log($"Player {CurrentPlayer} clicked at array position [{chosenMove.x}, {chosenMove.y}]");
        SwitchPlayer();
    }




    // Method to get a random available tile
    private Vector2 GetRandomAvailableTile()
    {
        Tile[,] tiles = GridManager.Instance.tiles;
        List<(Tile tile, Vector2 pos)> availableTiles = new List<(Tile tile, Vector2 pos)>();
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                if (tiles[x, y].Owner == 0) // tile is unclaimed
                {
                    availableTiles.Add((tiles[x, y], new Vector2(x, y)));
                }
            }
        }
        int randomIndex = Random.Range(0, availableTiles.Count);
        return availableTiles[randomIndex].pos;
    }


    // Record the opponent's move after every player action
    public void RecordOpponentMove(int x, int y)
    {
        if (CurrentPlayer == 1)
        {
            opponentMoves.Add(new Vector2(x, y));
        }
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
                if (CurrentPlayer == 2) // if AI is the first player, make the first move
                {
                    StartCoroutine(AIMove());
                }
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
