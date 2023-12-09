using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CoinCollector : MonoBehaviour
{
    [SerializeField] private LayerMask _coinLayerMask;

    private void OnTriggerEnter2D(Collider2D col)
    {
        var colliderLayer = col.gameObject.layer;
        if (_coinLayerMask != (_coinLayerMask | (1 << colliderLayer))) return;
        
        Destroy(col.gameObject);
    }
}
