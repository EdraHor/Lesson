using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    public Transform Target; // Цель для следования
    public Vector3 Offset = new Vector3(0, 2, -5); // Смещение от цели
    public float FollowSpeed = 2f; // Скорость следования
    public float ZoomSpeed = 2f; // Скорость зума

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (Target == null) return;

        // Плавно следуем за целью
        Vector3 targetPosition = Target.position + Offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, FollowSpeed * Time.deltaTime);
        transform.LookAt(Target);

        // Зум колесом мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - scroll * ZoomSpeed, 20f, 80f);
    }
}
