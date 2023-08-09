using System;
using UnityEngine;

namespace _Scripts
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private Color _mainColor, _offColor, _selectedColor;
        [SerializeField] private SpriteRenderer _renderer;

        // private bool _isOccupied = false;
        private BoardPiece _occupyingBoardPiece;

        private int _xBoardPosition;
        private int _yBoardPosition;

        public int XBoardPosition
        {
            get => _xBoardPosition;
            set => _xBoardPosition = value;
        }

        public int YBoardPosition
        {
            get => _yBoardPosition;
            set => _yBoardPosition = value;
        }


        public BoardPiece OccupyingBoardPiece
        {
            get => _occupyingBoardPiece;
            set => _occupyingBoardPiece = value;
        }

        public void SetColor(bool isOffcolor)
        {
            _renderer.color = isOffcolor ? _offColor : _mainColor;
        }

        public void SelectTile()
        {
            if (_occupyingBoardPiece) _renderer.color = _selectedColor;
        }

        public void DeselectTile()
        {
            _renderer.color = _mainColor;
            // if piece on tile, change tile color
        }

        public void OnMouseDown()
        {
            if (_occupyingBoardPiece != null)
            {
                BoardManager.Instance.SelectPiece(_occupyingBoardPiece);
                return;
            }

            if (BoardManager.Instance.IsMultiCapturePiece)
            {
                BoardManager.Instance.HandleMultipleCaptures(BoardManager.Instance.IsMultiCapturePiece, this);
            }
            else if (BoardManager.Instance.IsPlayerTurnRed)
            {
                BoardManager.Instance.PlayerMove(PlayerColor.RED, this);
            }
            else
            {
                BoardManager.Instance.PlayerMove(PlayerColor.WHITE, this);
            }

            if ((BoardManager.Instance.IsPlayerTurnRed && BoardManager.Instance.RedPlayer == PlayerType.Computer) ||
                (!BoardManager.Instance.IsPlayerTurnRed && BoardManager.Instance.WhitePlayer == PlayerType.Computer))
            {
                BoardManager.Instance.Waiter();
                // Debug.Log("bbbb");
                // BoardManager.Instance.ComputerMove();
            }
        }
    }
}