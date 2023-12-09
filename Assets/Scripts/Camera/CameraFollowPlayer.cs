using UnityEngine;
using Zenject;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private float _cameraSpeed = 0.5f;
    
    [Inject] private PlayerReference _player;
    [Inject] private PlayerAnimationController _playerAnimationController;
    private Transform _playerTransform;

    private Vector3 _offset;
    private PlayerOnGroundController _onGroundController;
    private bool _isGrounding;

    private void Awake()
    {
        _onGroundController = _player.GetComponent<PlayerOnGroundController>();
        _onGroundController.OnGrounding += MoveToAnotherSection;

        _playerAnimationController.OnGrounded += HandleOnGrounded;
        
        _playerTransform = _player.GetComponent<Transform>();

        _offset = transform.position - _playerTransform.position;
    }

    private void OnDestroy()
    {
        _onGroundController.OnGrounding -= MoveToAnotherSection;
        _playerAnimationController.OnGrounded -= HandleOnGrounded;
    }

    private void Update()
    {
        var currentPosition = transform.position;

        var offset = _playerTransform.TransformVector(_offset);
        offset.z = _offset.z;
        var targetPosition = _playerTransform.position + offset;
        
        if (!_isGrounding)
        {
            targetPosition.y = currentPosition.y;
        }
        
        transform.position = Vector3.MoveTowards(currentPosition, targetPosition, _cameraSpeed);
    }

    private void MoveToAnotherSection(Vector3 targetGroundingPosition)
    {
        _isGrounding = true;
    }
    
    private void HandleOnGrounded()
    {
        _isGrounding = false;
    }
}
