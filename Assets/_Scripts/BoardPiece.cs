using System;
using UnityEngine;

namespace _Scripts
{
    public class BoardPiece : MonoBehaviour
    {
        protected Tile _onTile;
        protected bool _isKing = false;

        
        // [SerializeField] private Color _mainColor, _offColor, _selectedColor;
        [SerializeField] protected SpriteRenderer _renderer;

        [SerializeField] protected Color _mainColor, _kingColor;
        protected PieceColor _pieceColor;

        private bool _isPieceSelected = false;


        public Tile OnTile
        {
            get => _onTile;
            set => _onTile = value;
        }

        public bool IsKing
        {
            get => _isKing;
            set => _isKing = value;
        }

        public PieceColor PieceColor
        {
            get => _pieceColor;
        }

        public bool IsPieceSelected
        {
            get => _isPieceSelected;
            set => _isPieceSelected = value;
        }
        
        private void OnMouseDown()
        {
            BoardManager.Instance.SelectPiece(this);
        }

        
        public void MovePieceToTile(Tile destinationTile)
        {
            _onTile.OccupyingBoardPiece.transform.position = destinationTile.transform.position;
            _onTile.OccupyingBoardPiece = null;
            _onTile = destinationTile;
            _onTile.OccupyingBoardPiece = this;
        }

        public virtual void ConvertToKing() {}
    }
}

public enum PieceColor
{
    RED,
    WHITE
}