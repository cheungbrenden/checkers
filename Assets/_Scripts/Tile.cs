using System;
using UnityEngine;

namespace _Scripts
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private Color _mainColor, _offColor, _selectedColor;
        [SerializeField] private SpriteRenderer _renderer;

        // private bool _isOccupied = false;

        public BoardPiece BoardPiece { get; set; }

        public void SetColor(bool isOffcolor)
        {
            _renderer.color = isOffcolor ? _offColor : _mainColor;
            
        }

        public void SelectTile()
        {
            if (BoardPiece) _renderer.color = _selectedColor;
        }

        public void DeselectTile()
        {
            _renderer.color = _mainColor;


            // if piece on tile, change tile color
        }
    }
}