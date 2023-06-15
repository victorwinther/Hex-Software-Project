    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;


public class GameManager : MonoBehaviour
    {
    public Text timerText;
    public bool isGameOver = false;

    //public ParticleSystem fireworks;

    private List<Vector2> allMoves = new List<Vector2>();
    public void RecordMove(int x, int y)
    {
        allMoves.Add(new Vector2(x, y));

        Debug.Log($"Player {CurrentPlayer} moved to [{x}, {y}]");

        // Debug the entire list of moves every time a move is made
        Debug.Log("List of all moves made so far:");
        foreach (Vector2 move in allMoves)
        {
            Debug.Log($"Move at [{move.x}, {move.y}]");
        }
    }

    void Update()
    {
        // Check if 'u' key is pressed
        if (Input.GetKeyDown(KeyCode.U))
        {
            UndoLastMove();
        }

        if (isBlitzMode)
        {
            if (!isGameOver) // Only update if the game is not over
            {
                timeLeft -= Time.deltaTime; // Decrease the time left every frame.
                                            // Update the text field.
                timerText.text = "Time Left: " + Mathf.Round(timeLeft).ToString();

                if (timeLeft <= 0f)
                {
                    // Time's up. Make a random move.
                    MakeRandomMove();
                    // Reset the timer.
                    timeLeft = timeForTurn;
                    SwitchPlayer();
                }
            }
        }
    }

    void UndoLastMove()
    {
        if (allMoves.Count > 0)
        {
            // Get the last move from the allMoves list
            Vector2 lastMove = allMoves[allMoves.Count - 1];

            // Convert the vector2 coordinates to integers
            int x = (int)lastMove.x;
            int y = (int)lastMove.y;

            // Get the tile at the last move's position
            Tile tile = GridManager.Instance.tiles[x][y];

            // Reset the tile's owner and color
            tile.Owner = 0;
            tile.GetComponent<SpriteRenderer>().color = Color.white;

            // Remove the last move from the allMoves list
            allMoves.RemoveAt(allMoves.Count - 1);

            // Switch the current player
            GameManager.Instance.SwitchPlayer();

        }
        else
        {
            Debug.Log("No moves to undo");
        }
    }

    public float timeForTurn = 5f; // Change this value as per the required turn time in seconds.
    private float timeLeft;
    public bool isBlitzMode = false;


    private void MakeRandomMove()
    {
        // List of all available tiles
        List<Vector2> availableTiles = new List<Vector2>();

        // Iterate through all the tiles
        for (int x = 0; x < MainMenuManager.gridSize; x++)
        {
            for (int y = 0; y < MainMenuManager.gridSize; y++)
            {
                // If this tile is not owned yet
                if (GridManager.Instance.tiles[x][y].Owner == 0)
                {
                    availableTiles.Add(new Vector2(x, y));
                }
            }
        }

        // Pick a random available tile
        if (availableTiles.Count > 0)
        {
            Vector2 chosenMove = availableTiles[UnityEngine.Random.Range(0, availableTiles.Count)];

            // Make the chosen move
            GridManager.Instance.tiles[(int)chosenMove.x][(int)chosenMove.y].Owner = CurrentPlayer;
            GridManager.Instance.tiles[(int)chosenMove.x][(int)chosenMove.y].GetComponent<SpriteRenderer>().color = CurrentPlayer == 1 ? Color.red : Color.blue;

            // Record the move
            RecordMove((int)chosenMove.x, (int)chosenMove.y);

            // Do not switch player here as it's not a player initiated action
        }
        else
        {
            Debug.Log("No empty tiles left!");
        }
    }

    public void StartBlitzMode()
    {
        isBlitzMode = true;
    }

    // Call this method to stop the Blitz mode.
    public void StopBlitzMode()
    {
        isBlitzMode = false;
    }



    GameUtils gameUtils = new GameUtils();

        public static GameManager Instance;

        public GameState State;
        public static bool notHumanTurn = false;

        public enum PlayerType { Human, AI }
        public enum AIDifficulty { Easy, Medium, Hard }

        private PlayerType Player1Type;
        private PlayerType Player2Type;
        public AIDifficulty AILevel; // add AI difficulty level

        public static int CurrentPlayer;
        public static int playerSwitchCount = 0;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Method to attempt blocking the opponent
        private Vector2? TryBlockOpponentMove()
        {
            int opponent = CurrentPlayer == 1 ? 2 : 1;

            GameUtils gameUtils = new GameUtils();
            Tile[][] tiles = GridManager.Instance.tiles;

            for (int r = 0; r < tiles.Length; r++)
            {
                for (int c = 0; c < tiles[0].Length; c++)
                {
                    if (tiles[r][c].Owner == 0) // if the cell is not occupied
                    {
                        tiles[r][c].Owner = opponent; // Temporarily assign it to the opponent
                    (int winner, List<(int, int)> path) = gameUtils.CheckWin(GridManager.Instance.tiles, opponent);
                    if (winner == opponent)
                        {
                            tiles[r][c].Owner = 0; // Revert the change
                            return new Vector2(r, c); // Found a move that could block the opponent
                        }
                        tiles[r][c].Owner = 0; // Revert the change
                    }
                }
            }
            return null; // No blocking move found
        }
        public bool BothHuman()
        {
        if(Player1Type == PlayerType.Human && Player2Type == PlayerType.Human)
        {
            return true;
        } else
        {
            return false;
        }
        }
        public void SwitchPlayer()
        {
            playerSwitchCount++;
            notHumanTurn = false;

            CurrentPlayer = CurrentPlayer == 1 ? 2 : 1;

        if (GetPlayerType() == PlayerType.AI)
            {
                StartCoroutine(AIMove());
            }

        timeLeft = timeForTurn;
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

        public void StopCorutine()
        {
            StopAllCoroutines();
        }
    
        private Vector2 secondMove;
        private IEnumerator AIMove()
        {
            yield return new WaitForSeconds(0.5f); // wait for 1 second before AI makes move

            //default value.
            Vector2 chosenMove = GetRandomAvailableTile();

            switch (AILevel)
            {
                case AIDifficulty.Easy:
                    // For easy AI, always make a random move.
                    chosenMove = GetRandomAvailableTile();  
                    break;
                case AIDifficulty.Medium:
                    // For medium AI, try to block the opponent, then make a random move if no blocking is needed.
                    Vector2? blockingMove = TryBlockOpponentMove();
                    chosenMove = blockingMove.HasValue ? blockingMove.Value : GetRandomAvailableTile();
                    break;
                case AIDifficulty.Hard:
                // For hard AI, implement your own strategy here. This example uses the same strategy as the medium level.
                if (MainMenuManager.gridSize == 3)
                {
                    if (Player1Type == PlayerType.AI)
                    {
                        if (opponentMoves.Count == 0)
                        {
                            chosenMove = new Vector2(1, 1); // Center position
                        }
                        else if (opponentMoves.Count == 1)
                        {
                            if (opponentMoves[0].x == 2) // Right column
                            {
                                chosenMove = GridManager.Instance.tiles[2][0].Owner == 0 ? new Vector2(2, 0) : new Vector2(2, 1);
                            }
                            else if (opponentMoves[0].x == 1) // Middle column
                            {


                                {
                                    chosenMove = GridManager.Instance.tiles[2][0].Owner == 0 ? new Vector2(2, 0) : new Vector2(2, 1);
                                }
                            }
                            else // opponentMoves[0].y == 2 left column
                            {
                                chosenMove = GridManager.Instance.tiles[0][1].Owner == 0 ? new Vector2(0, 1) : new Vector2(0, 2);
                            }
                            secondMove = chosenMove; // Record the second move of AI
                        }
                        else // opponentMoves.Count == 2
                        {
                            if (opponentMoves[0].x == 2) // Right column
                            {
                                chosenMove = GridManager.Instance.tiles[0][1].Owner == 0 ? new Vector2(0, 1) : new Vector2(0, 2);
                            }
                            else // opponentMoves[0].y == 2 left column
                            {
                                chosenMove = GridManager.Instance.tiles[2][0].Owner == 0 ? new Vector2(2, 0) : new Vector2(2, 1);
                            }
                        }

                        // If the chosen tile is not available, switch to the default random strategy
                        if (GridManager.Instance.tiles[(int)chosenMove.x][(int)chosenMove.y].Owner != 0)
                        {
                            chosenMove = GetRandomAvailableTile();
                        }
                    }

                }
                else
                {
                    blockingMove = TryBlockOpponentMove();
                    chosenMove = blockingMove.HasValue ? blockingMove.Value : GetRandomAvailableTile();
                }
                    break;
                default:
                    chosenMove = GetRandomAvailableTile();
                    break;
            }
        
            // Make the chosen move
            GridManager.Instance.tiles[(int)chosenMove.x][(int)chosenMove.y].Owner = CurrentPlayer;
            GridManager.Instance.tiles[(int)chosenMove.x][(int)chosenMove.y].GetComponent<SpriteRenderer>().color = CurrentPlayer == 1 ? Color.red : Color.blue;

            // Record AI's move
            aiMoves.Add(chosenMove);
            // Adds move to the list of all moves.  
            RecordMove((int)chosenMove.x, (int)chosenMove.y);

            GameUtils gameUtils = new GameUtils();
        (int winner, List<(int, int)> path) = gameUtils.CheckWin(GridManager.Instance.tiles, CurrentPlayer);

        if (winner != 0)
        {
            Debug.Log("Shortest path:");
            foreach ((int x, int y) in path)
            {
                GridManager.Instance.tiles[x][y].GetComponent<SpriteRenderer>().color = Color.cyan;
               
                Debug.Log($"({x}, {y})");
            }
           
            PlayerTurnText.win = true;
            Tile.SetClickable();
            isGameOver = true;
            // Hide timer text
            timerText.gameObject.SetActive(false);

        }
        else { SwitchPlayer(); }

          
            Debug.Log($"Player {CurrentPlayer} clicked at array position [{chosenMove.x}, {chosenMove.y}]");
            
        }

    private Vector2 GetRandomAvailableTile()
    {
    // List of all available tiles, prioritized by their proximity to the center
    List<Vector2> availableTiles = new List<Vector2>();
    List<Vector2> priorityTiles = new List<Vector2>();
    List<Vector2> neighborTiles = new List<Vector2>();

    // Calculate the center of the grid
    float centerX = MainMenuManager.gridSize / 2;
    float centerY = MainMenuManager.gridSize / 2;

    // Iterate through all the tiles
    for (int x = 0; x < MainMenuManager.gridSize; x++)
    {
        for (int y = 0; y < MainMenuManager.gridSize; y++)
        {
            // If this tile is not owned yet
            if (GridManager.Instance.tiles[x][y].Owner == 0)
            {
                int neighborCount = GetOwnedNeighborCount(x, y);

                // If this tile is at the center or immediately adjacent to the center
                if (Math.Abs(x - centerX) <= 1 && Math.Abs(y - centerY) <= 1 && playerSwitchCount <= 2)
                {
                    priorityTiles.Add(new Vector2(x, y));
                }
                
                // If this tile has only one owned neighboring tile
                else if (neighborCount >= 1)
                {
                    neighborTiles.Add(new Vector2(x, y));
                }
                
                else
                {
                    availableTiles.Add(new Vector2(x, y));
                }
            }
        }
    }

    // If there are any priority tiles available, select one of those
    if (priorityTiles.Count > 0)
    {
        return priorityTiles[UnityEngine.Random.Range(0, priorityTiles.Count)];
    }
    // If there are neighbor tiles available, select one of those
    else if (neighborTiles.Count > 0)
    {
        return neighborTiles[UnityEngine.Random.Range(0, neighborTiles.Count)];
    }
    // If there are no priority tiles, neighbor tiles, or defensive tiles, select a random available tile
    else
    {
        return availableTiles[UnityEngine.Random.Range(0, availableTiles.Count)];
    }
}

private int GetOwnedNeighborCount(int x, int y)
{
    int count = 0;
    int[] dx = { -1, -1, 0, 0, 1, 1 };
    int[] dy = { -1, 0, -1, 1, 0, 1 };

    for (int i = 0; i < dx.Length; i++)
    {
        int nx = x + dx[i];
        int ny = y + dy[i];

        if (nx >= 0 && nx < MainMenuManager.gridSize && ny >= 0 && ny < MainMenuManager.gridSize)
        {
            if (GridManager.Instance.tiles[nx][ny].Owner == 1)
            {
                count++;
            }
        }
    }

    return count;
}

        // Record the opponent's move after every player action
        public void RecordOpponentMove(int x, int y)
        {
            if (GetPlayerType() == PlayerType.Human)
            {
                opponentMoves.Add(new Vector2(x, y));
            }
        }

        public void UpdatePlayerTypes()
    {
        if(MainMenuManager.Player1Type == "Human")
        {
            Player1Type = PlayerType.Human;
        }
        else {
            Player1Type = PlayerType.AI;
                }

        if (MainMenuManager.Player2Type == "Human")
        {
            Player2Type = PlayerType.Human;
        }
        else
        {
            Player2Type = PlayerType.AI;
        }
    }

        // Start is called before the first frame update
        void Start()
        {
            StartBlitzMode();
            UpdateGameState(GameState.GenerateGrid);
            // Initialize the timeLeft with timeForTurn at the start.
            timeLeft = timeForTurn;
        }
        public void UpdateGameState(GameState newState)
        {
            State = newState;

            switch (newState)
            {
                case GameState.GenerateGrid:
                    GridManager.Instance.CreateGrid();
                    CurrentPlayer = 1;
                     UpdatePlayerTypes();
                    if (Player1Type == PlayerType.AI) // if Player1 is AI, make the first move
                    {
                        notHumanTurn = true;
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
            string winningPlayerName = PlayerPrefs.GetString("Player" + CurrentPlayer + "Name", "Player " + CurrentPlayer);
            PlayerPrefs.SetString("WinningPlayerName", winningPlayerName);
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
