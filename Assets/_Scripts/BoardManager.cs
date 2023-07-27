using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts
{
    public class BoardManager : MonoBehaviour
    {
        public static BoardManager Instance;
        
        [SerializeField] private int _height = 8, _width = 8;
        [SerializeField] private int _numRedPieces = 12, _numWhitePieces = 12;

        [SerializeField] private Tile _tilePrefab;
        [SerializeField] private RedPiece _redPiecePrefab;
        [SerializeField] private WhitePiece _whitePiecePrefab;

        [SerializeField] private Camera _camera;

        private Tile[,] _tiles = new Tile [8, 8];
        private List<RedPiece> _redPieces = new List<RedPiece>();
        private List<WhitePiece> _whitePieces = new List<WhitePiece>();

        private BoardPiece _selectedBoardPiece = null;
        
        public Tile[,] Tiles => _tiles;

        public Tile TileSelected { get; set; } = null;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            
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

                    _tiles[i, j] = newTile;
                }
            }

            _camera.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);
            
            GameManager.Instance.ChangeState(GameState.GeneratePieces);
        }

        public void GenerateBoardPieces()
        {
            for (int i = 0; i < _numRedPieces; i++)
            {
                RedPiece newRedPiece = Instantiate(_redPiecePrefab, new Vector2(0, i), Quaternion.identity);
                WhitePiece newWhitePiece = Instantiate(_whitePiecePrefab, new Vector2(2, i), Quaternion.identity);
                
                    
                _redPieces.Add(newRedPiece);
                _whitePieces.Add(newWhitePiece);
                
                newRedPiece.gameObject.SetActive(false);
                newWhitePiece.gameObject.SetActive(false);

            }
            
            GameManager.Instance.ChangeState(GameState.SetPieces);
        }

        public void RemovePiece(int x, int y)
        {
            _tiles[x, y].BoardPiece.gameObject.SetActive(false);
            _tiles[x, y].BoardPiece = null;
        }
        
        public void PlacePiece(BoardPiece piece, int x, int y)
        {
            piece.gameObject.SetActive(true);
            piece.transform.position = new Vector2(x, y);
            piece.OnTile = _tiles[x, y];
            _tiles[x, y].BoardPiece = piece;
        }

        public void SetBoardPieces(string startingPos = "standard")
        {
            if (startingPos == "standard")
            {
                int x_counter = 0;
                int y_counter = 0;
                int row_switch_counter = 0;


                foreach (var redPiece in _redPieces)
                {

                    PlacePiece(redPiece, x_counter, y_counter);

                    x_counter += 2;
                    if (x_counter >= 8)
                    {
                        y_counter++;
                        row_switch_counter++;
                        x_counter = row_switch_counter % 2;
                    }
                }
            
            
                x_counter = 7;
                y_counter = 7;
                row_switch_counter = 0;
                foreach (var whitePiece in _whitePieces)
                {
            
                    PlacePiece(whitePiece, x_counter, y_counter);
            
                    x_counter -= 2;
                    if (x_counter < 0)
                    {
                        y_counter--;
                        row_switch_counter++;
                        x_counter = 7 - row_switch_counter % 2;
                    }
                }
            }
            else if (startingPos == "test_capture")
            {
                PlacePiece(_redPieces[0], 1, 1);
                PlacePiece(_redPieces[1], 2, 2);
                // possible moves are (1,1) - (0,2), (2,2) - (1,3), (2,2) - (3,3)

            }
            else
            {
                // set a different position
            }
            
            
            
            GameManager.Instance.ChangeState(GameState.RedTurn);
        }
        
        public void SelectPiece(BoardPiece piece)
        {
            if (piece == null) return;
            if (TileSelected != null) TileSelected.DeselectTile();
            TileSelected = piece.OnTile;
            TileSelected.SelectTile();
            _selectedBoardPiece = piece;
        }

        public void ListAllValidMoves(PlayerColor playerColor)
        {
            if (playerColor == PlayerColor.RED)
            {
                
                bool mustCapture = false;
                
                foreach (var piece in _redPieces)
                {
                    /*
                     *  check if piece has a capture, if so, don't wanna check for regular sliding moves
                     * 
                     */
                    
                }
                
            }
            else
            {
                
            }
        }

        public List<Vector2> ListShiftingMoves(BoardPiece piece)
        {
            return new List<Vector2>();
        }
        
        public List<Vector2> ListCaptureMoves(BoardPiece piece)
        {
            return new List<Vector2>();
        }

    }
}

public enum PlayerColor
{
    RED,
    WHITE
}