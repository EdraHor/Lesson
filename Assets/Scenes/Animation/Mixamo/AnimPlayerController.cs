using UnityEngine;

public class AnimPlayerController : MonoBehaviour
{
    public float Speed = 5f;
    public float JumpForce = 300f;
    public Vector3 MoveDirection = Vector3.right;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Движение влево-вправо
        float horizontal = Input.GetAxisRaw("Horizontal");
        transform.Translate(MoveDirection * horizontal * Speed * Time.deltaTime, Space.World);

        // Поворот персонажа
        if (horizontal > 0)
            transform.rotation = Quaternion.LookRotation(MoveDirection);
        else if (horizontal < 0)
            transform.rotation = Quaternion.LookRotation(-MoveDirection);

        // Передаём скорость в Animator
        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        // Прыжок
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * JumpForce);
            animator.SetTrigger("Jump");
        }
    }
}