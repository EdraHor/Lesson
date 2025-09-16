using UnityEngine;

public class InspectorCustomization : MonoBehaviour
{
    [Header("Настройки движения")] // - Так выглядит атрибут
    public float MoveSpeed = 5f;
    public float JumpForce = 10f;

    [Space(10)] // Добавляет пустое место

    [Header("Настройки здоровья")]
    [Range(0, 100)] // Слайдер от 0 до 100
    public int Health = 100;

    [Tooltip("Эта переменная управляет скоростью атаки")]
    public float AttackSpeed = 1f;
}
