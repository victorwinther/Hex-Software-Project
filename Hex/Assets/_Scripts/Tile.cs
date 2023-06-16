    using System;
    using System.Collections;
    using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

    public class Tile : MonoBehaviour
    {
        [SerializeField] private Color _baseColor = Color.white;
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private GameObject _highlight;
        [SerializeField] private int _owner = 0;
        private bool isHighlighted;
         private Color originalColor;

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
        // Extract the x and y index from the GameObject's name
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
                    GameManager.Instance.RecordOpponentMove(int.Parse(gameObject.name.Split('|')[0].Substring(7)), int.Parse(gameObject.name.Split('|')[1]));
                    GameManager.Instance.SwitchPlayer();
                    // Update the color of the tile based on the owner
                    _renderer.color = Owner == 1 ? Color.red : Color.blue;
                    if (!GameManager.Instance.BothHuman())
                    {
                        GameManager.notHumanTurn = true;
                    }

                }

            }



                Debug.Log($"Player {Owner} clicked at array position [{xIndex}, {yIndex}]");

                GameUtils gameUtils = new GameUtils();
                (int winner, List<(int, int)> path)= gameUtils.CheckWin(GridManager.Instance.tiles, Owner);

                if (winner != 0)
            {
                    GameManager.Instance.replayButton.SetActive(true);
                    GameManager.Instance.traceButton.SetActive(false);
                    StartCoroutine(WinColors(path,Owner));
                    clickable = false;
                    GameManager.Instance.StopCorutine();
                    PlayerTurnText.win = true;
                    Debug.Log("Player " + winner + " wins!");
                }

                GridManager.Instance.PrintBoardState();
            }
        }
    public static IEnumerator WinColors(List<(int, int)> path, int Owner )
    {
        Color redColor = Color.red;
        redColor.a = 0.75f;
        Color darkRed = new Color(0.5f, 0.0f, 0.0f);

        //Color blueColor = Color.darkBlue;
        Color darkBlue = new Color(0.0f, 0.0f, 0.5f);
        //blueColor.a = 0.75f;

        Debug.Log("Shortest path:");
        foreach ((int x, int y) in path)
        {
            GridManager.Instance.tiles[x][y].GetComponent<SpriteRenderer>().color = Color.white;
            Debug.Log($"({x}, {y})");

        }
        yield return new WaitForSeconds(1.0f);
        foreach ((int x, int y) in path)
        {
            GridManager.Instance.tiles[x][y].GetComponent<SpriteRenderer>().color = Owner == 1 ? darkRed : darkBlue;
            yield return new WaitForSeconds(0.3f);
        }
    }

    }
  
