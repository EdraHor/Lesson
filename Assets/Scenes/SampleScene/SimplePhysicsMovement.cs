using UnityEngine;

public class SimplePhysicsMovement : MonoBehaviour
{
    public float MoveForce = 10f; // Сила движения
    public float JumpForce = 5f; // Сила прыжка

    private Rigidbody rb;

    void Start()
    {
        // Получаем компонент Rigidbody
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Получаем ввод от игрока
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Создаем вектор движения и применяем силу
        Vector3 movement = new Vector3(horizontal, 0, vertical) * MoveForce;
        rb.AddForce(movement);

        // Прыжок по пробелу
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }
    }
}