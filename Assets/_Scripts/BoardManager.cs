using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _Scripts
{
    public class BoardManager : MonoBehaviour
    {
        public static BoardManager Instance;

        // . = empty space
        // r = red piece     w = white piece
        // R = red king      W = white king

        private const string Standard = ".w.w.w.w\n" +
                                        "w.w.w.w.\n" +
                                        ".w.w.w.w\n" +
                                        "........\n" +
                                        "........\n" +
                                        "r.r.r.r.\n" +
                                        ".r.r.r.r\n" +
                                        "r.r.r.r.\n";

        private const string Custom1 = ".......w\n" +
                                       "........\n" +
                                       "........\n" +
                                       "....w...\n" +
                                       ".....r..\n" +
                                       "..w.....\n" +
                                       ".r......\n" +
                                       "........\n";

        private const string Custom2 = "........\n" +
                                       "r.w.w...\n" +
                                       ".w......\n" +
                                       "........\n" +
                                       "........\n" +
                                       "........\n" +
                                       "........\n" +
                                       "........\n";
        
        private const string Custom3 = "........\n" +
                                       "....R...\n" +
                                       "...w.w..\n" +
                                       "........\n" +
                                       "...w.w..\n" +
                                       "........\n" +
                                       "........\n" +
                                       "........\n";

        public string CustomBoard1 => Custom1;
        public string CustomBoard2 => Custom2;
        public string CustomBoard3 => Custom3;


        [SerializeField] private int _height = 8, _width = 8;

        [SerializeField] private Tile _tilePrefab;
        [SerializeField] private RedPiece _redPiecePrefab;
        [SerializeField] private WhitePiece _whitePiecePrefab;

        [SerializeField] private Camera _camera;

        private Tile[,] _tiles = new Tile [8, 8];
        private List<BoardPiece> _redPieces = new List<BoardPiece>();
        private List<BoardPiece> _whitePieces = new List<BoardPiece>();

        public BoardPiece IsMultiCapturePiece { get; set; } = null;

        public bool IsPlayerTurnRed { get; set; } = true;

        public Tile[,] Tiles => _tiles;

        public BoardPiece SelectedBoardPiece { get; set; } = null;

        public Tile TileSelected { get; set; } = null;

        private void Awake()
        {
            Instance = this;
        }

        public void GenerateBoard()
        {
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    Tile newTile = Instantiate(_tilePrefab, new Vector2(i, j), Quaternion.identity);

                    newTile.name = $"Tile {i} {j}";
                    bool isOffcolor = (i + j) % 2 == 1;
                    newTile.SetColor(isOffcolor);
                    newTile.XBoardPosition = i;
                    newTile.YBoardPosition = j;
                    newTile.gameObject.AddComponent<BoxCollider2D>();

                    _tiles[i, j] = newTile;
                }
            }

            _camera.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);

            // GameManager.Instance.ChangeState(GameState.GeneratePieces);
            GameManager.Instance.ChangeState(GameState.GenerateAndSetPieces);
        }


        public void GenerateAndSetBoardPiece(string boardPosition = Standard)
        {
            int x = 0;
            int y = 7;
            boardPosition = String.Join("", boardPosition.Split('\n'));

            foreach (char line in boardPosition)
            {
                var symbol = line.ToString();
                switch (symbol)
                {
                    case "r":
                        RedPiece newRedPiece = Instantiate(_redPiecePrefab, new Vector2(x, y), Quaternion.identity);
                        _redPieces.Add(newRedPiece);
                        newRedPiece.OnTile = _tiles[x, y];
                        _tiles[x, y].OccupyingBoardPiece = newRedPiece;
                        break;
                    case "w":
                        WhitePiece newWhitePiece = Instantiate(_whitePiecePrefab, new Vector2(x, y), Quaternion.identity);
                        _whitePieces.Add(newWhitePiece);
                        newWhitePiece.OnTile = _tiles[x, y];
                        _tiles[x, y].OccupyingBoardPiece = newWhitePiece;
                        break;
                    case "R":
                        RedPiece newRedKing = Instantiate(_redPiecePrefab, new Vector2(x, y), Quaternion.identity);
                        newRedKing.ConvertToKing();
                        _redPieces.Add(newRedKing);
                        newRedKing.OnTile = _tiles[x, y];
                        _tiles[x, y].OccupyingBoardPiece = newRedKing;
                        break;
                    case "W":
                        WhitePiece newWhiteKing = Instantiate(_whitePiecePrefab, new Vector2(x, y), Quaternion.identity);
                        newWhiteKing.ConvertToKing();
                        _whitePieces.Add(newWhiteKing);
                        newWhiteKing.OnTile = _tiles[x, y];
                        _tiles[x, y].OccupyingBoardPiece = newWhiteKing;
                        break;
                    default:
                        break;

                    // piece.gameObject.SetActive(true);
                    // piece.transform.position = new Vector2(x, y);
                    // piece.OnTile = _tiles[x, y];
                    // _tiles[x, y].OccupyingBoardPiece = piece;
                }

                x++;
                if (x > 7)
                {
                    x = 0;
                    y--;
                }
            }

            GameManager.Instance.ChangeState(GameState.PlayCheckers);
        }

        public void RemovePiece(int x, int y)
        {
            _tiles[x, y].OccupyingBoardPiece.gameObject.SetActive(false);
            _tiles[x, y].OccupyingBoardPiece = null;
        }

        public void RemovePiece(Tile tile)
        {
            BoardPiece removedPiece = tile.OccupyingBoardPiece;
            if (_redPieces.Contains(removedPiece))
            {
                _redPieces.Remove(removedPiece);
            }
            else
            {
                _whitePieces.Remove(removedPiece);
            }

            removedPiece.gameObject.SetActive(false);
            tile.OccupyingBoardPiece = null;
        }

        public void PlacePiece(BoardPiece piece, int x, int y)
        {
            piece.gameObject.SetActive(true);
            piece.transform.position = new Vector2(x, y);
            piece.OnTile = _tiles[x, y];
            _tiles[x, y].OccupyingBoardPiece = piece;
        }

        public void SelectPiece(BoardPiece piece)
        {
            if (piece == null) return;
            if (TileSelected != null) TileSelected.DeselectTile();
            TileSelected = piece.OnTile;
            TileSelected.SelectTile();
            SelectedBoardPiece = piece;
            piece.IsPieceSelected = true;
        }

        // (StartTile, EndTile, optional CapturingTile)
        public (List<(Tile, Tile, Tile)>, bool) ListAllValidMoves(PieceColor pieceColor)
        {
            var captureMoves = new List<(Tile, Tile, Tile)>();
            var shiftMoves = new List<(Tile, Tile, Tile)>();

            List<BoardPiece> pieceList;

            pieceList = pieceColor == PieceColor.RED ? _redPieces : _whitePieces;

            foreach (var piece in pieceList)
            {
                captureMoves.AddRange(ListCaptureMoves(piece));

                if (!captureMoves.Any()) // if list is empty
                {
                    shiftMoves.AddRange(ListShiftingMoves(piece));
                }
            }

            Debug.Log("Capture Moves:");
            foreach (var move in captureMoves)
            {
                Debug.Log(move);
            }

            Debug.Log("Shift Moves:");
            foreach (var move in shiftMoves)
            {
                Debug.Log(move);
            }

            if (captureMoves.Any())
            {
                return (captureMoves, true);
            }

            return (shiftMoves, false);
        }

        public List<(Tile, Tile, Tile)> ListShiftingMoves(BoardPiece piece)
        {
            var listShiftingMoves = new List<(Tile, Tile, Tile)>();

            var pieceColor = piece.PieceColor;
            var currentTile = piece.OnTile;
            var tileX = currentTile.XBoardPosition;
            var tileY = currentTile.YBoardPosition;

            if (pieceColor == PieceColor.RED)
            {
                if (tileY != 7) // check if piece is not on last row
                {
                    if (tileX != 0 &&
                        _tiles[tileX - 1, tileY + 1].OccupyingBoardPiece ==
                        null) // check if piece is not on first column and if so, can move  
                    {
                        listShiftingMoves.Add((currentTile, _tiles[tileX - 1, tileY + 1], null));
                        // listShiftingMoves.Add(new Vector4(tileX, tileY, tileX - 1, t ileY + 1));
                    }

                    if (tileX != 7 && _tiles[tileX + 1, tileY + 1].OccupyingBoardPiece == null)
                    {
                        listShiftingMoves.Add((currentTile, _tiles[tileX + 1, tileY + 1], null));
                    }
                }

                if (piece.IsKing && tileY != 0)
                {
                    if (tileX != 0 && _tiles[tileX - 1, tileY - 1].OccupyingBoardPiece == null)
                    {
                        listShiftingMoves.Add((currentTile, _tiles[tileX - 1, tileY - 1], null));
                    }

                    if (tileX != 7 && _tiles[tileX + 1, tileY - 1].OccupyingBoardPiece == null)
                    {
                        listShiftingMoves.Add((currentTile, _tiles[tileX + 1, tileY - 1], null));
                    }
                }
            }
            else
            {
                if (tileY != 0) // check if piece is not on last row
                {
                    if (tileX != 0 &&
                        _tiles[tileX - 1, tileY - 1].OccupyingBoardPiece ==
                        null) // check if piece is not on first column and if so, can move  
                    {
                        listShiftingMoves.Add((currentTile, _tiles[tileX - 1, tileY - 1], null));
                        // listShiftingMoves.Add(new Vector4(tileX, tileY, tileX - 1, t ileY + 1));
                    }

                    if (tileX != 7 && _tiles[tileX + 1, tileY - 1].OccupyingBoardPiece == null)
                    {
                        listShiftingMoves.Add((currentTile, _tiles[tileX + 1, tileY - 1], null));
                    }
                }

                if (piece.IsKing && tileY != 7)
                {
                    if (tileX != 0 && _tiles[tileX - 1, tileY + 1].OccupyingBoardPiece == null)
                    {
                        listShiftingMoves.Add((currentTile, _tiles[tileX - 1, tileY + 1], null));
                    }

                    if (tileX != 7 && _tiles[tileX + 1, tileY + 1].OccupyingBoardPiece == null)
                    {
                        listShiftingMoves.Add((currentTile, _tiles[tileX + 1, tileY + 1], null));
                    }
                }
            }

            return listShiftingMoves;
        }

        public List<(Tile, Tile, Tile)> ListCaptureMoves(BoardPiece piece)
        {
            var listCapturingMoves = new List<(Tile, Tile, Tile)>();

            var pieceColor = piece.PieceColor;
            var currentTile = piece.OnTile;
            var tileX = currentTile.XBoardPosition;
            var tileY = currentTile.YBoardPosition;

            if (pieceColor == PieceColor.RED)
            {
                if (tileY <= 5) // check if piece is not on last row or second to last row
                {
                    if (tileX >= 2 && _tiles[tileX - 1, tileY + 1].OccupyingBoardPiece != null &&
                        _tiles[tileX - 1, tileY + 1].OccupyingBoardPiece.PieceColor == PieceColor.WHITE &&
                        _tiles[tileX - 2, tileY + 2].OccupyingBoardPiece == null)
                    {
                        listCapturingMoves.Add(
                            (currentTile, _tiles[tileX - 2, tileY + 2], _tiles[tileX - 1, tileY + 1]));
                        // listCapturingMoves.Add(new Vector4(tileX, tileY, tileX - 2, tileY + 2));
                    }

                    if (tileX <= 5 && _tiles[tileX + 1, tileY + 1].OccupyingBoardPiece != null &&
                        _tiles[tileX + 1, tileY + 1].OccupyingBoardPiece.PieceColor == PieceColor.WHITE &&
                        _tiles[tileX + 2, tileY + 2].OccupyingBoardPiece == null)
                    {
                        listCapturingMoves.Add(
                            (currentTile, _tiles[tileX + 2, tileY + 2], _tiles[tileX + 1, tileY + 1]));
                        // listCapturingMoves.Add(new Vector4(tileX, tileY, tileX + 2, tileY + 2));
                    }
                }

                if (piece.IsKing && tileY >= 2)
                {
                    if (tileX >= 2 && _tiles[tileX - 1, tileY - 1].OccupyingBoardPiece != null &&
                        _tiles[tileX - 1, tileY - 1].OccupyingBoardPiece.PieceColor == PieceColor.WHITE &&
                        _tiles[tileX - 2, tileY - 2].OccupyingBoardPiece == null)
                    {
                        listCapturingMoves.Add(
                            (currentTile, _tiles[tileX - 2, tileY - 2], _tiles[tileX - 1, tileY - 1]));
                        // listCapturingMoves.Add(new Vector4(tileX, tileY, tileX - 2, tileY + 2));
                    }

                    if (tileX <= 5 && _tiles[tileX + 1, tileY - 1].OccupyingBoardPiece != null &&
                        _tiles[tileX + 1, tileY - 1].OccupyingBoardPiece.PieceColor == PieceColor.WHITE &&
                        _tiles[tileX + 2, tileY - 2].OccupyingBoardPiece == null)
                    {
                        listCapturingMoves.Add(
                            (currentTile, _tiles[tileX + 2, tileY - 2], _tiles[tileX + 1, tileY - 1]));
                        // listCapturingMoves.Add(new Vector4(tileX, tileY, tileX + 2, tileY + 2));
                    }
                }
            }
            else
            {
                if (tileY >= 2) // check if piece is not on last row or second to last row
                {
                    if (tileX >= 2 && _tiles[tileX - 1, tileY - 1].OccupyingBoardPiece != null &&
                        _tiles[tileX - 1, tileY - 1].OccupyingBoardPiece.PieceColor == PieceColor.RED &&
                        _tiles[tileX - 2, tileY - 2].OccupyingBoardPiece == null)
                    {
                        listCapturingMoves.Add(
                            (currentTile, _tiles[tileX - 2, tileY - 2], _tiles[tileX - 1, tileY - 1]));
                        // listCapturingMoves.Add(new Vector4(tileX, tileY, tileX - 2, tileY + 2));
                    }

                    if (tileX <= 5 && _tiles[tileX + 1, tileY - 1].OccupyingBoardPiece != null &&
                        _tiles[tileX + 1, tileY - 1].OccupyingBoardPiece.PieceColor == PieceColor.RED &&
                        _tiles[tileX + 2, tileY - 2].OccupyingBoardPiece == null)
                    {
                        listCapturingMoves.Add(
                            (currentTile, _tiles[tileX + 2, tileY - 2], _tiles[tileX + 1, tileY - 1]));
                        // listCapturingMoves.Add(new Vector4(tileX, tileY, tileX + 2, tileY + 2));
                    }
                }

                if (piece.IsKing && tileY <= 5)
                {
                    if (tileX >= 2 && _tiles[tileX - 1, tileY + 1].OccupyingBoardPiece != null &&
                        _tiles[tileX - 1, tileY + 1].OccupyingBoardPiece.PieceColor == PieceColor.RED &&
                        _tiles[tileX - 2, tileY + 2].OccupyingBoardPiece == null)
                    {
                        listCapturingMoves.Add(
                            (currentTile, _tiles[tileX - 2, tileY + 2], _tiles[tileX - 1, tileY + 1]));
                        // listCapturingMoves.Add(new Vector4(tileX, tileY, tileX - 2, tileY + 2));
                    }

                    if (tileX <= 5 && _tiles[tileX + 1, tileY + 1].OccupyingBoardPiece != null &&
                        _tiles[tileX + 1, tileY + 1].OccupyingBoardPiece.PieceColor == PieceColor.RED &&
                        _tiles[tileX + 2, tileY + 2].OccupyingBoardPiece == null)
                    {
                        listCapturingMoves.Add(
                            (currentTile, _tiles[tileX + 2, tileY + 2], _tiles[tileX + 1, tileY + 1]));
                        // listCapturingMoves.Add(new Vector4(tileX, tileY, tileX + 2, tileY + 2));
                    }
                }
            }

            return listCapturingMoves;
        }

        public void PlayerMove(PlayerColor playerColor, Tile destinationTile) // add bool for mutlicapture
        {
            PieceColor pieceColor;
            int lastRow;
            bool turnChangeToRed;
            
            if (playerColor == PlayerColor.RED)
            {
                pieceColor = PieceColor.RED;
                lastRow = 7;
                turnChangeToRed = false;
            }
            else
            {
                pieceColor = PieceColor.WHITE;
                lastRow = 0;
                turnChangeToRed = true;
            }
            
            
            if (SelectedBoardPiece)
            {
                foreach (var move in ListAllValidMoves(pieceColor).Item1)
                {
                    if (move.Item1 == SelectedBoardPiece.OnTile && move.Item2 == destinationTile)
                    {
                        SelectedBoardPiece.MovePieceToTile(destinationTile);

                        if (move.Item3)
                        {
                            RemovePiece(move.Item3);
                        }

                        if (destinationTile.YBoardPosition == lastRow && !SelectedBoardPiece.IsKing)
                        {
                            SelectedBoardPiece.ConvertToKing();
                            IsPlayerTurnRed = turnChangeToRed;
                            break;
                        }

                        if (!move.Item3 || ListCaptureMoves(SelectedBoardPiece).Count == 0)
                        {
                            IsPlayerTurnRed = turnChangeToRed;
                            break;
                        }
                        else
                        {
                            IsMultiCapturePiece = SelectedBoardPiece;
                            break;
                        }
                        // if other captures are not possible, end turn, otherwise allow player to keep their turn and only allow captures using that piece
                        
                        


                    }
                }
            }
        }

        public void HandleMultipleCaptures(BoardPiece piece, Tile destinationTile)
        {
            int lastRow;
            bool turnChangeToRed;
            
            if (piece.PieceColor == PieceColor.RED)
            {
                lastRow = 7;
                turnChangeToRed = false;
            }
            else
            {
                lastRow = 0;
                turnChangeToRed = true;
            }
            
            foreach (var capture in ListCaptureMoves(piece))
            {
                if (capture.Item1 == SelectedBoardPiece.OnTile && capture.Item2 == destinationTile)
                {
                    SelectedBoardPiece.MovePieceToTile(destinationTile);
                    RemovePiece(capture.Item3);
                    
                    
                    if (destinationTile.YBoardPosition == lastRow && !SelectedBoardPiece.IsKing)
                    {
                        SelectedBoardPiece.ConvertToKing();
                        IsPlayerTurnRed = turnChangeToRed;
                        break;
                    }

                    if (ListCaptureMoves(piece).Count == 0)
                    {
                        IsPlayerTurnRed = turnChangeToRed;
                        IsMultiCapturePiece = null;
                        break;
                    }
                    
                    else
                    {
                        
                        break;
                    }
                }
            }
            


        }
        
        public void PlayCheckers()
        {
        }
    }
}

public enum PlayerColor
{
    RED,
    WHITE
}