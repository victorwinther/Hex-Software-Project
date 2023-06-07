using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    /*
    private MonteCarloTreeSearch aiMCTS;
    */


    GameUtils gameUtils = new GameUtils();

    public static GameManager Instance;

    public GameState State;

    public enum PlayerType { Human, AI }

    public PlayerType Player1Type = PlayerType.Human;
    public PlayerType Player2Type = PlayerType.AI;

    public static int CurrentPlayer { get; private set; } = 2; // Start with AI

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
        /*
        // Initialize the aiMCTS field
        aiMCTS = new MonteCarloTreeSearch();
        */
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


    /*
    private IEnumerator AIMove()
    {
        yield return new WaitForSeconds(1); // wait for 1 second before AI makes move

        Tile[][] currentGameState = GridManager.Instance.tiles;

        // Find the next move using Monte Carlo Tree Search
        Vector2Int chosenMove = aiMCTS.FindNextMove(currentGameState, CurrentPlayer);

        // Make the chosen move
        GridManager.Instance.tiles[chosenMove.x][chosenMove.y].Owner = CurrentPlayer;

        // Log AI's move
        Debug.Log($"Player {CurrentPlayer} clicked at array position [{chosenMove.x}, {chosenMove.y}]");

        // Update the color of the tile based on the owner
        GridManager.Instance.tiles[chosenMove.x][chosenMove.y].GetComponent<SpriteRenderer>().color = CurrentPlayer == 1 ? Color.blue : Color.red;

        GameUtils gameUtils = new GameUtils();
        int winner = gameUtils.CheckWin(GridManager.Instance.tiles, CurrentPlayer);

        if (winner != 0)
        {
            Debug.Log("Player " + winner + " wins!");
            EndGame();
        }

        SwitchPlayer();
    }
    */

    
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
                    chosenMove = GridManager.Instance.tiles[1][0].Owner == 0 ? new Vector2(1, 0) : new Vector2(2, 0);
                }
                else if (opponentMoves[0].y == 1) // Middle row
                {
                    if (secondMove.y == 0) // If AI's second move was in the top row
                    {
                        chosenMove = GridManager.Instance.tiles[0][2].Owner == 0 ? new Vector2(0, 2) : new Vector2(1, 2);
                    }
                    else if (secondMove.y == 2) // If AI's second move was in the bottom row
                    {
                        chosenMove = GridManager.Instance.tiles[1][0].Owner == 0 ? new Vector2(1, 0) : new Vector2(2, 0);
                    }
                    else
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

        // Make the chosen move

        GridManager.Instance.tiles[(int)chosenMove.x][(int)chosenMove.y].Owner = CurrentPlayer;
        GridManager.Instance.tiles[(int)chosenMove.x][(int)chosenMove.y].GetComponent<SpriteRenderer>().color = CurrentPlayer == 1 ? Color.blue : Color.red;

        // Record AI's move
        aiMoves.Add(chosenMove);

        GameUtils gameUtils = new GameUtils();
        int winner = gameUtils.CheckWin(GridManager.Instance.tiles, CurrentPlayer);

        if (winner != 0)
        {
            Debug.Log("Player " + winner + " wins!");
            EndGame();
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
        float centerX = GridManager.Instance.gridSize / 2;
        float centerY = GridManager.Instance.gridSize / 2;

        // Iterate through all the tiles
        for (int x = 0; x < GridManager.Instance.gridSize; x++)
        {
            for (int y = 0; y < GridManager.Instance.gridSize; y++)
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

                if (tiles[x][y].Owner == 0) // tile is unclaimed
                {
                    availableTiles.Add((tiles[x][y], new Vector2(x, y)));

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
        string winningPlayerName = PlayerPrefs.GetString("Player" + CurrentPlayer + "Name", "Player " + CurrentPlayer);
        PlayerPrefs.SetString("WinningPlayerName", winningPlayerName);
        SceneManager.LoadScene(EndScene);
    }
}














/*
public class MCTSNode
{
    public Tile[][] GameState;
    public List<MCTSNode> Children;
    public MCTSNode Parent;
    public int Wins;
    public int Visits;
    public int PlayerJustMoved;
    public Vector2Int MoveMade; // The move that led to this game state

    public MCTSNode()
    {
        this.Children = new List<MCTSNode>();
    }
}

public class MonteCarloTreeSearch
{
    private int _winScore = 1;
    private int _numberOfSimulations = 100; // Increase or decrease based on your needs and performance
    private System.Random _random = new System.Random();

    public Vector2Int FindNextMove(Tile[][] gameState, int playerJustMoved)
    {
        MCTSNode rootNode = new MCTSNode { GameState = gameState, PlayerJustMoved = playerJustMoved };

        for (int i = 0; i < _numberOfSimulations; ++i)
        {
            MCTSNode promisingNode = SelectPromisingNode(rootNode);
            if (CheckWin(promisingNode.GameState, promisingNode.PlayerJustMoved) == 0)
            {
                // If the game is not over, expand the node
                ExpandNode(promisingNode);
            }

            MCTSNode nodeToExplore = promisingNode;
            if (promisingNode.Children.Count > 0)
            {
                nodeToExplore = promisingNode.Children[_random.Next(promisingNode.Children.Count)];
            }

            int playoutResult = SimulateRandomPlayout(nodeToExplore);
            BackPropagate(nodeToExplore, playoutResult);
        }

        MCTSNode winnerNode = rootNode.Children[0];
        foreach (MCTSNode childNode in rootNode.Children)
        {
            if (childNode.Visits > winnerNode.Visits)
            {
                winnerNode = childNode;
            }
        }

        // Return the move that led to the winning node
        return winnerNode.MoveMade;
    }

    private MCTSNode SelectPromisingNode(MCTSNode rootNode)
    {
        MCTSNode node = rootNode;
        while (node.Children.Count != 0)
        {
            node = FindBestNodeWithUCT(node);
        }
        return node;
    }

    private void ExpandNode(MCTSNode node)
    {
        List<Vector2Int> possibleStates = GetPossibleMoves(node.GameState);
        foreach (Vector2Int move in possibleStates)
        {
            MCTSNode child = new MCTSNode()
            {
                GameState = MakeMove(node.GameState, move, node.PlayerJustMoved % 2 + 1),
                Parent = node,
                PlayerJustMoved = node.PlayerJustMoved % 2 + 1,
                MoveMade = move
            };
            node.Children.Add(child);
        }
    }

    private void BackPropagate(MCTSNode nodeToExplore, int playerNo)
    {
        MCTSNode tempNode = nodeToExplore;
        while (tempNode != null)
        {
            tempNode.Visits++;
            if (tempNode.PlayerJustMoved == playerNo)
            {
                tempNode.Wins += _winScore;
            }
            tempNode = tempNode.Parent;
        }
    }

    private int SimulateRandomPlayout(MCTSNode node)
    {
        MCTSNode tempNode = new MCTSNode { GameState = node.GameState, PlayerJustMoved = node.PlayerJustMoved };
        int boardStatus = CheckWin(tempNode.GameState, tempNode.PlayerJustMoved);

        if (boardStatus == tempNode.PlayerJustMoved)
        {
            tempNode.Parent.Wins += _winScore;
            return boardStatus;
        }

        while (boardStatus == 0)
        {
            List<Vector2Int> availablePositions = GetPossibleMoves(tempNode.GameState);
            tempNode.GameState = MakeMove(tempNode.GameState, availablePositions[_random.Next(availablePositions.Count)], tempNode.PlayerJustMoved % 2 + 1);
            tempNode.PlayerJustMoved = tempNode.PlayerJustMoved % 2 + 1;
            boardStatus = CheckWin(tempNode.GameState, tempNode.PlayerJustMoved);
        }

        return boardStatus;
    }

    private MCTSNode FindBestNodeWithUCT(MCTSNode node)
    {
        int parentVisit = node.Visits;
        return node.Children.OrderByDescending(n => n.Wins / (double)n.Visits + Math.Sqrt(2 * Math.Log(parentVisit) / (double)n.Visits)).First();
    }

    private int CheckWin(Tile[][] gameState, int playerJustMoved)
    {
        GameUtils gameUtils = new GameUtils();
        return gameUtils.CheckWin(gameState, playerJustMoved);
    }

    private List<Vector2Int> GetPossibleMoves(Tile[][] gameState)
    {
        // Implement a function here to get all possible moves for a given game state
        // A possible move would be any unoccupied tile

        // List of all available tiles, prioritized by their proximity to the center
        List<Vector2Int> availableTiles = new List<Vector2Int>();

        // Iterate through all the tiles
        for (int x = 0; x < GridManager.Instance.gridSize; x++)
        {
            for (int y = 0; y < GridManager.Instance.gridSize; y++)
            {
                // If this tile is not owned yet
                if (GridManager.Instance.tiles[x][y].Owner == 0)
                {

                    availableTiles.Add(new Vector2Int(x, y));

                }
            }
        }
        return availableTiles;
    }

    private Tile[][] MakeMove(Tile[][] gameState, Vector2Int move, int player)
    {
        // Create a new gameState to not mutate the original one
        Tile[][] newGameState = new Tile[gameState.Length][];
        for (int i = 0; i < gameState.Length; i++)
        {
            newGameState[i] = new Tile[gameState[i].Length];
            for (int j = 0; j < gameState[i].Length; j++)
            {
                newGameState[i][j] = new Tile();
                newGameState[i][j].Owner = gameState[i][j].Owner;
                // Copy any other properties of the Tile class if necessary
            }
        }

        // Make the move on the new game state
        newGameState[(int)move.x][(int)move.y].Owner = player;
        // newGameState[(int)move.x][(int)move.y].GetComponent<SpriteRenderer>().color = player == 1 ? Color.blue : Color.red;

        return newGameState;
    }


}
*/



public enum GameState
{
    GenerateGrid,
    PlayerTurn,
    EnemyTurn,
    Victory,
    Lose
}
