    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Tile : MonoBehaviour
    {
        [SerializeField] private Color _baseColor = Color.white;
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private GameObject _highlight;
        [SerializeField] private int _owner = 0;
        

        public static bool clickable = true;

        public static void SetClickable()
        {
            clickable = false;
        }

        public static void AllowClickable()
        {
            clickable = true;
        }

    public int Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        public static int colorDecidingCounter = 0;

        public void Init()
        {
            _renderer.color = Color.white;
        }

        void OnMouseEnter()
        {
        if (clickable) { 
            _highlight.SetActive(true);
             }
        }

        void OnMouseExit()
        {
            _highlight.SetActive(false);
        }

    private void OnMouseDown()
    {

        int xIndex = int.Parse(gameObject.name.Split('|')[0].Substring(7));
        int yIndex = int.Parse(gameObject.name.Split('|')[1]);

        Debug.Log(GameManager.notHumanTurn);
        if (GameManager.notHumanTurn == false)
        {
            if (clickable)
            {
                if (Owner == 0)
                {
                    Console.WriteLine("Visited positions:");
                    Owner = GameManager.CurrentPlayer;
                    GameManager.Instance.RecordMove(xIndex, yIndex);
                    GameManager.Instance.RecordOpponentMove(xIndex, yIndex);

                    GameManager.Instance.SwitchPlayer();

                    // Update the color of the tile based on the owner
                    _renderer.color = Owner == 1 ? Color.red : Color.blue;
                    if (!GameManager.Instance.BothHuman()) { 
                    GameManager.notHumanTurn = true;
                    }

                }

                // Extract the x and y index from the GameObject's name


                Debug.Log($"Player {Owner} clicked at array position [{xIndex}, {yIndex}]");

                GameUtils gameUtils = new GameUtils();
                (int winner, List<(int, int)> path)= gameUtils.CheckWin(GridManager.Instance.tiles, Owner);

                if (winner != 0)
                {
                    Debug.Log("Shortest path:");
                    foreach ((int x, int y) in path)
                    {
                        GridManager.Instance.tiles[x][y].GetComponent<SpriteRenderer>().color = Color.cyan;
                        Debug.Log($"({x}, {y})");
                    }
                    clickable = false;
                    GameManager.Instance.StopCorutine();
                    PlayerTurnText.win = true;
                    Debug.Log("Player " + winner + " wins!");
                    GameManager.Instance.isGameOver = true;
                    // Hide timer text
                    GameManager.Instance.timerText.gameObject.SetActive(false);
                    //                    GameManager.Instance.fireworks.Play();
                }

                GridManager.Instance.PrintBoardState();
            }
        }
    }
  }
