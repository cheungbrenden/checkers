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
            BoardManager.Instance._numRedKings++;
            BoardManager.Instance._numRedMen--;
        }

        public override void ConvertToMan()
        {
            _isKing = false;
            _renderer.color = _mainColor;
        }
        
    }
}
