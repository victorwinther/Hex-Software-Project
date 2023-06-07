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
        
            if (Owner == 0)
            {
                Owner = GameManager.CurrentPlayer;
                GameManager.Instance.RecordOpponentMove(int.Parse(gameObject.name.Split('|')[0].Substring(7)), int.Parse(gameObject.name.Split('|')[1]));
                GameManager.Instance.SwitchPlayer();

                // Update the color of the tile based on the owner
                _renderer.color = Owner == 1 ? Color.blue : Color.red;

            
            }
   

            // Extract the x and y index from the GameObject's name
            int xIndex = int.Parse(gameObject.name.Split('|')[0].Substring(7));
            int yIndex = int.Parse(gameObject.name.Split('|')[1]);

            Debug.Log($"Player {Owner} clicked at array position [{xIndex}, {yIndex}]");

            GameUtils gameUtils = new GameUtils();
            int winner = gameUtils.CheckWin(GridManager.Instance.tiles, Owner);

            if (winner != 0)
            {
                Debug.Log("Player " + winner + " wins!");
                GameManager.Instance.EndGame();
            }
      
            GridManager.Instance.PrintBoardState();
        }
    }
