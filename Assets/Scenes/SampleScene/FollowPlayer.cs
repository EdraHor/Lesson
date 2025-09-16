using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform Target; // Цель для следования
    public float FollowSpeed = 5f; // Скорость следования
    public Vector3 Offset = new Vector3(0, 5, -10); // Смещение от цели

    void Update()
    {
        if (Target != null)
        {
            // Вычисляем целевую позицию
            Vector3 targetPosition = Target.position + Offset;

            // Плавно перемещаемся к цели
            transform.position = Vector3.Lerp(transform.position, targetPosition, FollowSpeed * Time.deltaTime);

            // Поворачиваемся к цели
            transform.LookAt(Target);
        }
    }
}
