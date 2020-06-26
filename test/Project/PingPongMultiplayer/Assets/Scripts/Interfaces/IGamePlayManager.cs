using System;

namespace Interfaces
{
    public interface IGamePlayManager
    {
        event EventHandler Destroyed;
        
        bool IsWaitingState { get; }
        bool IsStartingState { get; }
        bool IsPlayingState { get; }
        
        void StartGame();
        void StopGame();
    }
}