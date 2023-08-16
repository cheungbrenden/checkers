using System;
using UnityEngine;

namespace _Scripts
{
    public class BoardPiece : MonoBehaviour
    {
        protected (int, int) _onTilePosition;
        protected bool _isKing = false;

        
        // [SerializeField] private Color _mainColor, _offColor, _selectedColor;
        [SerializeField] protected SpriteRenderer _renderer;

        [SerializeField] protected Color _mainColor, _kingColor;
        protected PieceColor _pieceColor;


        public (int, int) OnTilePosition
        {
            get => _onTilePosition;
            set => _onTilePosition = value;
        }

        public bool IsKing
        {
            get => _isKing;
            set => _isKing = value;
        }

        public PieceColor PieceColor
        {
            get => _pieceColor;
            set => _pieceColor = value;
        }

        public bool IsPieceSelected { get; set; } = false;

        private void OnMouseDown()
        {
            BoardManager.Instance.SelectPiece(this);
        }

        public virtual void ConvertToKing() {}
        
        public virtual void ConvertToMan() {}
    }
}

public enum PieceColor
{
    RED,
    WHITE
}