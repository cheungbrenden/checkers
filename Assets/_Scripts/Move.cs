namespace _Scripts
{
    public struct Move
    {
        public Move((int, int) startTilePos, (int,int) endTilePos, (int, int) captureTilePos, bool isCaptureMan, bool isPromotion) 
        {
            StartTilePos = startTilePos;
            EndTilePos = endTilePos; 
            CaptureTilePos = captureTilePos;
            IsCaptureMan = isCaptureMan;
            IsPromotion = isPromotion;
        }
        
        public (int, int) StartTilePos {get; }
        public (int, int) EndTilePos {get; }
        public (int, int) CaptureTilePos {get; }
        public bool IsCaptureMan {get; }
        public bool IsPromotion {get; }
        
    }
}