using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float Speed = 2f;
    public float Distance = 3f;

    void Update()
    {
        // Движение вперед-назад по синусоиде
        float movement = Mathf.Sin(Time.time * Speed) * Distance;
        transform.position = new Vector3(movement, transform.position.y, transform.position.z);
    }
}
