using UnityEngine;

public class Collectible : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Проверяем, что это игрок
        if (other.TryGetComponent<PlayerController>(out var player))
        {
            // Уничтожаем объект
            Destroy(gameObject);
        }
    }
}
