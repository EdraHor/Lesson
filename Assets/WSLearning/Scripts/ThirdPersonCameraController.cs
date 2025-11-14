using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset = new Vector3(0, 5, -7);
    public float Sensitivity = 3f;
    
    private float _rotationX, _rotationY;
    
    void LateUpdate() 
    {
        _rotationX += Input.GetAxis("Mouse X") * Sensitivity;
        _rotationY -= Input.GetAxis("Mouse Y") * Sensitivity;
        _rotationY = Mathf.Clamp(_rotationY, -40, 80);
        
        Quaternion rotation = Quaternion.Euler(_rotationY, _rotationX, 0);
        transform.rotation = rotation;
        transform.position = Target.position + rotation * Offset;
    }
}
