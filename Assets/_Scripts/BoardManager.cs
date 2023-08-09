using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

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

        private const string Custom1 = "........\n" +
                                       "w.......\n" +
                                       ".r......\n" +
                                       "........\n" +
                                       "........\n" +
                                       "..r.....\n" +
                                       ".......\n" +
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

        private const string Custom4 = "........\n" +
                                       "....w.w.\n" +
                                       "........\n" +
                                       "..w.....\n" +
                                       ".r......\n" +
                                       "..w.....\n" +
                                       "...r.r..\n" +
                                       "........\n";

        public string StandardBoard => Standard;
        public string CustomBoard1 => Custom1;
        public string CustomBoard2 => Custom2;
        public string CustomBoard3 => Custom3;

        public string CustomBoard4 => Custom4;


        [SerializeField] private int _height = 8, _width = 8;
        private PlayerType _redPlayer, _whitePlayer;

        [SerializeField] private Tile _tilePrefab;
        [SerializeField] private RedPiece _redPiecePrefab;
        [SerializeField] private WhitePiece _whitePiecePrefab;

        [SerializeField] private Camera _camera;

        private Tile[,] _tiles = new Tile [8, 8];
        private List<BoardPiece> _redPieces = new List<BoardPiece>();
        private List<BoardPiece> _whitePieces = new List<BoardPiece>();
        private int _redKingRow = 7;
        private int _whiteKingRow = 0;

        public BoardPiece IsMultiCapturePiece { get; set; } = null;

        public PlayerType RedPlayer => _redPlayer;

        public PlayerType WhitePlayer => _whitePlayer;

        public bool IsPlayerTurnRed { get; set; } = true;

        public Tile[,] Tiles => _tiles;

        public BoardPiece SelectedBoardPiece { get; set; } = null;

        public Tile TileSelected { get; set; } = null;

        
        // Engine Stuff

        private const int menValue = 3;
        private const int kingValue = 5;
        
        //
        
        
        
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

        public void PlacePiece(BoardPiece piece, int x, int y)
        {
            piece.gameObject.SetActive(true);
            piece.transform.position = new Vector2(x, y);
            piece.OnTile = _tiles[x, y];
            _tiles[x, y].OccupyingBoardPiece = piece;
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
                        WhitePiece newWhitePiece =
                            Instantiate(_whitePiecePrefab, new Vector2(x, y), Quaternion.identity);
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
                        WhitePiece newWhiteKing =
                            Instantiate(_whitePiecePrefab, new Vector2(x, y), Quaternion.identity);
                        newWhiteKing.ConvertToKing();
                        _whitePieces.Add(newWhiteKing);
                        newWhiteKing.OnTile = _tiles[x, y];
                        _tiles[x, y].OccupyingBoardPiece = newWhiteKing;
                        break;
                    default:
                        break;
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

            // Debug.Log("Capture Moves:");
            // foreach (var move in captureMoves)
            // {
            //     Debug.Log(move);
            // }
            //
            // Debug.Log("Shift Moves:");
            // foreach (var move in shiftMoves)
            // {
            //     Debug.Log(move);
            // }

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
                if (tileY != _redKingRow) // check if piece is not on last row
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

                if (piece.IsKing && tileY != _whiteKingRow)
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
            
            if (playerColor == PlayerColor.RED)
            {
                pieceColor = PieceColor.RED;
                lastRow = 7;
            }
            else
            {
                pieceColor = PieceColor.WHITE;
                lastRow = 0;
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
                            IsPlayerTurnRed = !IsPlayerTurnRed;
                            // if ((IsPlayerTurnRed && _redPlayer == PlayerType.Computer) ||
                            //     (!IsPlayerTurnRed && _whitePlayer == PlayerType.Computer))
                            // {
                            //     ComputerMove(IsPlayerTurnRed);
                            // }
                            // if (IsPlayerTurnRed == ) if (playerTurn is red and _redPlayer == computer) or (playerTurn is white and _whitePlayer = computer)  
                            break;
                        }

                        if (!move.Item3 || ListCaptureMoves(SelectedBoardPiece).Count == 0)
                        {
                            IsPlayerTurnRed = !IsPlayerTurnRed;
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
                lastRow = _redKingRow;
                turnChangeToRed = false;
            }
            else
            {
                lastRow = _whiteKingRow;
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
                        IsMultiCapturePiece = null;
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

        public void PlayCheckers(PlayerType redPlayer = PlayerType.Human, PlayerType whitePlayer = PlayerType.Human)
        {
            _redPlayer = redPlayer;
            _whitePlayer = whitePlayer;

            // if (_redPlayer == PlayerType.Computer && _whitePlayer == PlayerType.Computer)
            // {
            //     for (var i = 0; i < 100; i++)
            //     {
            //         Waiter();
            //     }
            // }
            
            if (_redPlayer == PlayerType.Computer)
            {
                Waiter();
            }

            // if (_redPlayer == PlayerType.Computer && _whitePlayer == PlayerType.Computer)
            // {
            //     for (var i = 0; i < 2000; i++)
            //     {
            //         Waiter();
            //     }
            //     
            // }
        }


        // public void Waiter2()
        // {
        //     Invoke(nameof(Empty), 5f);
        // }
        //
        // public void Empty()
        // {
        // }

        public void Waiter()
        {
            Invoke(nameof(ComputerMove), 0.5f);
        }


        // TODO: FIGURE OUT HOW TO GET COMPUTER TO WORK, ALSO PERHAPS SOME DELAY ON MOVES BC THEY INSTANT
        public void ComputerMove()
        {
            void asdf(BoardPiece multiCapturePiece = null)
            {
                Debug.Log("computer turn");


                int lastRow;

                if (IsPlayerTurnRed)
                {
                    lastRow = 7;
                }
                else
                {
                    lastRow = 0;
                }

                var ((startTile, endTile, pieceCapturedTile), isCapture) =
                    DetermineComputerMove(IsPlayerTurnRed, multiCapturePiece);
                var boardPiece = startTile.OccupyingBoardPiece;

                Debug.Log(
                    $"{startTile.XBoardPosition} {startTile.YBoardPosition} {endTile.XBoardPosition} {endTile.YBoardPosition}");

                boardPiece.MovePieceToTile(endTile);
                if (isCapture)
                {
                    RemovePiece(pieceCapturedTile);
                }

                if (endTile.YBoardPosition == lastRow && !boardPiece.IsKing)
                {
                    boardPiece.ConvertToKing();
                    IsPlayerTurnRed = !IsPlayerTurnRed;
                    IsMultiCapturePiece = null;
                    return;
                }

                if (!pieceCapturedTile || ListCaptureMoves(boardPiece).Count == 0)
                {
                    IsPlayerTurnRed = !IsPlayerTurnRed;
                    IsMultiCapturePiece = null;
                    return;
                }
                else
                {
                    asdf(boardPiece);

                    return;
                }
            }

            asdf();
        }

        public ((Tile, Tile, Tile), bool) DetermineComputerMove(bool isRedPlayerTurn,
            BoardPiece multiCapturePiece = null) // return a move
        {
            PieceColor pieceColor = isRedPlayerTurn ? PieceColor.RED : PieceColor.WHITE;


            List<(Tile, Tile, Tile)> moves;
            bool isCapture;

            if (!multiCapturePiece)
            {
                (moves, isCapture) = ListAllValidMoves(pieceColor);
            }
            else
            {
                (moves, isCapture) = (ListCaptureMoves(multiCapturePiece), true);
            }


            if (moves.Count == 1)
            {
                return (moves[0], isCapture);
            }

            var random = new Random();
            int randomIndex = random.Next(moves.Count);
            Debug.Log(moves[randomIndex]);
            return (moves[randomIndex], isCapture);
        }
    }
}

public enum PlayerColor
{
    RED,
    WHITE
}


//     
// if (destinationTile.YBoardPosition == lastRow && !SelectedBoardPiece.IsKing)
// {
//     SelectedBoardPiece.ConvertToKing();
//     IsPlayerTurnRed = turnChangeToRed;

//     // if (IsPlayerTurnRed == ) if (playerTurn is red and _redPlayer == computer) or (playerTurn is white and _whitePlayer = computer)  
//     break;
// }
//
// if (!move.Item3 || ListCaptureMoves(SelectedBoardPiece).Count == 0)
// {
//     IsPlayerTurnRed = turnChangeToRed;
//     break;
// }
// else
// {
//     IsMultiCapturePiece = SelectedBoardPiece;
//     break;
// }


// PieceColor pieceColor;
// int lastRow;
// bool turnChangeToRed;
//
// if (playerColor == PlayerColor.RED)
// {
//     pieceColor = PieceColor.RED;
//     lastRow = 7;
//     turnChangeToRed = false;
// }
// else
// {
//     pieceColor = PieceColor.WHITE;
//     lastRow = 0;
//     turnChangeToRed = true;
// }
//
//
// if (SelectedBoardPiece)
// {
//     foreach (var move in ListAllValidMoves(pieceColor).Item1)
//     {
//         if (move.Item1 == SelectedBoardPiece.OnTile && move.Item2 == destinationTile)
//         {
//             SelectedBoardPiece.MovePieceToTile(destinationTile);
//
//             if (move.Item3)
//             {
//                 RemovePiece(move.Item3);
//             }
//
//             if (destinationTile.YBoardPosition == lastRow && !SelectedBoardPiece.IsKing)
//             {
//                 SelectedBoardPiece.ConvertToKing();
//                 IsPlayerTurnRed = turnChangeToRed;
//                 if ((IsPlayerTurnRed && _redPlayer == PlayerType.Computer) ||
//                     (!IsPlayerTurnRed && _whitePlayer == PlayerType.Computer))
//                 {
//                     ComputerMove(IsPlayerTurnRed);
//                 }
//                 // if (IsPlayerTurnRed == ) if (playerTurn is red and _redPlayer == computer) or (playerTurn is white and _whitePlayer = computer)  
//                 break;
//             }
//
//             if (!move.Item3 || ListCaptureMoves(SelectedBoardPiece).Count == 0)
//             {
//                 IsPlayerTurnRed = turnChangeToRed;
//                 break;
//             }
//             else
//             {
//                 IsMultiCapturePiece = SelectedBoardPiece;
//                 break;
//             }
//             // if other captures are not possible, end turn, otherwise allow player to keep their turn and only allow captures using that piece
//             
//             
//
//
//         }
//     }
// }