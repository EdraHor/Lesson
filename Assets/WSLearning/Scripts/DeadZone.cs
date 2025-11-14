using System;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    public float Damage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerStats>(out var stats))
        {
            stats.HP -= Damage;
        }
        
        other.transform.position = Vector3.zero;
    }
}
