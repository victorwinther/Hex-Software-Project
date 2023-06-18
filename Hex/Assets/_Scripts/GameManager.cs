using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    private List<Vector2> allMoves = new List<Vector2>();
    GameUtils gameUtils = new GameUtils();

    public static GameManager Instance;

    public GameState State;
    public static bool notHumanTurn = false;
    public GameObject replayButton;
    public GameObject traceButton;
    public GameObject undoButton;
    public Image hexPrefab;
    private SpriteRenderer spriteRenderer;
    public enum PlayerType { Human, AI }
    public enum AIDifficulty { Easy, Medium, Hard }

    private PlayerType Player1Type;
    private PlayerType Player2Type;
    public AIDifficulty Player1AILevel; // add AI difficulty level
    public AIDifficulty Player2AILevel;

    public static int CurrentPlayer;
    public static int playerSwitchCount = 0;
    public (int winner, List<(int, int)> path) WinnerInfo;

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

    private void Update()
    {
        hexPrefab.color = CurrentPlayer == 1 ? Color.red : Color.blue;
    }

    public void RecordMove(int x, int y)
    {
        allMoves.Add(new Vector2(x, y));
    }

    public void UndoLastMove()
    {
        if (Player1Type == PlayerType.Human && Player2Type == PlayerType.Human)
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
                
            }
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
        Vector2 chosenMove = GetRandomAvailableTile2();
        AIDifficulty aiDifficulty = CurrentPlayer == 1 ? Player1AILevel : Player2AILevel;
        allChosenMoves.Add(chosenMove);
     

        switch (aiDifficulty)
        {

            case AIDifficulty.Easy:
                // For easy AI, always make a random move.
                break;
            case AIDifficulty.Medium:
                // For medium AI, try to block the opponent, then make a random move if no blocking is needed.
                Vector2? blockingMove = TryBlockOpponentMove();
                chosenMove = blockingMove.HasValue ? blockingMove.Value : GetRandomAvailableTile(false);
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
                            else if (opponentMoves[0].x == 0) // opponentMoves[0].y == 2 left column
                            {
                                chosenMove = GridManager.Instance.tiles[2][0].Owner == 0 ? new Vector2(2, 0) : new Vector2(2, 1);
                            }
                            else
                            {
                                if (secondMove.x == 0) { chosenMove = GridManager.Instance.tiles[2][0].Owner == 0 ? new Vector2(2, 0) : new Vector2(2, 1); }
                                else { chosenMove = GridManager.Instance.tiles[0][1].Owner == 0 ? new Vector2(0, 1) : new Vector2(0, 2); }
                            }
                        }

                        // If the chosen tile is not available, switch to the default random strategy
                        if (GridManager.Instance.tiles[(int)chosenMove.x][(int)chosenMove.y].Owner != 0)
                        {
                            chosenMove = GetRandomAvailableTile(true);
                        }
                    }

                }

                else
                {
                    blockingMove = TryBlockOpponentMove();
                    chosenMove = blockingMove.HasValue ? blockingMove.Value : GetRandomAvailableTile(true);
                }
                break;
            default:
                chosenMove = GetRandomAvailableTile2();
                break;
        }

        RecordMove((int)chosenMove.x, (int)chosenMove.y);
        // Make the chosen move
        GridManager.Instance.tiles[(int)chosenMove.x][(int)chosenMove.y].Owner = CurrentPlayer;
        GridManager.Instance.tiles[(int)chosenMove.x][(int)chosenMove.y].GetComponent<SpriteRenderer>().color = CurrentPlayer == 1 ? Color.red : Color.blue;

        // Record AI's move
        aiMoves.Add(chosenMove);


        WinnerInfo = gameUtils.CheckWin(GridManager.Instance.tiles, CurrentPlayer);

        if (WinnerInfo.winner != 0)
        {
            PlayerTurnText.player = WinnerInfo.winner;
            replayButton.SetActive(true);
            traceButton.SetActive(false);
            undoButton.SetActive(false);
            StartCoroutine(Tile.WinColors(WinnerInfo.path, CurrentPlayer));
            PlayerTurnText.win = true;
            Tile.SetClickable();

        }
        else { SwitchPlayer(); }
        
    }
    private Vector2 GetRandomAvailableTile2()
    {
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
        return availableTiles[UnityEngine.Random.Range(0, availableTiles.Count - 1)];
    }


    private Vector2 GetRandomAvailableTile(bool HardMode)
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
                    if (Math.Abs(x - centerX) <= 1 && Math.Abs(y - centerY) <= 1 && playerSwitchCount <= 1)

                    {
                        priorityTiles.Add(new Vector2(x, y));

                    }

                    // If this tile has only one owned neighboring tile
                    if (neighborCount >= 1)
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
        if (priorityTiles.Count > 1)
        {
            return priorityTiles[UnityEngine.Random.Range(0, priorityTiles.Count)];
        }
        // If there are neighbor tiles available, select one of those

        else if (neighborTiles.Count > 1)
        {
            if (HardMode)
            {
                HexTileGame game = new HexTileGame(GridManager.Instance.tiles);
                List<Hex> shortestPath = game.FindShortestPath(GameManager.CurrentPlayer);

                List<Vector2> shorestPathVector = new List<Vector2>();
                foreach (Hex hex in shortestPath)
                {
                    shorestPathVector.Add(new Vector2(hex.Position.Col, hex.Position.Row));

                }

                List<Vector2> commonVectors = neighborTiles.Intersect(shorestPathVector).ToList();

                if (commonVectors.Count > 0)
                {
                    return commonVectors[UnityEngine.Random.Range(0, commonVectors.Count - 1)];
                }
                else
                {
                    return availableTiles[UnityEngine.Random.Range(0, availableTiles.Count - 1)];
                }
            }
            else
            {
                return neighborTiles[UnityEngine.Random.Range(0, neighborTiles.Count - 1)];
            }
        }

        else
        {
            return availableTiles[UnityEngine.Random.Range(0, availableTiles.Count - 1)];
        }

    }


    // If there are no priority tiles, neighbor tiles, or defensive tiles, select a random available tile



    private int GetOwnedNeighborCount(int row, int col)
    {
        int count = 0;

        // Define the six possible directions for hex tile neighbors

        int[,] directions = new int[,]
        {
            { -1, 0 },   // Top
            { -1, 1 },   // Top-right
            { 0, 1 },    // Bottom-right
            { 1, 0 },    // Bottom
            { 1, -1 },   // Bottom-left
            { 0, -1 }    // Top-left
        };

        for (int i = 0; i < 6; i++)
        {
            int newRow = row + directions[i, 0];
            int newCol = col + directions[i, 1];

            // Check if the neighbor position is within the board boundaries
            if (newRow >= 0 && newRow < MainMenuManager.gridSize && newCol >= 0 && newCol < MainMenuManager.gridSize)
            {
                if (GridManager.Instance.tiles[newRow][newCol].Owner == CurrentPlayer)
                    count++;
            }
        }
        Console.Write("returned neighbors");
        return count;
    }


    private Coroutine placeMovesCoroutine;
    private int currentMoveIndex;
    private int savedMoveIndex;
    private bool isReplayPaused = false;
    private Color[] playerColors = { Color.red, Color.blue };
    public void RecordAIMove(int x, int y)
    {
        Vector2 newMove = new Vector2(x, y);
        aiMoves.Add(newMove);

        if (!allChosenMoves.Contains(newMove))
        {
            allChosenMoves.Add(newMove);
        } 
    }

    public void RecordOpponentMove(int x, int y)
    {
        Vector2 newMove = new Vector2(x, y);
        opponentMoves.Add(newMove);

        if (!allChosenMoves.Contains(newMove))
        {
            allChosenMoves.Add(newMove);
        }
    }

    public void PauseReplay()
    {
        if (placeMovesCoroutine != null)
        {
            isReplayPaused = true;
            StopCoroutine(placeMovesCoroutine);
            placeMovesCoroutine = null;
        }
    }

    public void ResumeReplay()
    {
        if (isReplayPaused)
        {
            isReplayPaused = false;
            placeMovesCoroutine = StartCoroutine(PlaceMovesWithDelay());
        }
    }

    public void PrintAllChosenMoves()
    {
        ResetGameState();
        StopAllCoroutines();

        currentMoveIndex = 0;
        placeMovesCoroutine = StartCoroutine(PlaceMovesWithDelay());

    }


    private IEnumerator PlaceMovesWithDelay()
    {
        int currentPlayerIndex = 0;

        allChosenMoves.Clear();

        int aiMoveIndex = 0;
        int opponentMoveIndex = 0;

        while (aiMoveIndex < aiMoves.Count || opponentMoveIndex < opponentMoves.Count)
        {
            while (isReplayPaused)
            {
                yield return null;
            }
            if (aiMoveIndex < aiMoves.Count)
            {
                var aiMove = aiMoves[aiMoveIndex];
                yield return new WaitForSeconds(0.2f);

                int x = (int)aiMove.x;
                int y = (int)aiMove.y;

                GridManager.Instance.tiles[x][y].Owner = (int)GameManager.PlayerType.AI;
                Color currentColor = playerColors[currentPlayerIndex];
                Color darkerColor = new Color(
                currentColor.r * 0.75f,  
                currentColor.g * 0.75f,  
                currentColor.b * 0.75f,  
                currentColor.a          
                );
                GridManager.Instance.tiles[x][y].GetComponent<SpriteRenderer>().color = darkerColor;

                allChosenMoves.Add(aiMove);

                currentPlayerIndex = (currentPlayerIndex + 1) % playerColors.Length;

                /*
                GridManager.Instance.tiles[x][y].Owner = (int)GameManager.PlayerType.AI;
                GridManager.Instance.tiles[x][y].GetComponent<SpriteRenderer>().color = playerColors[currentPlayerIndex];

                allChosenMoves.Add(aiMove);

                currentPlayerIndex = (currentPlayerIndex + 1) % playerColors.Length;
                */

                aiMoveIndex++;
                currentMoveIndex++;
            }

            if (opponentMoveIndex < opponentMoves.Count)
            {
                var opponentMove = opponentMoves[opponentMoveIndex];
                yield return new WaitForSeconds(0.2f);

                int x = (int)opponentMove.x;
                int y = (int)opponentMove.y;

                GridManager.Instance.tiles[x][y].Owner = CurrentPlayer;
                Color currentColor = playerColors[currentPlayerIndex];
                Color darkerColor = new Color(
                currentColor.r * 0.75f,  
                currentColor.g * 0.75f,  
                currentColor.b * 0.75f,  
                currentColor.a        
                );
                GridManager.Instance.tiles[x][y].GetComponent<SpriteRenderer>().color = darkerColor;

                allChosenMoves.Add(opponentMove);

                currentPlayerIndex = (currentPlayerIndex + 1) % playerColors.Length;

                /*
                GridManager.Instance.tiles[x][y].Owner = CurrentPlayer;
                GridManager.Instance.tiles[x][y].GetComponent<SpriteRenderer>().color = playerColors[currentPlayerIndex];

                allChosenMoves.Add(opponentMove);

                currentPlayerIndex = (currentPlayerIndex + 1) % playerColors.Length;
                */

                opponentMoveIndex++;
                currentMoveIndex++;
            }
        }

        placeMovesCoroutine = null;
        Tile.SetClickable();
    }

    public List<Vector2> GetAllChosenMoves()
    {
        PrintAllChosenMoves();
        return allChosenMoves;
    }

    public void ResetGameState()
    {
        UpdateGameState(GameState.GenerateGrid);
    }

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
                    if(Player1Type == PlayerType.AI && Player2Type == PlayerType.AI)
                    {
                        Tile.clickable = false;
                    } 
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
