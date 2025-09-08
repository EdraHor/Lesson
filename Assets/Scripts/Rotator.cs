using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float RotationSpeed = 50f;
    public Vector3 Direction = Vector3.up;

    void Update()
    {
        transform.Rotate(Direction * RotationSpeed * Time.deltaTime);
    }
}
