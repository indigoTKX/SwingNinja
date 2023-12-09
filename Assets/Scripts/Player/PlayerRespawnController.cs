using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class PlayerRespawnController : MonoBehaviour
{
    public event Action OnDie;
    
    [SerializeField] private LayerMask _killLayers;
    [SerializeField] private Vector3 _initialSpawnPoint;
    [SerializeField] private float _respawnTime;
    
    [Space] [SerializeField] private PlayerAnimationController _animationController;


    [Inject] private GameStateManager _gameStateManager;
    private Vector3 _respawnPoint;
    private Rigidbody2D _rigidbody;
    private PlayerOnGroundController _onGroundController;
    private bool _isGrounding;
    private bool _isDead;

    private void Awake()
    {
        _respawnPoint = _initialSpawnPoint;
        _rigidbody = GetComponent<Rigidbody2D>();
        _onGroundController = GetComponent<PlayerOnGroundController>();
        _onGroundController.OnGrounding += OverrideRespawnPoint;
        _animationController.OnGrounded += HandleOnGrounded;
    }

    private void OnDestroy()
    {
        _onGroundController.OnGrounding -= OverrideRespawnPoint;
        _animationController.OnGrounded -= HandleOnGrounded;
    }

    private void OverrideRespawnPoint(Vector3 newRespawnPos)
    {
        _respawnPoint = newRespawnPos;
        _isGrounding = true;
    }

    private void HandleOnGrounded()
    {
        _isGrounding = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (_isGrounding || _isDead) return;
        
        var colliderLayer = col.gameObject.layer;
        if (_killLayers != (_killLayers | (1 << colliderLayer))) return;

        _isDead = true;
        _rigidbody.velocity = Vector2.zero;
        StartCoroutine(Respawn());
        
        _gameStateManager.SetState(GameState.SCREEN_SHOWN);
        OnDie?.Invoke();
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(_respawnTime);
        _isDead = false;
        _gameStateManager.SetState(GameState.ON_GROUND);
        transform.position = _respawnPoint;
    }
}
