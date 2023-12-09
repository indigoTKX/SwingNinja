using System;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

public class PlayerAnimationController : MonoBehaviour
{
    public event Action OnGrounded;
    
    [SerializeField] private float _middleRange = 0.5f;
    [SerializeField] private float _restoreRotationAnimationTime = 1f;

    [Space] [SerializeField] private HookThrower _hookThrower;
    [SerializeField] private PlayerOnGroundController playerOnGroundController;
    [SerializeField] private PlayerRespawnController _playerRespawnController;
    [SerializeField] private Transform _playerTransform;

    //called from animation
    public void FireOnGrounded()
    {
        OnGrounded?.Invoke();
    }
    
    private const string SWING_LEFT_ANIMATION_UID = "SwingLeft";
    private const string SWING_MIDDLE_ANIMATION_UID = "SwingMiddle";
    private const string SWING_RIGHT_ANIMATION_UID = "SwingRight";
    private const string AIR_JUMP_ANIMATION_UID = "AirJump";
    private const string JUMP_ANIMATION_UID = "Jump";
    private const string GROUNDING_ANIMATION_UID = "Grounding";
    private const string DIE_ANIMATION_UID = "Die";
    private const string IDLE_ANIMATION_UID = "Idle";

    [Inject] private GameStateManager _gameStateManager;
    
    private Animator _animator;
    private bool _isSwinging = true;
    private Tweener _rotateTweener;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        
        _hookThrower.OnThrown += HandleOnThrown;
        _hookThrower.OnAttached += HandleOnAttached;
        _hookThrower.OnDetached += PlayDetachedAnimation;

        playerOnGroundController.OnJumped += PlayJumpAnimation;
        playerOnGroundController.OnGrounding += PlayGroundingAnimation;

        _playerRespawnController.OnDie += PlayDieAnimation;

        _gameStateManager.OnStateChanged += HandeGameStateChanged;
    }

    private void OnDestroy()
    {
        _hookThrower.OnThrown -= HandleOnThrown;
        _hookThrower.OnAttached -= HandleOnAttached;
        _hookThrower.OnDetached -= PlayDetachedAnimation;
        
        playerOnGroundController.OnJumped -= PlayJumpAnimation;
        playerOnGroundController.OnGrounding -= PlayGroundingAnimation;
        
        _playerRespawnController.OnDie -= PlayDieAnimation;
        
        _gameStateManager.OnStateChanged -= HandeGameStateChanged;
    }

    private void Update()
    {
        if (!_isSwinging) return;
        
        var hook = _hookThrower.GetCurrentHook();
        if (!hook) return;

        var swingPosition = _playerTransform.position.x - hook.transform.position.x;
        
        var lookDir = _playerTransform.InverseTransformVector(_playerTransform.right);
        swingPosition *= lookDir.x > 0 ? 1 : -1;
        
        if (swingPosition <= -_middleRange)
        {
            _animator.SetTrigger(SWING_LEFT_ANIMATION_UID);
        }
        else if (swingPosition >= _middleRange)
        {
            _animator.SetTrigger(SWING_RIGHT_ANIMATION_UID);
        }
        else
        {
            _animator.SetTrigger(SWING_MIDDLE_ANIMATION_UID);
        }
    }

    private void HandleOnThrown()
    {
        _rotateTweener.Complete();
    }
    
    private void HandleOnAttached()
    {
        _isSwinging = true;
        playerOnGroundController.UnlockRotation();
    }

    private void PlayDetachedAnimation()
    {
        _isSwinging = false;
        _animator.ResetTrigger(SWING_LEFT_ANIMATION_UID);
        _animator.ResetTrigger(SWING_MIDDLE_ANIMATION_UID);
        _animator.ResetTrigger(SWING_RIGHT_ANIMATION_UID);
        _animator.SetTrigger(AIR_JUMP_ANIMATION_UID);
        _rotateTweener = _playerTransform.DORotate(Vector3.zero, _restoreRotationAnimationTime);
        playerOnGroundController.LockRotation();
    }

    private void PlayJumpAnimation()
    {
        _animator.SetTrigger(JUMP_ANIMATION_UID);
    }

    private void PlayGroundingAnimation(Vector3 tgtPos)
    {
        _animator.SetTrigger(GROUNDING_ANIMATION_UID);
    }
    
    
    private void PlayDieAnimation()
    {
        _animator.SetTrigger(DIE_ANIMATION_UID);
    }

    private void HandeGameStateChanged(GameState state)
    {
        if (state == GameState.ON_GROUND)
        {
            _animator.SetTrigger(IDLE_ANIMATION_UID);
        }
        else
        {
            _animator.ResetTrigger(IDLE_ANIMATION_UID);
        }
    }
}
