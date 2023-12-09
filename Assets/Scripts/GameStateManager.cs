using System;
using UnityEngine;
using Zenject;

public class GameStateManager : MonoBehaviour
{
    public event Action<GameState> OnStateChanged;

    // [SerializeField] private string playThemeUid;

    public GameState GetCurrentState()
    {
        return _currentState;
    }


    [Inject] private AudioManager _audioManager;
    
    private GameState _currentState = GameState.ON_GROUND;

    private void Start()
    {
        SetState(GameState.ON_GROUND);
        // _audioManager.PlayAudio(playThemeUid);
    }

    public void SetState(GameState state)
    {
        _currentState = state;
        OnStateChanged?.Invoke(_currentState);
        
        Debug.Log(_currentState.ToString());
    }
}
    
public enum GameState
{
    ON_GROUND,
    IN_AIR,
    SCREEN_SHOWN,
    GAME_OVER,
    LEVEL_WON
}


