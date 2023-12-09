using UnityEngine;

public class PlayerReference : MonoBehaviour
{
    [SerializeField] private Transform _handTransform;

    public Transform GetHandTransform()
    {
        return _handTransform;
    }
}
