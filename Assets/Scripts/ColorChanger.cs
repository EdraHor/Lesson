using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    private Renderer meshRenderer;

    void Start()
    {
        // Получаем компонент Renderer
        meshRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // Меняем цвет по нажатию C
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Создаем случайный цвет
            Color randomColor = new Color(Random.value, Random.value, Random.value, 1f);

            // Применяем цвет к материалу
            meshRenderer.material.color = randomColor;

            Debug.Log($"Цвет изменен на: {randomColor}");
        }
    }
}
