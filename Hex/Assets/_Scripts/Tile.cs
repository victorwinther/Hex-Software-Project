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
            _highlight.SetActive(true);
        }

        void OnMouseExit()
        {
            _highlight.SetActive(false);
        }

    private void OnMouseDown()
    {
        Debug.Log(GameManager.notHumanTurn);
        if (GameManager.notHumanTurn == false)
        {
            if (clickable)
            {
                // Extract the x and y index from the GameObject's name
                int xIndex = int.Parse(gameObject.name.Split('|')[0].Substring(7));
                int yIndex = int.Parse(gameObject.name.Split('|')[1]);

                if (Owner == 0)
                {
                    Owner = GameManager.CurrentPlayer;
                    GameManager.Instance.RecordOpponentMove(xIndex, yIndex);
                    GameManager.Instance.SwitchPlayer();

                    // Update the color of the tile based on the owner
                    _renderer.color = Owner == 1 ? Color.red : Color.blue;
                    GameManager.notHumanTurn = true;

                    // Update the corresponding tileOwners element after updating Owner
                    GridManager.Instance.tileOwners[xIndex][yIndex] = Owner;
                }

                Debug.Log($"Player {Owner} clicked at array position [{xIndex}, {yIndex}]");

                GameUtils gameUtils = new GameUtils();
                int winner = gameUtils.CheckWin(GridManager.Instance.tiles, Owner);

                if (winner != 0)
                {
                    clickable = false;
                    GameManager.Instance.StopCorutine();
                    Debug.Log("Player " + winner + " wins!");
                }

                GridManager.Instance.PrintBoardState();
            }
        }
    }

}
