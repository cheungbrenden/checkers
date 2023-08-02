using UnityEngine;

namespace _Scripts
{
    public class RedPiece : BoardPiece
    {

        void Awake()
        {
            _pieceColor = PieceColor.RED;
            _renderer.color = _mainColor;
        }

        public override void ConvertToKing()
        {
            _isKing = true;
            _renderer.color = _kingColor;
        }
        
    }
}
