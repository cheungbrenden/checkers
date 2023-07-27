using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State;

    public static event Action<GameState> OnGameStateChanged; 

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        ChangeState(GameState.GenerateBoard);
    }

    public void ChangeState(GameState newState)
    {
        State = newState;

        switch (State)
        {
            case GameState.GenerateBoard:
                BoardManager.Instance.GenerateBoard();
                break;
            case GameState.GeneratePieces:
                BoardManager.Instance.GenerateBoardPieces();
                break;
            case GameState.SetPieces:
                BoardManager.Instance.SetBoardPieces("test_capture");
                break;
            case GameState.RedTurn:
                break;
            case GameState.WhiteTurn:
                break;
            case GameState.EndGameResult:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(State), newState, null);
        }

        OnGameStateChanged?.Invoke(State);
        
        
    }
    
}

public enum GameState
{
    GenerateBoard,
    GeneratePieces,
    SetPieces,
    RedTurn,
    WhiteTurn,
    EndGameResult
}