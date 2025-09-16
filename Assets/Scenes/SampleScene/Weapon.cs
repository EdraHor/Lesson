using UnityEngine;

// Компонент который может атаковать
public class Weapon : MonoBehaviour
{
    public Health TargetHealth; // Перетащите сюда цель в Inspector
    public int Damage = 25;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && TargetHealth != null)
        {
            TargetHealth.TakeDamage(Damage);
            Debug.Log($"Атака! Нанесен урон: {Damage}");
        }
    }
}
