using UnityEngine;

public class ThirdPersonCharController : MonoBehaviour
{
    public float Speed = 5f;
    public float RotationSpeed = 10f;
    
    private Rigidbody rb;
    
    void Start() 
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void Update() 
    {
        // Направление относительно камеры
        Vector3 direction = Camera.main.transform.forward * Input.GetAxis("Vertical") + 
                            Camera.main.transform.right * Input.GetAxis("Horizontal");
        direction.y = 0;
        
        // Движение
        rb.linearVelocity = direction.normalized * Speed + Vector3.up * rb.linearVelocity.y;
        
        // Поворот
        if (direction.magnitude > 0.1f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), RotationSpeed * Time.deltaTime);
    }
}
