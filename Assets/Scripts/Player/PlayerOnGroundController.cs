using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class PlayerOnGroundController : MonoBehaviour
{
    public event Action OnJumped;
    public event Action<Vector3> OnGrounding;
    
    [SerializeField] private Vector3 _jumpDirection;
    [SerializeField] private float _jumpForce = 10f;

    [Space] [SerializeField] private float _moveAnimationTime = 1f;
    [SerializeField] private PlayerAnimationController _animationController;

    public void LockRotation()
    {
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void UnlockRotation()
    {
        _rigidbody.constraints = RigidbodyConstraints2D.None;
    }
    
    [Inject] private GameStateManager _gameStateManager;
    [Inject] private InputManager _inputManager;
    
    private bool _isOnGround;
    private bool _isGrounding;
    private Rigidbody2D _rigidbody;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody2D>();
        _jumpDirection.Normalize();
        _gameStateManager.OnStateChanged += HandleGameStateChanged;
        _animationController.OnGrounded += HandleGrounded;
    }

    private void OnDestroy()
    {
        _gameStateManager.OnStateChanged -= HandleGameStateChanged;
        _inputManager.OnAnyTap -= Jump;
        _animationController.OnGrounded -= HandleGrounded;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var turnTrigger = col.gameObject.GetComponent<TurnTrigger>();
        if (turnTrigger == null || turnTrigger.IsTriggered()) return;

        turnTrigger.SetTriggered();
        var targetPosition = turnTrigger.GetTargetPosition();
        
        OnGrounding?.Invoke(targetPosition);
        _isGrounding = true;
        _rigidbody.velocity = Vector2.zero;

        _transform.DOMove(targetPosition, _moveAnimationTime);
        
        _gameStateManager.SetState(GameState.ON_GROUND);
    }

    private void HandleGameStateChanged(GameState gameState)
    {
        _isOnGround = gameState == GameState.ON_GROUND;

        if (_isOnGround)
        {
            _inputManager.OnAnyTap += Jump;
            LockRotation();
        }
        else
        {
            _inputManager.OnAnyTap -= Jump;
            UnlockRotation();
        }
    }

    private void Jump()
    {
        if (_isGrounding) return;

        var jumpDir = _transform.TransformVector(_jumpDirection).normalized;
        _rigidbody.AddForce(jumpDir * _jumpForce);
        _gameStateManager.SetState(GameState.IN_AIR);
        OnJumped?.Invoke();
    }

    private void HandleGrounded()
    {
        _isGrounding = false;
        var scale = _transform.localScale;
        scale.x *= -1;
        _transform.localScale = scale;
    }
}
