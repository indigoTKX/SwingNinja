using UnityEngine;

public class TurnTrigger : MonoBehaviour
{
    [SerializeField] private Transform _targetPosTracker;

    private bool _isTriggered;
    
    public Vector3 GetTargetPosition()
    {
        return _targetPosTracker.position;
    }

    public bool IsTriggered()
    {
        return _isTriggered;
    }
    
    public void SetTriggered()
    {
        _isTriggered = true;
    }
    
}
