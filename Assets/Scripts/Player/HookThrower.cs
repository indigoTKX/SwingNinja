using System;
using UnityEngine;
using Zenject;

public class HookThrower : MonoBehaviour
{
    public event Action OnThrown;
    public event Action OnAttached;
    public event Action OnDetached;
    
    [SerializeField] private Hook _hookPrefab;

    [Space] [SerializeField] private Vector3 _throwHookDirection;

    public Hook GetCurrentHook()
    {
        return _currentHook;
    }
    
    [Inject] private GameStateManager _gameStateManager;
    [Inject] private InputManager _inputManager;
    
    private DiContainer _diContainer;
    private Transform _transform;

    private bool _isInAir;
    private bool _isAttached;
    private Hook _currentHook;
    
    private void Awake()
    {
        _transform = transform;
        _throwHookDirection.Normalize();
        _gameStateManager.OnStateChanged += HandleGameStateChanged;
        _inputManager.OnAnyTap -= HandleOnTap;
    }

    private void OnDestroy()
    {
        _gameStateManager.OnStateChanged -= HandleGameStateChanged;
    }

    [Inject]
    private void Construct(DiContainer diContainer)
    {
        _diContainer = diContainer;
    }

    private void HandleGameStateChanged(GameState gameState)
    {
        _isInAir = gameState == GameState.IN_AIR;
        
        if (_isInAir)
        {
            _inputManager.OnAnyTap += HandleOnTap;
        }
        else
        {
            _inputManager.OnAnyTap -= HandleOnTap;
            Detach();
        }
    }

    private void HandleOnTap()
    {
        if (!_currentHook)
        {
            ThrowHook();
        }
        else
        {
            Detach();
        }
    }
    
    private void ThrowHook()
    {
        _currentHook = _diContainer.InstantiatePrefabForComponent<Hook>(_hookPrefab, transform.position, Quaternion.identity, _transform);
        var throwDir = _transform.TransformVector(_throwHookDirection);
        _currentHook.Throw(throwDir);
        OnThrown?.Invoke();

        _currentHook.OnAttached += HandleAttach;
    }

    private void HandleAttach()
    {
        _isAttached = true;
        OnAttached?.Invoke();
    }

    private void Detach()
    {
        if (!_currentHook) return;
        
        Destroy(_currentHook.gameObject);
        _isAttached = false;
        OnDetached?.Invoke();
    }
}
