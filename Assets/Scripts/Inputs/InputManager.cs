using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public event Action OnAnyTap;
    
    private TouchControls _touchControlsMap;
    
    private void Awake()
    {
        _touchControlsMap = new TouchControls();
    }
    
    private void OnEnable()
    {
        _touchControlsMap.Enable();
    }

    private void OnDisable()
    {
        _touchControlsMap.Disable();
    }

    private void Start()
    {
        _touchControlsMap.Gameplay.AnyTap.started += HandleAnyTap;
    }
    
    private void HandleAnyTap(InputAction.CallbackContext inputContext)
    {
        OnAnyTap?.Invoke();
    }
}
