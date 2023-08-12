using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
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

        private List<BoardPiece> _storeRedPieces = new List<BoardPiece>();
        private List<BoardPiece> _storeWhitePieces = new List<BoardPiece>();

        private int _redKingRow = 7;
        private int _whiteKingRow = 0;

        private bool _gameInProgress = false;

        public BoardPiece IsMultiCapturePiece { get; set; } = null;

        public PlayerType RedPlayer => _redPlayer;

        public PlayerType WhitePlayer => _whitePlayer;

        public bool IsPlayerTurnRed { get; set; } = true;

        public Tile[,] Tiles => _tiles;

        public BoardPiece SelectedBoardPiece { get; set; } = null;

        public Tile TileSelected { get; set; } = null;


        // Engine Stuff

        private WaitForSeconds moveDelay = new WaitForSeconds(0.1f);
        private int iterations = 100;

        public int _numRedMen = 0;
        public int _numWhiteMen = 0;
        public int _numRedKings = 0;
        public int _numWhiteKings = 0;

        private const int MenValue = 3;
        private const int KingValue = 5;

        private int _redWins = 0;
        private int _whiteWins = 0;

        //


        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (_redPlayer == PlayerType.Computer && _whitePlayer == PlayerType.Computer)
            {
                StartCoroutine(AIvAI());
            }
        }

        IEnumerator AIvAI()
        {
            for (var i = 0; i < iterations; i++)
            {
                while (_gameInProgress)
                {
                    ComputerMove();
                    yield return moveDelay;
                }
        
                ResetBoard();
                print(i + 1);

                // print($"{_numRedMen} {_numWhiteMen} {_numRedKings} {_numWhiteKings}");
            }
        
            Debug.Log($"{_redWins} {_whiteWins}");
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
            piece.OnTilePosition = (x, y);
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

                        _storeRedPieces.Add(newRedPiece);

                        newRedPiece.OnTilePosition = (x, y);
                        _tiles[x, y].OccupyingBoardPiece = newRedPiece;
                        _numRedMen++;
                        break;
                    case "w":
                        WhitePiece newWhitePiece =
                            Instantiate(_whitePiecePrefab, new Vector2(x, y), Quaternion.identity);
                        _whitePieces.Add(newWhitePiece);
                        _storeWhitePieces.Add(newWhitePiece);
                        newWhitePiece.OnTilePosition = (x, y);
                        _tiles[x, y].OccupyingBoardPiece = newWhitePiece;
                        _numWhiteMen++;
                        break;
                    case "R":
                        RedPiece newRedKing = Instantiate(_redPiecePrefab, new Vector2(x, y), Quaternion.identity);
                        newRedKing.ConvertToKing();
                        _redPieces.Add(newRedKing);
                        _storeRedPieces.Add(newRedKing);
                        newRedKing.OnTilePosition = (x, y);
                        _tiles[x, y].OccupyingBoardPiece = newRedKing;
                        _numRedKings++;
                        break;
                    case "W":
                        WhitePiece newWhiteKing =
                            Instantiate(_whitePiecePrefab, new Vector2(x, y), Quaternion.identity);
                        newWhiteKing.ConvertToKing();
                        _whitePieces.Add(newWhiteKing);
                        _storeWhitePieces.Add(newWhiteKing);
                        newWhiteKing.OnTilePosition = (x, y);
                        _tiles[x, y].OccupyingBoardPiece = newWhiteKing;
                        _numWhiteKings++;
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

        public void ResetBoard(string boardPosition = Standard)
        {
            while (_redPieces.Count != 0)
            {
                RemovePiece(_redPieces[0].OnTilePosition);
            }

            while (_whitePieces.Count != 0)
            {
                RemovePiece(_whitePieces[0].OnTilePosition);
            }

            int x = 0;
            int y = 7;
            boardPosition = String.Join("", boardPosition.Split('\n'));

            int redPieceIndex = 0;
            int whitePieceIndex = 0;

            foreach (char line in boardPosition)
            {
                var symbol = line.ToString();
                switch (symbol)
                {
                    case "r":
                        BoardPiece newRedPiece = _storeRedPieces[redPieceIndex];
                        newRedPiece.gameObject.SetActive(true);
                        _redPieces.Add(newRedPiece);
                        newRedPiece.transform.position = new Vector2(x, y);
                        newRedPiece.OnTilePosition = (x, y);
                        _tiles[x, y].OccupyingBoardPiece = newRedPiece;
                        newRedPiece.ConvertToMan();
                        _numRedMen++;
                        redPieceIndex++;
                        break;
                    case "w":
                        BoardPiece newWhitePiece = _storeWhitePieces[whitePieceIndex];
                        newWhitePiece.gameObject.SetActive(true);
                        _whitePieces.Add(newWhitePiece);
                        newWhitePiece.transform.position = new Vector2(x, y);
                        newWhitePiece.OnTilePosition = (x, y);
                        _tiles[x, y].OccupyingBoardPiece = newWhitePiece;
                        newWhitePiece.ConvertToMan();
                        _numWhiteMen++;
                        whitePieceIndex++;
                        break;
                    // case "R":
                    //     RedPiece newRedKing = Instantiate(_redPiecePrefab, new Vector2(x, y), Quaternion.identity);
                    //     newRedKing.ConvertToKing();
                    //     _redPieces.Add(newRedKing);
                    //     newRedKing.OnTile = _tiles[x, y];
                    //     _tiles[x, y].OccupyingBoardPiece = newRedKing;
                    //     break;
                    // case "W":
                    //     WhitePiece newWhiteKing =
                    //         Instantiate(_whitePiecePrefab, new Vector2(x, y), Quaternion.identity);
                    //     newWhiteKing.ConvertToKing();
                    //     _whitePieces.Add(newWhiteKing);
                    //     newWhiteKing.OnTile = _tiles[x, y];
                    //     _tiles[x, y].OccupyingBoardPiece = newWhiteKing;
                    //     break;
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

        public void RemovePiece((int, int) boardCoordinates, Tile[,] position = null, bool changeOnMainBoard = true)
        {
            var (x, y) = boardCoordinates;

            if (position == null)
            {
                position = _tiles;
            }
            
            var removedPiece = position[x, y].OccupyingBoardPiece;

            if (changeOnMainBoard)
            {
                if (_redPieces.Contains(removedPiece))
                {
                    if (removedPiece.IsKing)
                    {
                        _numRedKings--;
                    }
                    else
                    {
                        _numRedMen--;
                    }

                    _redPieces.Remove(removedPiece);
                }
                else
                {
                    if (removedPiece.IsKing)
                    {
                        _numWhiteKings--;
                    }
                    else
                    {
                        _numWhiteMen--;
                    }

                    _whitePieces.Remove(removedPiece);
                }

            }
            
            
            
            _tiles[x, y].OccupyingBoardPiece.gameObject.SetActive(false);
            _tiles[x, y].OccupyingBoardPiece = null;
        }

        public void RemovePiece(Tile tile)
        {
            BoardPiece removedPiece = tile.OccupyingBoardPiece;
            if (_redPieces.Contains(removedPiece))
            {
                if (removedPiece.IsKing)
                {
                    _numRedKings--;
                }
                else
                {
                    _numRedMen--;
                }

                _redPieces.Remove(removedPiece);
            }
            else
            {
                if (removedPiece.IsKing)
                {
                    _numWhiteKings--;
                }
                else
                {
                    _numWhiteMen--;
                }

                _whitePieces.Remove(removedPiece);
            }

            removedPiece.gameObject.SetActive(false);
            tile.OccupyingBoardPiece = null;
        }

        public void SelectPiece(BoardPiece piece)
        {
            if (piece == null) return;
            if (TileSelected != null) TileSelected.DeselectTile();
            TileSelected = _tiles[piece.OnTilePosition.Item1, piece.OnTilePosition.Item2];
            TileSelected.SelectTile();
            SelectedBoardPiece = piece;
            piece.IsPieceSelected = true;
        }

        // // copy of ListAllValidMoves funciton befoer I fuck shit up

        // public (List<(Tile, Tile, Tile)>, bool) ListAllValidMoves(Tile[,] position, PieceColor pieceColor)
        // {
        //     var captureMoves = new List<(Tile, Tile, Tile)>();
        //     var shiftMoves = new List<(Tile, Tile, Tile)>();
        //
        //     // cannot do this to do function from any position UNLESSSSSSSSS position also includes lists of them pieces
        //     var pieceList = pieceColor == PieceColor.RED ? _redPieces : _whitePieces;
        //
        //     foreach (var piece in pieceList)
        //     {
        //         captureMoves.AddRange(ListCaptureMoves(position, piece));
        //
        //         if (!captureMoves.Any()) // if list is empty
        //         {
        //             shiftMoves.AddRange(ListShiftingMoves(position, piece));
        //         }
        //     }
        //
        //     // Debug.Log("Capture Moves:");
        //     // foreach (var move in captureMoves)
        //     // {
        //     //     Debug.Log(move);
        //     // }
        //     //
        //     // Debug.Log("Shift Moves:");
        //     // foreach (var move in shiftMoves)
        //     // {
        //     //     Debug.Log(move);
        //     // }
        //
        //     if (captureMoves.Any())
        //     {
        //         return (captureMoves, true);
        //     }
        //
        //     return (shiftMoves, false);
        // }

        // TODO: want to list all valid moves from any position

        // (StartTile, EndTile, optional CapturingTile)
        public List<((int, int), (int, int), (int, int))> ListAllValidMoves(PieceColor pieceColor,
            Tile[,] position = null)
        {
            if (position == null)
            {
                position = _tiles;
            }

            var captureMoves = new List<((int, int), (int, int), (int, int))>();
            var shiftMoves = new List<((int, int), (int, int), (int, int))>();

            // cannot do this to do function from any position UNLESSSSSSSSS position also includes lists of them pieces
            for (int i = 0; i < position.GetLength(0); i++)
            {
                for (int j = 0; j < position.GetLength(1); j++)
                {
                    BoardPiece piece = position[i, j].OccupyingBoardPiece;
                    if (piece && (piece.PieceColor == pieceColor))
                    {
                        captureMoves.AddRange(ListCaptureMoves(piece));

                        if (!captureMoves.Any()) // if list is empty
                        {
                            shiftMoves.AddRange(ListShiftingMoves(piece));
                        }
                    }
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
                return captureMoves;
            }

            return shiftMoves;
        }

        private List<((int, int), (int, int), (int, int))> ListShiftingMoves(BoardPiece piece, Tile[,] position = null)
        {
            if (position == null)
            {
                position = _tiles;
            }

            var listShiftingMoves = new List<((int, int), (int, int), (int, int))>();

            var pieceColor = piece.PieceColor;

            var (tileX, tileY) = piece.OnTilePosition;

            if (pieceColor == PieceColor.RED)
            {
                if (tileY != _redKingRow) // check if piece is not on last row
                {
                    if (tileX != 0 &&
                        _tiles[tileX - 1, tileY + 1].OccupyingBoardPiece ==
                        null) // check if piece is not on first column and if so, can move  
                    {
                        listShiftingMoves.Add(((tileX, tileY), (tileX - 1, tileY + 1), (-1, -1)));

                        // listShiftingMoves.Add(new Vector4(tileX, tileY, tileX - 1, t ileY + 1));
                    }

                    if (tileX != 7 && _tiles[tileX + 1, tileY + 1].OccupyingBoardPiece == null)
                    {
                        listShiftingMoves.Add(((tileX, tileY), (tileX + 1, tileY + 1), (-1, -1)));
                    }
                }

                if (piece.IsKing && tileY != _whiteKingRow)
                {
                    if (tileX != 0 && _tiles[tileX - 1, tileY - 1].OccupyingBoardPiece == null)
                    {
                        listShiftingMoves.Add(((tileX, tileY), (tileX - 1, tileY - 1), (-1, -1)));
                    }

                    if (tileX != 7 && _tiles[tileX + 1, tileY - 1].OccupyingBoardPiece == null)
                    {
                        listShiftingMoves.Add(((tileX, tileY), (tileX + 1, tileY - 1), (-1, -1)));
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
                        listShiftingMoves.Add(((tileX, tileY), (tileX - 1, tileY - 1), (-1, -1)));
                        // listShiftingMoves.Add(new Vector4(tileX, tileY, tileX - 1, t ileY + 1));
                    }

                    if (tileX != 7 && _tiles[tileX + 1, tileY - 1].OccupyingBoardPiece == null)
                    {
                        listShiftingMoves.Add(((tileX, tileY), (tileX + 1, tileY - 1), (-1, -1)));
                    }
                }

                if (piece.IsKing && tileY != 7)
                {
                    if (tileX != 0 && _tiles[tileX - 1, tileY + 1].OccupyingBoardPiece == null)
                    {
                        listShiftingMoves.Add(((tileX, tileY), (tileX - 1, tileY + 1), (-1, -1)));
                    }

                    if (tileX != 7 && _tiles[tileX + 1, tileY + 1].OccupyingBoardPiece == null)
                    {
                        listShiftingMoves.Add(((tileX, tileY), (tileX + 1, tileY + 1), (-1, -1)));
                    }
                }
            }

            return listShiftingMoves;
        }

        public List<((int, int), (int, int), (int, int))> ListCaptureMoves(BoardPiece piece, Tile[,] position = null)
        {
            if (position == null)
            {
                position = _tiles;
            }

            var listCapturingMoves = new List<((int, int), (int, int), (int, int))>();

            var pieceColor = piece.PieceColor;
            var (tileX, tileY) = piece.OnTilePosition;

            if (pieceColor == PieceColor.RED)
            {
                if (tileY <= 5) // check if piece is not on last row or second to last row
                {
                    if (tileX >= 2 && _tiles[tileX - 1, tileY + 1].OccupyingBoardPiece != null &&
                        _tiles[tileX - 1, tileY + 1].OccupyingBoardPiece.PieceColor == PieceColor.WHITE &&
                        _tiles[tileX - 2, tileY + 2].OccupyingBoardPiece == null)
                    {
                        listCapturingMoves.Add(
                            ((tileX, tileY), (tileX - 2, tileY + 2), (tileX - 1, tileY + 1)));
                        // listCapturingMoves.Add(new Vector4(tileX, tileY, tileX - 2, tileY + 2));
                    }

                    if (tileX <= 5 && _tiles[tileX + 1, tileY + 1].OccupyingBoardPiece != null &&
                        _tiles[tileX + 1, tileY + 1].OccupyingBoardPiece.PieceColor == PieceColor.WHITE &&
                        _tiles[tileX + 2, tileY + 2].OccupyingBoardPiece == null)
                    {
                        listCapturingMoves.Add(
                            ((tileX, tileY), (tileX + 2, tileY + 2), (tileX + 1, tileY + 1)));
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
                            ((tileX, tileY), (tileX - 2, tileY - 2), (tileX - 1, tileY - 1)));
                        // listCapturingMoves.Add(new Vector4(tileX, tileY, tileX - 2, tileY + 2));
                    }

                    if (tileX <= 5 && _tiles[tileX + 1, tileY - 1].OccupyingBoardPiece != null &&
                        _tiles[tileX + 1, tileY - 1].OccupyingBoardPiece.PieceColor == PieceColor.WHITE &&
                        _tiles[tileX + 2, tileY - 2].OccupyingBoardPiece == null)
                    {
                        listCapturingMoves.Add(
                            ((tileX, tileY), (tileX + 2, tileY - 2), (tileX + 1, tileY - 1)));
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
                            ((tileX, tileY), (tileX - 2, tileY - 2), (tileX - 1, tileY - 1)));
                        // listCapturingMoves.Add(new Vector4(tileX, tileY, tileX - 2, tileY + 2));
                    }

                    if (tileX <= 5 && _tiles[tileX + 1, tileY - 1].OccupyingBoardPiece != null &&
                        _tiles[tileX + 1, tileY - 1].OccupyingBoardPiece.PieceColor == PieceColor.RED &&
                        _tiles[tileX + 2, tileY - 2].OccupyingBoardPiece == null)
                    {
                        listCapturingMoves.Add(
                            ((tileX, tileY), (tileX + 2, tileY - 2), (tileX + 1, tileY - 1)));
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
                            ((tileX, tileY), (tileX - 2, tileY + 2), (tileX - 1, tileY + 1)));
                        // listCapturingMoves.Add(new Vector4(tileX, tileY, tileX - 2, tileY + 2));
                    }

                    if (tileX <= 5 && _tiles[tileX + 1, tileY + 1].OccupyingBoardPiece != null &&
                        _tiles[tileX + 1, tileY + 1].OccupyingBoardPiece.PieceColor == PieceColor.RED &&
                        _tiles[tileX + 2, tileY + 2].OccupyingBoardPiece == null)
                    {
                        listCapturingMoves.Add(
                            ((tileX, tileY), (tileX + 2, tileY + 2), (tileX + 1, tileY + 1)));
                        // listCapturingMoves.Add(new Vector4(tileX, tileY, tileX + 2, tileY + 2));
                    }
                }
            }

            return listCapturingMoves;
        }

        public void PlayerMove(PlayerColor playerColor, (int, int) destinationTilePosition) // add bool for mutlicapture
        {
            print("test");
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

//
            if (SelectedBoardPiece)
            {
                foreach (var move in ListAllValidMoves(pieceColor))
                {
                    if (move.Item1 == SelectedBoardPiece.OnTilePosition && move.Item2 == destinationTilePosition)
                        
                    {
                        MovePieceToTile(SelectedBoardPiece, destinationTilePosition);

                        if (move.Item3 != (-1, -1))
                        {
                            RemovePiece(move.Item3);
                        }

                        if (destinationTilePosition.Item2 == lastRow && !SelectedBoardPiece.IsKing)
                        {
                            SelectedBoardPiece.ConvertToKing();
                            IsPlayerTurnRed = !IsPlayerTurnRed;
                            break;
                        }

                        if (move.Item3 == (-1,-1) || ListCaptureMoves(SelectedBoardPiece).Count == 0)
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

        public void HandleMultipleCaptures(BoardPiece piece, (int, int) destinationTilePosition)
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
                if (capture.Item1 == SelectedBoardPiece.OnTilePosition && capture.Item2 == destinationTilePosition)
                {
                    MovePieceToTile(SelectedBoardPiece, destinationTilePosition);
                    RemovePiece(capture.Item3);


                    if (destinationTilePosition.Item2 == lastRow && !SelectedBoardPiece.IsKing)
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

            _gameInProgress = true;

            if (_redPlayer == PlayerType.Computer && _whitePlayer == PlayerType.Human)
            {
                Waiter();
            }
            
        }
        

        public void Waiter()
        {
            Invoke(nameof(ComputerMove), 0.5f);
        }


        // TODO: FIGURE OUT HOW TO GET COMPUTER TO WORK, ALSO PERHAPS SOME DELAY ON MOVES BC THEY INSTANT
        public void ComputerMove()
        {
            void Move(BoardPiece multiCapturePiece = null)
            {
                while (true)
                {
                    // Debug.Log("computer turn");
                    // print($"{_numRedMen} {_numRedKings} {_numWhiteMen} {_numWhiteKings}");

                    int lastRow;


                    (int, int) startTilePos;
                    (int, int) destinationTilePos;
                    (int, int) captureTilePos;


                    if (IsPlayerTurnRed)
                    {
                        lastRow = 7;
                    }
                    else
                    {
                        lastRow = 0;
                    }


                    if (IsPlayerTurnRed)
                    {
                        (startTilePos, destinationTilePos, captureTilePos) = DetermineRedComputerMove(multiCapturePiece);
                    }
                    else
                    {
                        (startTilePos, destinationTilePos, captureTilePos) = DetermineWhiteComputerMove(multiCapturePiece);
                    }

                    if (!_gameInProgress)
                    {
                        return;
                    }

                    var boardPiece = _tiles[startTilePos.Item1, startTilePos.Item2].OccupyingBoardPiece;

                    // Debug.Log(
                    //     $"{startTile.XBoardPosition} {startTile.YBoardPosition} {endTile.XBoardPosition} {endTile.YBoardPosition}");

                    MovePieceToTile(boardPiece, destinationTilePos);
                    if (captureTilePos != (-1, -1))
                    {
                        RemovePiece(captureTilePos);
                    }

                    if (destinationTilePos.Item2 == lastRow && !boardPiece.IsKing)
                    {
                        boardPiece.ConvertToKing();
                        IsPlayerTurnRed = !IsPlayerTurnRed;
                        IsMultiCapturePiece = null;
                        return;
                    }

                    if (captureTilePos == (-1, -1) || ListCaptureMoves(boardPiece).Count == 0)
                    {
                        IsPlayerTurnRed = !IsPlayerTurnRed;
                        IsMultiCapturePiece = null;
                        return;
                    }
                    else
                    {
                        multiCapturePiece = boardPiece;
                    }
                }
            }

            Move();
        }
        
        public ((int, int), (int, int), (int, int)) DetermineRedComputerMove(BoardPiece multiCapturePiece = null)
        {
            PieceColor pieceColor = PieceColor.RED;
        
            List<((int, int), (int, int), (int, int))> moves;

            if (!multiCapturePiece)
            {
                moves = ListAllValidMoves(pieceColor);
            }
            else
            {
                moves = ListCaptureMoves(multiCapturePiece);
            }
        
            if (moves.Count == 0)
            {
                // Debug.Log("White Won");
                _whiteWins++;
                _gameInProgress = false;
                return ((-1, -1) , (-1, -1), (-1, -1));
            }
        
            if (moves.Count == 1)
            {
                return moves[0];
            }
        
            return RandomMove(moves);
        }
        
        public ((int, int), (int, int), (int, int)) DetermineWhiteComputerMove(BoardPiece multiCapturePiece = null)
        {
            PieceColor pieceColor = PieceColor.WHITE;
        
            List<((int, int), (int, int), (int, int))> moves;

            if (!multiCapturePiece)
            {
                moves = ListAllValidMoves(pieceColor);
            }
            else
            {
                moves = ListCaptureMoves(multiCapturePiece);
            }
        
            if (moves.Count == 0)
            {
                // Debug.Log("Red Won");
                _redWins++;
                _gameInProgress = false;
                return ((-1, -1) , (-1, -1), (-1, -1));
            }
        
            if (moves.Count == 1)
            {
                return moves[0];
            }
        
            return RandomMove(moves);
            //return (Minimax(moves, false), isCapture);
        }
        
        
        // Algorithms 
        private ((int, int), (int, int), (int, int)) FirstMoveInList(List<((int, int), (int, int), (int, int))> moves)
        {
            return moves[0];
        }
        
        private ((int, int), (int, int), (int, int)) RandomMove(List<((int, int), (int, int), (int, int))> moves)
        {
            var random = new Random();
            int randomIndex = random.Next(moves.Count);
            return moves[randomIndex];
        }
        //
        // // do a shitty version first and then we can bitboard/optimaizing stuff
        //
        // // idk how to call positions
        // private (int, int, int, int) CountPieces(Tile[,] position)
        // {
        //     int redManCount = 0, redKingCount = 0, whiteManCount = 0, whiteKingCount = 0;
        //
        //     for (int i = 0; i < position.GetLength(0); i++)
        //     {
        //         for (int j = 0; j < position.GetLength(1); j++)
        //         {
        //             BoardPiece piece = position[i, j].OccupyingBoardPiece;
        //             if (piece)
        //             {
        //                 if (piece.PieceColor == PieceColor.RED)
        //                 {
        //                     if (piece.IsKing)
        //                     {
        //                         redKingCount++;
        //                     }
        //                     else
        //                     {
        //                         redManCount++;
        //                     }
        //                 }
        //
        //                 else
        //                 {
        //                     if (piece.IsKing)
        //                     {
        //                         whiteKingCount++;
        //                     }
        //                     else
        //                     {
        //                         whiteManCount++;
        //                     }
        //                 }
        //             }
        //         }
        //     }
        //
        //     return (redManCount, redKingCount, whiteManCount, whiteKingCount);
        // }
        //
        // private int Evaluation(Tile[,] position, (int, int, int, int) pieceCount)
        // {
        //     return (pieceCount.Item1 - pieceCount.Item3) * MenValue + (pieceCount.Item2 - pieceCount.Item4) * KingValue;
        // }
        //
        // // note: maximizing player is Red
        // private (Tile, Tile, Tile) Minimax(List<(Tile, Tile, Tile)> moves, bool isRed)
        // {
        //     int MinimaxAlgo(Tile[,] position, int depth, bool maximizingPlayer)
        //     {
        //         int redManCount, redKingCount, whiteManCount, whiteKingCount;
        //
        //         (redManCount, redKingCount, whiteManCount, whiteKingCount) = CountPieces(position);
        //
        //         if (depth == 0 || (redManCount + redKingCount == 0) || (whiteManCount + whiteKingCount == 0))
        //             return Evaluation(null, (redManCount, redKingCount, whiteManCount, whiteKingCount));
        //
        //
        //         if (maximizingPlayer)
        //         {
        //             int maxEval = int.MinValue;
        //             var (whiteMoves, isCapture) = ListAllValidMoves(PieceColor.WHITE, position);
        //             foreach (var move in whiteMoves)
        //             {
        //                 var newPosition = ApplyMoveToPosition(move, position, false);
        //                 maxEval = Math.Max(maxEval, MinimaxAlgo(newPosition, depth - 1, false));
        //             }
        //
        //             return maxEval;
        //         }
        //         else
        //         {
        //             int minEval = int.MaxValue;
        //             var (redMoves, isCapture) = ListAllValidMoves(PieceColor.RED, position);
        //             foreach (var move in redMoves)
        //             {
        //                 var newPosition = ApplyMoveToPosition(move, position, false);
        //                 minEval = Math.Min(minEval, MinimaxAlgo(newPosition, depth - 1, true));
        //             }
        //
        //             return minEval;
        //         }
        //     }
        //
        //     (Tile, Tile, Tile) bestMove = (null, null, null);
        //     var score = int.MinValue;
        //     foreach (var move in moves)
        //     {
        //         Tile[,] tilesCopy = _tiles.Clone() as Tile[,];
        //
        //         var newPos = ApplyMoveToPosition(move, tilesCopy, false);
        //         var newPosScore = MinimaxAlgo(newPos, 0, isRed);
        //         if (newPosScore > score)
        //         {
        //             score = newPosScore;
        //             bestMove = move;
        //         }
        //     }
        //
        //     return bestMove;
        // }
        // function minimax(node, depth, maximizingPlayer) is
        //      if depth = 0 or node is a terminal node then
        //          return the heuristic value of node
        //      if maximizingPlayer then
        //          value := −∞
        //          for each child of node do
        //              value := max(value, minimax(child, depth − 1, FALSE))
        //          return value
        //      else (* minimizing player *)
        //          value := +∞
        //          for each child of node do
        //              value := min(value, minimax(child, depth − 1, TRUE))
        //          return value


        // TODO: implement this and then we can do minimax algo
        // private Tile[,] ApplyMoveToPosition(((int, int), (int, int), (int, int)) move, Tile[,] position, bool changeOnMainBoard = true)
        // {
        //     var (startTile, endTile, captureTile) = move;
        //     var piece = startTile.OccupyingBoardPiece;
        //
        //     MovePieceToTile(piece, endTile, position, changeOnMainBoard);
        //     if (captureTile)
        //     {
        //         RemovePiece(captureTile);
        //     }
        //
        //     if (!piece.IsKing && ((piece.PieceColor == PieceColor.RED && endTile.YBoardPosition == _redKingRow) ||
        //                           (piece.PieceColor == PieceColor.WHITE && endTile.YBoardPosition == _whiteKingRow)))
        //     {
        //         piece.ConvertToKing();
        //     }
        //
        //     return position;
        // }

        private void MovePieceToTile(BoardPiece piece, (int, int) destinationTilePosition, Tile[,] position = null, bool changeOnMainBoard = true)
        {
            if (position == null)
            {
                position = _tiles;
            }
            
            if (changeOnMainBoard)
            {
                piece.transform.position = _tiles[destinationTilePosition.Item1, destinationTilePosition.Item2].transform.position;
            }

            
            // (Tile with piece on it).OccupyingBoardPiece = null
            position[piece.OnTilePosition.Item1, piece.OnTilePosition.Item2].OccupyingBoardPiece = null;
            
            // Change piece tilePos to destinationTilePos
            piece.OnTilePosition = (destinationTilePosition.Item1, destinationTilePosition.Item2);
            
            // destinationTile.OccupyingBoardPiece = piece
            position[destinationTilePosition.Item1, destinationTilePosition.Item2].OccupyingBoardPiece = piece;
        }
    }
}

public enum PlayerColor
{
    RED,
    WHITE
}