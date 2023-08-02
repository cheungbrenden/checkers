namespace _Scripts
{
    public class WhitePiece : BoardPiece
    {
        void Awake()
        {
            _pieceColor = PieceColor.WHITE;
            _renderer.color = _mainColor;
        }
        
        public override void ConvertToKing()
        {
            _isKing = true;
            _renderer.color = _kingColor;
        }
    }
}
