    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.SceneManagement;


    public class GameManager : MonoBehaviour
    {

        GameUtils gameUtils = new GameUtils();

        public static GameManager Instance;

        public GameState State;
        public static bool notHumanTurn = false;

        public enum PlayerType { Human, AI }
        public enum AIDifficulty { Easy, Medium, Hard }

        public PlayerType Player1Type;
        public PlayerType Player2Type;
        public AIDifficulty AILevel; // add AI difficulty level

        public static int CurrentPlayer;

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
                        if (gameUtils.CheckWin(tiles, opponent) == opponent)
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

        public void SwitchPlayer()
        {
            notHumanTurn = false;
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
            /*
            if (MainMenuManager.gridSize == 3)
            {
                if (Player2Type == PlayerType.AI)
                {

                    // Specific strategy for 3x3 grid VERTICAL
                    if (opponentMoves.Count == 0)
                    {
                        chosenMove = new Vector2(1, 1); // Center position
                    }
                    else if (opponentMoves.Count == 1)
                    {
                        if (opponentMoves[0].y == 0) // Top row
                        {
                            chosenMove = GridManager.Instance.tiles[1][0].Owner == 0 ? new Vector2(1, 0) : new Vector2(2, 0);
                        }
                        else if (opponentMoves[0].y == 1) // Middle row
                        {
                        
                        
                            {
                                chosenMove = GridManager.Instance.tiles[1][0].Owner == 0 ? new Vector2(1, 0) : new Vector2(2, 0);
                            }
                        }
                        else // opponentMoves[0].y == 2 Bottom row
                        {
                            chosenMove = GridManager.Instance.tiles[0][2].Owner == 0 ? new Vector2(0, 2) : new Vector2(1, 2);
                        }
                        secondMove = chosenMove; // Record the second move of AI
                    }
                    else // opponentMoves.Count == 2
                    {
                        if (opponentMoves[0].y == 0) // Top row
                        {
                            chosenMove = GridManager.Instance.tiles[0][2].Owner == 0 ? new Vector2(0, 2) : new Vector2(1, 2);
                        }
                        else // opponentMoves[0].y == 2 Bottom row
                        {
                            chosenMove = GridManager.Instance.tiles[1][0].Owner == 0 ? new Vector2(1, 0) : new Vector2(2, 0);
                        }
                    }

                    // If the chosen tile is not available, switch to the default random strategy
                    if (GridManager.Instance.tiles[(int)chosenMove.x][(int)chosenMove.y].Owner != 0)
                    {
                        chosenMove = GetRandomAvailableTile();
                    }


                }
                else
                // HORIZONTAL ATTEMPT HERE 
                {
                    // Specific strategy for 3x3 grid
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
            else if (MainMenuManager.gridSize == 2)
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
                    if (GridManager.Instance.tiles[0][1].Owner == 0)
                    {
                        chosenMove = new Vector2(0, 1);
                    }
                    else if (GridManager.Instance.tiles[1][1].Owner == 0)
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
                // For other grid sizes, first try to block the opponent, then make a random move if no blocking is needed.
                Vector2? blockingMove = TryBlockOpponentMove();
                if (blockingMove.HasValue)
                {
                    chosenMove = blockingMove.Value;
                }
                else
                {
                    chosenMove = GetRandomAvailableTile();
                }
            }
            */
            // Make the chosen move

            GridManager.Instance.tiles[(int)chosenMove.x][(int)chosenMove.y].Owner = CurrentPlayer;
            GridManager.Instance.tiles[(int)chosenMove.x][(int)chosenMove.y].GetComponent<SpriteRenderer>().color = CurrentPlayer == 1 ? Color.red : Color.blue;

            // Record AI's move
            aiMoves.Add(chosenMove);

            GameUtils gameUtils = new GameUtils();
            int winner = gameUtils.CheckWin(GridManager.Instance.tiles, CurrentPlayer);

            if (winner != 0)
            {
                Debug.Log("Player " + winner + " wins!");

                Tile.SetClickable();

            }

            // Log AI's move
            Debug.Log($"Player {CurrentPlayer} clicked at array position [{chosenMove.x}, {chosenMove.y}]");
            SwitchPlayer();
        }

        // Method to get a random available tile
        private Vector2 GetRandomAvailableTile()
        {
            // List of all available tiles, prioritized by their proximity to the center
            List<Vector2> availableTiles = new List<Vector2>();
            List<Vector2> priorityTiles = new List<Vector2>();

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
                        // If this tile is at the center or immediately adjacent to the center
                        if (Math.Abs(x - centerX) <= 1 && Math.Abs(y - centerY) <= 1)
                        {
                            priorityTiles.Add(new Vector2(x, y));
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
            // If there are no priority tiles, select a random available tile
            else
            {
                return availableTiles[UnityEngine.Random.Range(0, availableTiles.Count)];
            }
        }

        // Record the opponent's move after every player action
        public void RecordOpponentMove(int x, int y)
        {
            if (GetPlayerType() == PlayerType.Human)
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
                    CurrentPlayer = 1;
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
