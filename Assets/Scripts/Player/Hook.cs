using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class Hook : MonoBehaviour
{
    public event Action OnAttached;

    [SerializeField] private float _throwSpeed = 2f;
    [SerializeField] private float _maxThrowDistance = 30f;

    [Space] [SerializeField] private HingeJoint2D _hingeJoint;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Transform _rope;
    [SerializeField] private Transform _hookModel;
    
    [Inject] private PlayerReference _player;
    
    private Rigidbody2D _playerRigidBody;
    private Transform _playerHand;

    private Transform _transform;
    private bool _isFlying;
    private Vector3 _targetPosition;

    private void OnDestroy()
    {
        Destroy(_hookModel.gameObject);
    }

    public void Throw(Vector3 direction)
    {
        _targetPosition = _transform.position + direction * _maxThrowDistance;
        _hookModel.right = direction;

        _isFlying = true;
        StartCoroutine(Fly());
    }

    private void Awake()
    {
        _transform = transform;
        _playerRigidBody = _player.GetComponent<Rigidbody2D>();
        _playerHand = _player.GetHandTransform();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        _isFlying = false;

        UpdateRopeLength();
        
        _hingeJoint.connectedBody = _playerRigidBody;
        OnAttached?.Invoke();
    }
    
    private IEnumerator Fly()
    {
        while (_isFlying)
        {
            UpdateRopeLength();
            
            var currentPosition = _rigidbody.position;
            var moveTargetPosition = Vector3.MoveTowards(currentPosition, _targetPosition, _throwSpeed);
            _rigidbody.MovePosition(moveTargetPosition);
            
            yield return null;
        }
    }

    private void UpdateRopeLength()
    {
        var vectorToPlayer = _playerHand.position - _rope.position;
        _rope.right = vectorToPlayer.normalized;
            
        var scale = _rope.localScale;
        scale.x = vectorToPlayer.magnitude;
        _rope.localScale = scale;
    }
}
