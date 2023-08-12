using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts
{
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
                case GameState.GenerateAndSetPieces:
                    BoardManager.Instance.GenerateAndSetBoardPiece(BoardManager.Instance.StandardBoard);
                    break;
                case GameState.PlayCheckers:
                    BoardManager.Instance.PlayCheckers(PlayerType.Computer, PlayerType.Computer);
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
        GenerateAndSetPieces,
        PlayCheckers,
        EndGameResult
    }
    
    public enum PlayerType
    {
        Human,
        Computer
    }
}