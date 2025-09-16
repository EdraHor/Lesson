using UnityEngine;
using UnityEngine.Events;

public class SimpleTrigger : MonoBehaviour
{
    public UnityEvent OnTriggerEnterEvent; // События можно настроить в Inspector

    void OnTriggerEnter(Collider other)
    {
        // Выводим информацию о входе в триггер
        Debug.Log($"Объект вошел в триггер: {other.name}");

        // Вызываем событие (можно настроить в Inspector)
        OnTriggerEnterEvent?.Invoke();
    }
}
