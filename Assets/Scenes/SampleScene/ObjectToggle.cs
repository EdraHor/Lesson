using UnityEngine;

public class ObjectToggle : MonoBehaviour
{
    public GameObject TargetObject; // Объект для включения/выключения

    void Update()
    {
        // Переключаем объект по нажатию T
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (TargetObject != null)
            {
                // Инвертируем активность объекта
                TargetObject.SetActive(!TargetObject.activeSelf);

                // Выводим статус в консоль
                Debug.Log($"Объект {TargetObject.name} теперь: {(TargetObject.activeSelf ? "включен" : "выключен")}");
            }
        }
    }
}
