using UnityEngine;
using static System.Threading.Tasks.Task;

public class DynamicComponents : MonoBehaviour
{
    async void Start()
    {
        Debug.Log("=== Добавление компонентов ===");
        await Delay(2000);
        Debug.Log("=== Добавление компонентов ===");

        // Добавляем Rigidbody
        Rigidbody newRb = gameObject.AddComponent<Rigidbody>();
        newRb.mass = 2f;
        Debug.Log("Rigidbody добавлен");

        await Delay(1500);
        
        // Добавляем BoxCollider
        BoxCollider newCollider = gameObject.AddComponent<BoxCollider>();
        newCollider.size = new Vector3(1, 1, 1);
        Debug.Log("BoxCollider добавлен");
        
        await Delay(1500);
        
        // Добавляем Rigidbody
        Destroy(newRb);
        Debug.Log("Rigidbody удален");
    }
}
