using UnityEngine;

public class ComponentAccess : MonoBehaviour
{
    void Update()
    {
        // Пример 1: Прямой доступ к Rigidbody (кнопка 1)
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("F1");
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(Vector3.up * 300f); // Подкидываем объект вверх
            Debug.Log("Объект подброшен!");
        }

        // Пример 2: Безопасный доступ к Renderer (кнопка 2)
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("F2");
            if (TryGetComponent<Renderer>(out Renderer renderer))
            {
                renderer.material.color = Color.red; // Меняем цвет на красный
                Debug.Log("Цвет изменен на красный!");
            }
        }

        // Пример 3: Прямой доступ к Collider (кнопка 3)
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Debug.Log("F3");
            Collider col = GetComponent<Collider>();
            col.enabled = !col.enabled; // Включаем/выключаем коллизию
            Debug.Log($"Коллизия: {(col.enabled ? "включена" : "выключена")}");
        }
    }
}