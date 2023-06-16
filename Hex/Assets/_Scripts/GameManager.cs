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
        public GameObject replayButton;
        public GameObject traceButton;

        public enum PlayerType { Human, AI }
        public enum AIDifficulty { Easy, Medium, Hard }

        private PlayerType Player1Type;
        private PlayerType Player2Type;
        public AIDifficulty Player1AILevel; // add AI difficulty level
        public AIDifficulty Player2AILevel;

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
            if (Player1Type == PlayerType.Human && Player2Type == PlayerType.Human)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void SwitchPlayer()
        {
            if (GridManager.Instance.isHighlighted)
            {
                GridManager.Instance.onClick();
            }
            playerSwitchCount++;
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
        private List<Vector2> allChosenMoves = new List<Vector2>();

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
            AIDifficulty aiDifficulty = CurrentPlayer == 1 ? Player1AILevel : Player2AILevel;
            allChosenMoves.Add(chosenMove);
                            foreach (Vector2 move in allChosenMoves)
                            {
                            Debug.Log($"Chosen move: [{move.x}, {move.y}]");
                            }

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

            GameUtils gameUtils = new GameUtils();
            (int winner, List<(int, int)> path) = gameUtils.CheckWin(GridManager.Instance.tiles, CurrentPlayer);

            if (winner != 0)
            {
                replayButton.SetActive(true);
                traceButton.SetActive(false);
                StartCoroutine(Tile.WinColors(path, CurrentPlayer));
                PlayerTurnText.win = true;
                Tile.SetClickable();

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

public void RecordOpponentMove(int x, int y)
{
    if (GetPlayerType() == PlayerType.Human)
    {
        Vector2 newMove = new Vector2(x, y);
        opponentMoves.Add(newMove);

        // Add the new move to allChosenMoves only if it doesn't already exist
        if (!allChosenMoves.Contains(newMove))
        {
            allChosenMoves.Add(newMove);
        }
    }

    foreach (Vector2 move in allChosenMoves)
    {
        Debug.Log($"Chosen move: [{move.x}, {move.y}]");
    }
}

public void PrintAllChosenMoves()
{
    foreach (Vector2 move in allChosenMoves)
    {
        Debug.Log($"Chosen move: [{move.x}, {move.y}]");
    }

    ResetGameState();

    StartCoroutine(PlaceMovesWithDelay());
}

private Color[] playerColors = { Color.red, Color.blue };

        private IEnumerator PlaceMovesWithDelay()
        {
            int currentPlayerIndex = 0;

    foreach (var move in allChosenMoves)
    {
        yield return new WaitForSeconds(0.5f);

        int x = (int)move.x;
        int y = (int)move.y;

        GridManager.Instance.tiles[x][y].Owner = CurrentPlayer;
        GridManager.Instance.tiles[x][y].GetComponent<SpriteRenderer>().color = playerColors[currentPlayerIndex];

        currentPlayerIndex = (currentPlayerIndex + 1) % playerColors.Length;
    }

    Tile.SetClickable();
}

/*
private IEnumerator PlaceMovesWithDelay()
{
    foreach (var move in allChosenMoves)
    {
        yield return new WaitForSeconds(0.5f);

        GridManager.Instance.tiles[(int)move.x][(int)move.y].Owner = CurrentPlayer;
        GridManager.Instance.tiles[(int)move.x][(int)move.y].GetComponent<SpriteRenderer>().color = CurrentPlayer == 1 ? Color.red : Color.blue;
    }

    Tile.SetClickable();
}
*/

public List<Vector2> GetAllChosenMoves()
{
    PrintAllChosenMoves();
    return allChosenMoves;
}

public void ResetGameState()
{
    // Reset the game state to the standard
    UpdateGameState(GameState.GenerateGrid);
}

/*
public void oneMoveAhead()
{
    if (!isCoroutinePaused)
    {
        if (placeMovesCoroutine != null)
        {
            StopCoroutine(placeMovesCoroutine);
            placeMovesCoroutine = null;
        }

        StartCoroutine(PlaceMovesWithDelay());
    }
    else
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % playerColors.Length;
        isCoroutinePaused = false;
        StartCoroutine(PlaceMovesWithDelay());
    }
}

public void oneMoveBack()
{
    if (placeMovesCoroutine != null)
    {
        StopCoroutine(placeMovesCoroutine);
        placeMovesCoroutine = null;
    }

    int lastMoveIndex = allChosenMoves.Count - 1;

    if (lastMoveIndex >= 0)
    {
        Vector2 lastMove = allChosenMoves[lastMoveIndex];
        int x = (int)lastMove.x;
        int y = (int)lastMove.y;

        GridManager.Instance.tiles[x][y].Owner = 0; // Reset the owner of the tile
        GridManager.Instance.tiles[x][y].GetComponent<SpriteRenderer>().color = Color.white; // Reset the color of the tile

        allChosenMoves.RemoveAt(lastMoveIndex); // Remove the last move from the list
    }

    Tile.SetClickable();
}
*/

        public void UpdatePlayerTypes()
        {
            String playerType1 = MainMenuManager.Player1Type;
            if (playerType1 == "Human")
            {
                Player1Type = PlayerType.Human;

            }
            else if (playerType1 == "AI Easy")
            {
                Player1Type = PlayerType.AI;
                Player1AILevel = AIDifficulty.Easy;
            }
            else if (playerType1 == "AI Medium")
            {
                Player1Type = PlayerType.AI;
                Player1AILevel = AIDifficulty.Medium;
            }
            else if (playerType1 == "AI Hard")
            {
                Player1Type = PlayerType.AI;
                Player1AILevel = AIDifficulty.Hard;
            }
            String playerType2 = MainMenuManager.Player2Type;
            if (playerType2 == "Human")
            {
                Player2Type = PlayerType.Human;
            }
            else if (playerType2 == "AI Easy")
            {
                Player2Type = PlayerType.AI;
                Player2AILevel = AIDifficulty.Easy;
            }
            else if (playerType2 == "AI Medium")
            {
                Player2Type = PlayerType.AI;
                Player2AILevel = AIDifficulty.Medium;
            }
            else if (playerType2 == "AI Hard")
            {
                Player2Type = PlayerType.AI;
                Player2AILevel = AIDifficulty.Hard;
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
