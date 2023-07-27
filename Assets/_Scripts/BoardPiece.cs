using System;
using UnityEngine;

namespace _Scripts
{
    public class BoardPiece : MonoBehaviour
    {
        // private Vector2 _boardPosition;
        protected Tile _onTile;
        protected bool _isKing = false;
        private bool isPieceSelected = false;
        
        public Tile OnTile
        {
            get => _onTile;
            set => _onTile = value;
        }
        
        //
        // public Vector2 BoardPosition
        // {
        //     get => _boardPosition;
        //     set => _boardPosition = value;
        // }
        //
        private void OnMouseDown()
        {
            BoardManager.Instance.SelectPiece(this);
            isPieceSelected = true;
        }
        
        // public void MovePieceToTile(Tile destinationTile)
        // {
        //     if (!_isKing)
        //     {
        //         _onTile.BoardPiece.transform.position = destinationTile.transform.position;
        //         _onTile.BoardPiece = null;
        //         _onTile = destinationTile;
        //         _onTile.BoardPiece = this;
        //     }
        // }
    }
    
    
}
