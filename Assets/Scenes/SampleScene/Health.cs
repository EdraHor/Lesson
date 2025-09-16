using UnityEngine;

// Компонент который хранит здоровье
public class Health : MonoBehaviour
{
    public int CurrentHealth = 100;
    public int MaxHealth = 100;

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        Debug.Log($"Получен урон: {damage}. Здоровье: {CurrentHealth}/{MaxHealth}");

        if (CurrentHealth <= 0)
        {
            Debug.Log("Объект уничтожен!");
            Destroy(gameObject);
        }
    }
}
